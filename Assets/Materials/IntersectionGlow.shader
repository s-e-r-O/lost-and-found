// Borrowed heavily from https://chrismflynn.wordpress.com/2012/09/06/fun-with-shaders-and-the-depth-buffer/
// Thanks Chris!
// Note that for now this only works with orthographic cameras
Shader "Intersection Glow" {
    Properties
    {
        _HighlightColor("Highlight Color", Color) = (1, 1, 1, .5) //Color when intersecting
        _HighlightThresholdMax("Highlight Threshold Max", Range(0, 0.1)) = 0.1 //Max difference for intersections
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType"="Transparent"  }

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma target 3.0
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            uniform sampler2D _CameraDepthTexture; //Depth Texture
            uniform float4 _HighlightColor;
            uniform float _HighlightThresholdMax;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float4 projPos : TEXCOORD1; //Screen position of pos
            };

            v2f vert(appdata_base v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.projPos = ComputeScreenPos(o.pos);

                return o;
            }

            half4 frag(v2f i) : COLOR
            {
                float4 finalColor;

                //Get the distance to the camera from the depth buffer for this point
                float sceneZ = tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)).r;
                if (unity_OrthoParams.w == 0)
                {
                    // Perspective TODO
                }

                //Actual distance to the camera
                float partZ = i.projPos.z;

                //If the two are similar, then there is an object intersecting with our object
                float diff = abs(sceneZ - partZ) / _HighlightThresholdMax;

                if (diff <= 1)
                {
                    finalColor = _HighlightColor;
                    finalColor.a = 1 - lerp(0, 1 - _HighlightColor.a, diff);
                }
                else
                {
                    finalColor.a = 0;
                }
                return finalColor;
            }

            ENDCG
        }
    }
}
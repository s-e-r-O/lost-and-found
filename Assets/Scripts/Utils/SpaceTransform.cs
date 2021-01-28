using UnityEngine;

public static class SpaceTransform
{
    public static Vector2 ApplyCameraPerspective(this Vector2 input, Camera camera)
    {
        return input.ApplyPerspective(camera.transform);
    }

    public static Vector2 ApplyPerspective(this Vector2 input, Transform reference)
    {
        var forward = reference.forward;
        var right = reference.right;

        //project forward and right vectors on the horizontal plane (y = 0)
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();

        //this is the direction in the world space we want to move:
        var desiredDir3 = forward * input.y + right * input.x;
        return new Vector2(desiredDir3.x, desiredDir3.z);
    }

    public static Vector2 RotatePerspective(this Vector2 input, Transform reference)
    {
        var desiredDir3 = Quaternion.Inverse(reference.rotation) * new Vector3(input.x, 0f, input.y);
        return new Vector2(desiredDir3.x, desiredDir3.z);
    }
}
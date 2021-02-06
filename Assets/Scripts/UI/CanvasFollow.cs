using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CanvasFollow : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    protected Transform target;
    
    protected TMP_Text text;
    
    private RectTransform rectTransform;
    private CanvasScaler canvasScaler;
    private PixelatedCamera gameCamera;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        canvasScaler = GetComponentInParent<CanvasScaler>();
        gameCamera = Camera.main.GetComponent<PixelatedCamera>();
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            return;
        }
        Vector3 pixelScreenPos = gameCamera.renderCamera.WorldToScreenPoint(target.position);
        if (canvasScaler.referenceResolution.x != gameCamera.targetScreenSize.width || canvasScaler.referenceResolution.y != gameCamera.targetScreenSize.height)
        {
            canvasScaler.referenceResolution = new Vector2(gameCamera.targetScreenSize.width, gameCamera.targetScreenSize.height);
        }
        //Vector2 scale = new Vector2(gameCamera.targetScreenSize.width / canvasScaler.referenceResolution.x, gameCamera.targetScreenSize.height / canvasScaler.referenceResolution.y);
        //Vector3 screenPos = new Vector3(pixelScreenPos.x / scale.x, pixelScreenPos.y / scale.y, pixelScreenPos.z);
        rectTransform.anchoredPosition = Vector3.Lerp(rectTransform.anchoredPosition, pixelScreenPos, moveSpeed * Time.deltaTime);
        //Ray ray = gameCamera.renderCamera.ScreenPointToRay(screenPos);
        //if (Physics.Raycast(ray, out RaycastHit hit))
        //{
        //    if (hit.transform.gameObject.CompareTag("Player"))
        //    {
        //        Debug.Log("Visible");
        //    }
        //    else
        //    {
        //        Debug.Log("Invisible");
        //    }
        //}


        //Debug.Log("target is " + screenPos.x + " pixels from the left");

        //gameCamera.renderCamera.ray
    }
}

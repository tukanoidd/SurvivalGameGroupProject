using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldToCanvasPlacer : MonoBehaviour
{
    public RectTransform SnappingPointUI;
    public RectTransform SnappingPointParentUI;

    public GameObject snappingPoint;
    public GameObject snappingPointParent;

    private RectTransform canvasRect;

    [SerializeField] private Camera camera;

    // Update is called once per frame
    void Update()
    {
        SetUIToCanvas(snappingPoint, SnappingPointUI);
        SetUIToCanvas(snappingPointParent, SnappingPointParentUI);
    }

    void SetUIToCanvas(GameObject obj, RectTransform UIElement)
    {
        if (obj != null)
        {
            canvasRect = GetComponent<RectTransform>();

            Vector2 ViewportPosition = camera.WorldToViewportPoint(obj.transform.position);
            Vector2 WorldObject_ScreenPosition = new Vector2(
                ((ViewportPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.5f)),
                ((ViewportPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.5f)));

            UIElement.anchoredPosition = WorldObject_ScreenPosition;
        }
    }
}
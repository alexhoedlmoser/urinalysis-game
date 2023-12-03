using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowMouse : MonoBehaviour
{
    private Canvas _parentCanvas;

    public void Start()
    {
        _parentCanvas = GetComponentInParent<Canvas>().rootCanvas;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentCanvas.transform as RectTransform, Input.mousePosition,
            _parentCanvas.worldCamera,
            out _);
    }

    public void Update()
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            _parentCanvas.transform as RectTransform,
            Input.mousePosition, _parentCanvas.worldCamera,
            out var movePos);

        transform.position = _parentCanvas.transform.TransformPoint(movePos);
    }
}

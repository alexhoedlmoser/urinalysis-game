using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DiagnoseCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    private Image cardImage;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightColor;

    private float highlightYOffset;

    private void Start()
    {
        cardImage = GetComponent<Image>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cardImage.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardImage.color = normalColor;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }
}

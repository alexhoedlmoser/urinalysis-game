using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UrinProbe : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UrinType urinType;

    [SerializeField] private Image fillImage;

    [SerializeField] private Image highlightImage;
    // Start is called before the first frame update
    void Start()
    {
        urinType = GameManager.Instance.GetRandomType();
        fillImage.color = urinType.urinColor;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        highlightImage.gameObject.SetActive(true);
        GameManager.Instance.currentMouseProbe = this;

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        highlightImage.gameObject.SetActive(false);
        GameManager.Instance.currentMouseProbe = null;
    }
}

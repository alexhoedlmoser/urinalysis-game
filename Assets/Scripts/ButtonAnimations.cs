using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class ButtonAnimations : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler
{

    [SerializeField] private Image cardImage;
    [SerializeField] private Image _highlightImage;

    [SerializeField] private Color _normalColor;
    [SerializeField] private Color _highlightColor;
    [SerializeField] private Color _clickColor;
    [SerializeField] private float hoverMoveDelta;

    private RectTransform _rectTransform;

    private float _standardPosY;
    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _standardPosY = _rectTransform.localPosition.y;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        cardImage.color = _highlightColor;
        _highlightImage.enabled = true;
        _rectTransform.DOLocalMoveY(_rectTransform.localPosition.y + hoverMoveDelta, 0.15f);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardImage.color = _normalColor;
        _highlightImage.enabled = false;
        _rectTransform.DOLocalMoveY(_standardPosY, 0.15f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        cardImage.color = _clickColor;
    }
}

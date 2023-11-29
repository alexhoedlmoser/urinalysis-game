using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;

public class UrinProbe : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public UrinType urinType;
    public bool isActive;

    [SerializeField] private Image fillImage;
    [SerializeField] private Image highlightImage;

    [SerializeField] private CanvasGroup _canvasGroup;
    private Sequence _currentBlinkSequence;
    
    // Start is called before the first frame update
    void Start()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
        SetupProbe();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetupProbe()
    {
        highlightImage.gameObject.SetActive(false);
        urinType = GameManager.Instance.GetRandomType();
        fillImage.color = urinType.urinColor;
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

    public void StartProbeBlink()
    {
        if (_currentBlinkSequence.IsActive()) return;
        _currentBlinkSequence = BlinkOut().OnComplete(() => gameObject.SetActive(false));
    }

    private Sequence BlinkOut()
    {
        return DOTween.Sequence()
            .Append(_canvasGroup.DOFade(0f, 2f).SetEase(Ease.Flash))
            .Append(_canvasGroup.DOFade(1f, 1f).SetEase(Ease.Flash))
            .Append(_canvasGroup.DOFade(0f, 0.5f).SetEase(Ease.Flash))
            .Append(_canvasGroup.DOFade(1f, 0.25f).SetEase(Ease.Flash))
            .Append(_canvasGroup.DOFade(0f, 0.25f).SetEase(Ease.Flash));
    }
}

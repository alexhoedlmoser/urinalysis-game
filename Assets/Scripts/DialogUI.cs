using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class DialogUI : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Canvas _parentCanvas;
    public TMP_Text nameText;
    public TMP_Text dialogText;
    public Image _nameBackground;
    public Image _background;
    
    [Header("Settings")]
    public Color nameColor;
    [SerializeField] private float _backgroundAlpha;
    [SerializeField] private float _endSizeY;
    [SerializeField] private float _fadeInDuration;
    [SerializeField] private float timeBetweenCharacters;
    //[SerializeField] private float timeBetweenWords;
    
    // OLD
    public float defaultTextBlendDuration = 0.25f;

    private float _standardSizeX;
    private RectTransform _rectTransform;
    
    private Coroutine _currentTextCoroutine;
    private Sequence _currentFadeSequence;

    public void DisplayLine(DialogLine line)
    {
        if (_currentTextCoroutine != null)
        {
            StopCoroutine(_currentTextCoroutine);
        }

        dialogText.text = line.englishText;
        _currentTextCoroutine = StartCoroutine(TextVisible());
    }
    
    public void DisplayName(string name)
    {
        nameText.text = name.ToUpper();
    }

    private IEnumerator TextVisible()
    {
        dialogText.ForceMeshUpdate();
        int totalCharacters = dialogText.textInfo.characterCount;
        int counter = 0;

        while (true)
        {
            int visibleCount = counter % (totalCharacters + 1);
            dialogText.maxVisibleCharacters = visibleCount;

            if (visibleCount >= totalCharacters)
            {
                break;
            }
            
            counter += 1;
            yield return new WaitForSeconds(timeBetweenCharacters);
        }
    }
    
    // Start is called before the first frame update
    private void Awake()
    {
        _parentCanvas = transform.parent.GetComponent<Canvas>();
        _rectTransform = GetComponent<RectTransform>();
        _standardSizeX = _rectTransform.sizeDelta.x;
    }
    
    public void FadeInDialog()
    {
        _parentCanvas.enabled = true;
        _rectTransform.sizeDelta = new Vector2(_standardSizeX, 0f);

        _currentFadeSequence?.Kill();
        _currentFadeSequence = FadeInDialogSequence();
    }
    
    public Sequence FadeInDialogSequence()
    {
        return DOTween.Sequence()
            .Append(_rectTransform.DOSizeDelta(new Vector2(_standardSizeX, _endSizeY), _fadeInDuration)
                .SetEase(Ease.OutQuart))
            .Join(dialogText.DOFade(1, _fadeInDuration))
            .Join(nameText.DOFade(1, _fadeInDuration))
            .Join(_background.DOFade(_backgroundAlpha, _fadeInDuration));
    }
    
    public void FadeOutDialog()
    {
        _currentFadeSequence?.Kill();
        _currentFadeSequence = FadeOutDialogSequence().OnComplete((() => _parentCanvas.enabled = false));
    }
    
    public Sequence FadeOutDialogSequence()
    {
        return DOTween.Sequence()
            .Append(_rectTransform.DOSizeDelta(new Vector2(_standardSizeX, 0), _fadeInDuration)
                .SetEase(Ease.InQuart))
            .Join(dialogText.DOFade(0, _fadeInDuration))
            .Join(nameText.DOFade(0, _fadeInDuration))
            .Join(_background.DOFade(0, _fadeInDuration));
    }

    public Sequence BlendText(string newText, float blendDuration)
    {
        return DOTween.Sequence()
            .Append(dialogText.DOFade(0, blendDuration))
            .AppendCallback(() => dialogText.text = newText)
            .Append(dialogText.DOFade(1f, blendDuration));
    }
    
    public Sequence BlendName(string newName, float blendDuration)
    {
        return DOTween.Sequence()
            .Append(nameText.DOFade(0, blendDuration))
            .AppendCallback(() => nameText.text = newName)
            .Append(nameText.DOFade(1f, blendDuration));
    }

    public void SetNameColor(Color color)
    {
        nameText.color = color;
        
        Color tmpColor = color;
        tmpColor.a = _backgroundAlpha;
        _nameBackground.color = tmpColor;
    }
}

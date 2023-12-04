using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ReviewUI : MonoBehaviour
{

    [SerializeField] private TMP_Text _correctDiagnosesText;
    [SerializeField] private TMP_Text _wrongDiagnosesText;
    [SerializeField] private TMP_Text _timeText;
    [SerializeField] private TMP_Text _gradeText;
    [SerializeField] private TMP_Text _diagnoseCountText;

    [SerializeField] private Image _gradeBackground;
    [SerializeField] private Image _background;

    [SerializeField] private float _fadeInMoveDelta;
    [SerializeField] private float _fadeDuration;
    [SerializeField] private float _backgroundAlpha;
    
    [SerializeField] private Canvas _parentCanvas;
    [SerializeField] private CanvasGroup _canvasGroup;

    private RectTransform _rectTransform;

    private float _standardPosY;

    private Sequence _currentFadeSequence;

    
    // Start is called before the first frame update
    void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _standardPosY = _rectTransform.localPosition.y;
    }

    private void OnEnable()
    {
        GameManager.Instance.OnSwitchGameState += OnStateSwitchHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnSwitchGameState -= OnStateSwitchHandler;
    }

    private void OnStateSwitchHandler(GameState state)
    {
        if (state == GameState.InReview)
        {
            SetupReviewDisplay();
            FadeInReview();
        }
    }

    private void SetupReviewDisplay()
    {
        ReviewGrade grade = GameManager.Instance.GetGrade();

        _correctDiagnosesText.text = "Correct Diagnoses: " + GameManager.Instance.correctDiagnoses.ToString();
        _wrongDiagnosesText.text = "Wrong Diagnoses: " + GameManager.Instance.wrongDiagnoses.ToString();
        _timeText.text = "Time Needed: " + GameManager.Instance.timeNeeded.ToString("F1", CultureInfo.InvariantCulture) + " seconds";
        _diagnoseCountText.text = "After " + GameManager.Instance.diagnosesPerRound + " Diagnoses";
        _gradeText.text = grade.displayText;
        _gradeBackground.color = grade.color;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private Sequence FadeInReviewSequence()
    {
        return DOTween.Sequence()
            .Append(_rectTransform.DOLocalMoveY(_standardPosY, _fadeDuration)
                .SetEase(Ease.OutBack))
            .Join(_canvasGroup.DOFade(_backgroundAlpha, _fadeDuration).SetEase(Ease.OutCubic));
    }
    
    private Sequence FadeOutReviewSequence()
    {
       return DOTween.Sequence()
            .Append(_rectTransform.DOLocalMoveY(_standardPosY + _fadeInMoveDelta, _fadeDuration)
                .SetEase(Ease.InBack))
            .Join(_canvasGroup.DOFade(0f, _fadeDuration).SetEase(Ease.InCubic));
    }
    
    public void FadeInReview()
    {
        _parentCanvas.enabled = true;
        _rectTransform.localPosition = new Vector3(_rectTransform.localPosition.x, _standardPosY + _fadeInMoveDelta,
            _rectTransform.localPosition.z);
        _canvasGroup.alpha = 0f;

        _currentFadeSequence?.Kill();
        _currentFadeSequence = FadeInReviewSequence();
    }
    
    public void FadeOutReview()
    {
        _currentFadeSequence?.Kill();
        _currentFadeSequence = FadeOutReviewSequence().OnComplete((() => _parentCanvas.enabled = false));
    }

    public void RestartGame()
    {
        GameManager.Instance.RestartGame();
        FadeOutReview();
    }

    public void QuitGame()
    {
        GameManager.Instance.QuitGame();
    }
}

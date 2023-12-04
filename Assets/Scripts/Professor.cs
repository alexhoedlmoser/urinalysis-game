using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using Sequence = DG.Tweening.Sequence;

public class Professor : MonoBehaviour
{

    public DialogInteraction dialog;

    [SerializeField] private Sprite[] _inDialogSprites;
    [SerializeField] private Sprite _idleSprite;
    [SerializeField] private Sprite _falseHintSprite;
    [SerializeField] private Sprite _trueHintSprite;
    [SerializeField] private TMP_Text _scoreFeed;
    [SerializeField] private Color _scorePlusColor;
    [SerializeField] private Color _scoreMinusColor;

    private Image _image;

    private RectTransform _rectTransform;

    private RectTransform _scoreFeedTransform;
    private float _scoreFeedStandardPosX;

    private Sequence _currentScoreSequence;
    // Start is called before the first frame update
    void Start()
    {
        _image = GetComponent<Image>();
        _rectTransform = GetComponent<RectTransform>();
        dialog = GetComponent<DialogInteraction>();

        _scoreFeedTransform = _scoreFeed.gameObject.GetComponent<RectTransform>();
        _scoreFeedStandardPosX = _scoreFeedTransform.localPosition.x;
        
        dialog.StartDialogFromQueue();
        
        
        StartIdleLoop();
    }

    private void OnEnable()
    {
        GameManager.Instance.OnSwitchGameState += OnStateSwitchHandler;
        GameManager.Instance.OnScoreChange += OnScoreChangeHandler;
    }

    private void OnDisable()
    {
        GameManager.Instance.OnSwitchGameState -= OnStateSwitchHandler;
        GameManager.Instance.OnScoreChange -= OnScoreChangeHandler;
    }

    private void OnScoreChangeHandler(bool plus)
    {
        if (plus)
        {
            _scoreFeed.color = _scorePlusColor;
            _scoreFeed.text = "+";
        }
        else
        {
            _scoreFeed.color = _scoreMinusColor;
            _scoreFeed.text = "-";
        }

        _currentScoreSequence?.Kill();
        _currentScoreSequence = ScoreVisualSequence();
    }

    private void OnStateSwitchHandler(GameState state)
    {
        switch (state)
        {
            case GameState.InProgress:
                _image.sprite = _idleSprite;
                break;
            case GameState.InDialog:
                _image.sprite = _inDialogSprites[Random.Range(0, _inDialogSprites.Length)];
                break;
            case GameState.InReview:
                _image.sprite = _idleSprite;
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }

    private Sequence IdleLoopSequence()
    {
        return DOTween.Sequence()
            .Append(_rectTransform.DOLocalMoveY(0f, 0f))
            .AppendInterval(1f)
            .Append(_rectTransform.DOLocalMoveY(-5f, 0f));
    }

    private void StartIdleLoop()
    {
        IdleLoopSequence().SetLoops(-1, LoopType.Yoyo).Play();
    }

    private Sequence ScoreVisualSequence()
    {
        _scoreFeedTransform.localScale = Vector3.zero;
        _scoreFeedTransform.localPosition = new Vector3(_scoreFeedStandardPosX, _scoreFeedTransform.localPosition.y);

        return DOTween.Sequence()
            .Append(_scoreFeedTransform.DOScale(1f, 0.25f).SetEase(Ease.OutBack))
            .Join(_scoreFeedTransform.DOLocalMoveX(_scoreFeedStandardPosX + 50f, 2f))
            .Append(_scoreFeedTransform.DOScale(0f, 0.25f).SetEase(Ease.InBack));
    }
    
    
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using DG.Tweening;
using UnityEditor;
using UnityEngine.Serialization;

public class UrineProbe : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [FormerlySerializedAs("urinType")] public UrineType urineType;
    public Transform cardLayout;
    public bool isActive;

    [SerializeField] private Image fillImage;
    [SerializeField] private Image highlightImage;

    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private float _blinkInterval = 0.1f;
    [SerializeField] private float _respawnCooldown = 3f;
    
    private Coroutine _currentBlinkCoroutine;
    private Coroutine _currentRespawnCoroutine;
    private bool _currentlyBlinking;
    private bool _currentlyRespawning;
    public List<DiagnoseType> typeHistory;
    private bool _isActive;

    // Start is called before the first frame update
    void Start()
    {
       
        highlightImage.gameObject.SetActive(false);
        SetupProbe();
    }

    private void Awake()
    {
        _canvasGroup = GetComponent<CanvasGroup>();
    }

    private void OnEnable()
    {
        //GameManager.Instance.OnSwitchGameState -= OnStateSwitchHandler;
        //GameManager.Instance.OnSwitchGameState += OnStateSwitchHandler;
    }

    /*private void OnStateSwitchHandler(GameState state)
    {
        if(!isActive) return;
        
        switch (state)
        {
            case GameState.InProgress:
                _canvasGroup.interactable = true;
                _canvasGroup.blocksRaycasts = true;
                break;
            case GameState.InDialog:
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
                break;
            case GameState.InReview:
                _canvasGroup.interactable = false;
                _canvasGroup.blocksRaycasts = false;
                break;
            case GameState.InDisclaimer:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(state), state, null);
        }
    }*/

    public void SetupProbe()
    {
        AssignUrineType();
        ToggleProbe(true);
        ResetCardsOnProbe();
        
        if (_currentlyBlinking)
        {
            StopCoroutine(_currentBlinkCoroutine);
        }
        
        if (_currentlyRespawning)
        {
            StopCoroutine(_currentRespawnCoroutine);
        }
    }

    private void AssignUrineType()
    {
        urineType = GameManager.Instance.GetRandomType();
        fillImage.sprite = urineType.urinFillImage;
        fillImage.color = urineType.urinColor;
        ResetTypeHistory();
    }

    private void ToggleProbe(bool toggle)
    {
        _canvasGroup.alpha = toggle ? 1f : 0f;
        _isActive = toggle;
        
        _canvasGroup.interactable = toggle;
        _canvasGroup.blocksRaycasts = toggle;
    }

    private void ResetCardsOnProbe()
    {
        foreach (Transform child in cardLayout)
        {
            Destroy(child.gameObject);
        }
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
        if (_currentlyBlinking)
        {
            StopCoroutine(_currentBlinkCoroutine);
        }

        if (_currentlyRespawning)
        {
            StopCoroutine(_currentRespawnCoroutine);
        }
        
        _currentBlinkCoroutine = StartCoroutine(BlinkOut());
    }

    private void StartRespawn()
    {
        if (_currentlyRespawning)
        {
            StopCoroutine(_currentRespawnCoroutine);
        }
        
        _currentRespawnCoroutine = StartCoroutine(RespawnProbe());
    }

    private IEnumerator BlinkOut()
    {
        _currentlyBlinking = true;
        _canvasGroup.alpha = 1f;
        
        yield return new WaitUntil( () => GameManager.Instance.gameState == GameState.InProgress);
        yield return new WaitForSeconds(3f);
        yield return new WaitUntil( () => GameManager.Instance.gameState == GameState.InProgress);
        
        _canvasGroup.alpha = 0f;
        yield return new WaitForSeconds(_blinkInterval);
        _canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(1f);
        _canvasGroup.alpha = 0f;
        yield return new WaitForSeconds(_blinkInterval);
        _canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(0.5f);
        _canvasGroup.alpha = 0f;
        yield return new WaitForSeconds(_blinkInterval);
        _canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(0.25f);
        _canvasGroup.alpha = 0f;
        yield return new WaitForSeconds(_blinkInterval);
        _canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(_blinkInterval);
        _canvasGroup.alpha = 0f;
        yield return new WaitForSeconds(_blinkInterval);
        _canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(_blinkInterval);
        
        
        ToggleProbe(false);
        ResetCardsOnProbe();
        
        _currentlyBlinking = false;
        
        StartRespawn();
    }

    private IEnumerator RespawnProbe()
    {
        _currentlyRespawning = true;
        yield return new WaitForSeconds(_respawnCooldown);
        
        AssignUrineType();
        ToggleProbe(true);
        
        _canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(_blinkInterval);
        _canvasGroup.alpha = 0f;
        yield return new WaitForSeconds(_blinkInterval);
        _canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(_blinkInterval);
        _canvasGroup.alpha = 0f;
        yield return new WaitForSeconds(_blinkInterval);
        _canvasGroup.alpha = 1f;
        yield return new WaitForSeconds(_blinkInterval);
        _canvasGroup.alpha = 0f;                         
        yield return new WaitForSeconds(_blinkInterval); 
        _canvasGroup.alpha = 1f;

        _currentlyRespawning = false;
    }

    public bool CheckTypeHistory(DiagnoseType diagnoseType)
    {
        if (typeHistory.Contains(diagnoseType))
        {
            return false;
        }
        else
        {
            typeHistory.Add(diagnoseType);
            return true;
        }
    }

    private void ResetTypeHistory()
    {
        typeHistory.Clear();
    }
}

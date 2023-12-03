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
    public List<DiagnoseType> typeHistory;

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
        AssignUrineType();
        ToggleProbe(true);
    }

    private void AssignUrineType()
    {
        urinType = GameManager.Instance.GetRandomType();
        fillImage.color = urinType.urinColor;
        ResetTypeHistory();
    }

    public void ToggleProbe(bool toggle)
    {
        _canvasGroup.interactable = toggle;
        _canvasGroup.alpha = toggle ? 1f : 0f;
        _canvasGroup.blocksRaycasts = toggle;
    }

    public void ResetCardsOnProbe()
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

        if (_currentRespawnCoroutine != null)
        {
            StopCoroutine(_currentRespawnCoroutine);
        }
        
        _currentBlinkCoroutine = StartCoroutine(BlinkOut());
    }

    private void StartRespawn()
    {
        _currentRespawnCoroutine = StartCoroutine(RespawnProbe());
    }

    private IEnumerator BlinkOut()
    {
        _currentlyBlinking = true;
        _canvasGroup.alpha = 1f; 
        
        yield return new WaitUntil( () => GameManager.Instance.gameState == GameState.InProgress);
        
        yield return new WaitForSeconds(1f);
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
        yield return new WaitForSeconds(0.25f);
        
        ToggleProbe(false);
        ResetCardsOnProbe();
        
        _currentlyBlinking = false;
        
        StartRespawn();
    }

    private IEnumerator RespawnProbe()
    {
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

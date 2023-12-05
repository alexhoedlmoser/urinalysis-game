using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Mime;
using UnityEngine;
using DG.Tweening;
using TMPro;
using UnityEngine.Serialization;

public class Disclaimer : MonoBehaviour
{

    [SerializeField] private CanvasGroup _promptCanvasGroup;
    [SerializeField] private float _promptFadeDuration;
    [SerializeField] private TMP_Text _disclaimerText;
    [SerializeField] private float _timeBetweenCharacters = 0.025f;

    private RectTransform _promptTransform;
    // Start is called before the first frame update
    void Start()
    {
        _promptTransform = _promptCanvasGroup.gameObject.GetComponent<RectTransform>();

        _promptCanvasGroup.alpha = 0f;
        StartCoroutine(TextVisible());
        
        GameManager.Instance.SwitchGameState(GameState.InDisclaimer);
    }

    private void OnEnable()
    {
        GameManager.CurrentInputHandler.OnSkip += OnSkipHandler;
    }

    private void OnSkipHandler()
    {
        GameManager.Instance.LoadGameScene(1);
    }

    private IEnumerator TextVisible()
    {
        _disclaimerText.ForceMeshUpdate();
        int totalCharacters = _disclaimerText.textInfo.characterCount;
        int counter = 0;

        while (true)
        {
            int visibleCount = counter % (totalCharacters + 1);
            _disclaimerText.maxVisibleCharacters = visibleCount;

            if (visibleCount >= totalCharacters)
            {
                _promptCanvasGroup.DOFade(1f, _promptFadeDuration);
                _promptTransform.DOLocalMoveY(_promptTransform.localPosition.y + 10f, _promptFadeDuration)
                    .SetEase(Ease.OutCubic);
                break;
                
            }
            
            counter += 1;
            yield return new WaitForSeconds(_timeBetweenCharacters);
        }
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum DiagnoseType
{
    Dehydrated,
    Overhydrated,
    Infection,
    Hematuria,
    VitaminOverdose,
    Healthy,
    RedBeetJuice,
    LiverIssue,
    MethyleneDye,
    Rhubarb,
    Rhabdomyolosis,
    PhysicalActivity,
    KidneyDisease,
    Diabetes
    
}

public class DiagnoseCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Image cardImage;

    public DiagnoseType diagnoseType;
    public DialogSequence explanationDialog;
    public bool explainDiagnose;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color clickColor;

    [SerializeField] private float hoverMoveDelta;
    private float _standardPosY;

    [SerializeField] private Image _highlightImage;
    [SerializeField] private GameObject draggingPrefab;
    [SerializeField] private GameObject onProbePrefab;
    [SerializeField] private Vector3 draggingRotation;
    [SerializeField] private TMP_Text cardLabel;

    private RectTransform _rectTransform;
    private float highlightYOffset;
    private GameObject currentMouseObject;

    private Transform draggingParent;

    private void Start()
    {
        _rectTransform = GetComponent<RectTransform>();
        _standardPosY = _rectTransform.localPosition.y;
        
        cardImage = GetComponent<Image>();
        draggingParent = GetComponentInParent<Canvas>().rootCanvas.transform;
        SetLabel();
    }

    private void SetLabel()
    {
        cardLabel.text = GameManager.Instance.GetDiagnoseLabel(diagnoseType);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        cardImage.color = highlightColor;
        _highlightImage.enabled = true;
        _rectTransform.DOLocalMoveY(_rectTransform.localPosition.y + hoverMoveDelta, 0.15f);

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardImage.color = normalColor;
        _highlightImage.enabled = false;
        _rectTransform.DOLocalMoveY(_standardPosY, 0.15f);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        cardImage.color = clickColor;
    }

    public void OnDrag(PointerEventData eventData)
    {
        //
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        currentMouseObject = Instantiate(draggingPrefab, Input.mousePosition, Quaternion.Euler(draggingRotation), draggingParent);
        currentMouseObject.GetComponent<DiagnoseCardDrag>().SetLabel(cardLabel.text);
        GameManager.Instance.currentMouseDiagnose = this;
        GameManager.Instance.SetDragCursor();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.currentMouseProbe)
        {
            UrineProbe urineProbe = GameManager.Instance.currentMouseProbe;
            UrineType urineType = urineProbe.urineType;

            if (urineProbe.CheckTypeHistory(diagnoseType))
            {
                DiagnoseCardOnProbe diagnoseCardOnProbe = Instantiate(onProbePrefab, Vector3.zero, Quaternion.identity, urineProbe.cardLayout).GetComponent<DiagnoseCardOnProbe>();
                diagnoseCardOnProbe.SetLabel(cardLabel.text);
                
                if (urineType.diagnoseTypes.Contains(diagnoseType))
                {
                    print("correct");
                    diagnoseCardOnProbe.SetBackgroundColor(true);

                    if (explainDiagnose)
                    {
                        int index = Array.IndexOf(urineType.diagnoseTypes, diagnoseType);
                        
                        GameManager.CurrentProfessor.dialog.StartDialog(explanationDialog);
                        explainDiagnose = false;
                        
                    }
                    GameManager.Instance.IncreaseScore();
                }
                else
                {
                    print("uncorrect");
                    diagnoseCardOnProbe.SetBackgroundColor(false);
                    GameManager.Instance.DecreaseScore();
                }
                
                urineProbe.StartProbeBlink();
            }
            
            //GameManager.Instance.currentMouseProbe = null;
            //CheckProbes();
        }
        
        Destroy(currentMouseObject);
        GameManager.Instance.currentMouseDiagnose = null;
        GameManager.Instance.SetNormalCursor();
    }

    private void CheckProbes()
    {
        if (GameManager.Instance.allProbes.Any(probe => probe.gameObject.activeSelf))
        {
            return;
        }

        foreach (var probe in GameManager.Instance.allProbes)
        {
            probe.gameObject.SetActive(true);
            probe.SetupProbe();
        }
    }


}

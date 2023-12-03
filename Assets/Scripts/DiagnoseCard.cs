using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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
    public bool explainDiagnose;

    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightColor;
    [SerializeField] private Color clickColor;
    [SerializeField] private GameObject draggingPrefab;
    [SerializeField] private GameObject onProbePrefab;
    [SerializeField] private Vector3 draggingRotation;
    [SerializeField] private TMP_Text cardLabel;

    private float highlightYOffset;
    private GameObject currentMouseObject;

    private Transform draggingParent;

    private void Start()
    {
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
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        cardImage.color = normalColor;
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
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.currentMouseProbe)
        {

            UrinProbe urinProbe = GameManager.Instance.currentMouseProbe;
            UrinType urinType = urinProbe.urinType;

            if (urinProbe.CheckTypeHistory(diagnoseType))
            {
                DiagnoseCardOnProbe diagnoseCardOnProbe = Instantiate(onProbePrefab, Vector3.zero, Quaternion.identity, urinProbe.cardLayout).GetComponent<DiagnoseCardOnProbe>();
                diagnoseCardOnProbe.SetLabel(cardLabel.text);
                
                if (urinType.diagnoseTypes.Contains(diagnoseType))
                {
                    print("correct");
                    GameManager.Instance.IncreaseScore();
                    diagnoseCardOnProbe.SetBackgroundColor(true);

                    if (explainDiagnose)
                    {
                        int index = Array.IndexOf(urinType.diagnoseTypes, diagnoseType);

                        if (urinType.diagnoseExplanations.Length > index)
                        {
                            GameManager.CurrentProfessor.dialog.StartDialog(urinType.diagnoseExplanations[index]);
                            explainDiagnose = false;
                        }
                    }
                }
                else
                {
                    print("uncorrect");
                    GameManager.Instance.DecreaseScore();
                    diagnoseCardOnProbe.SetBackgroundColor(false);
                }
                
                urinProbe.StartProbeBlink();
            }
            
            //GameManager.Instance.currentMouseProbe = null;
            //CheckProbes();
        }
        
        Destroy(currentMouseObject);
        GameManager.Instance.currentMouseDiagnose = null;
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

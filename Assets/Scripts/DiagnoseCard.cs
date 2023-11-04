using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum DiagnoseType
{
    Dehydrated,
    Overhydrated,
    Infection,
    Blood,
    VitaminOverdose,
    Healthy
}

public class DiagnoseCard : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler, IDragHandler, IBeginDragHandler, IEndDragHandler
{
    private Image cardImage;

    public DiagnoseType diagnoseType;
    [SerializeField] private Color normalColor;
    [SerializeField] private Color highlightColor;
    [SerializeField] private GameObject draggingPrefab;
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

    public void OnPointerClick(PointerEventData eventData)
    {
        throw new System.NotImplementedException();
    }

    public void OnDrag(PointerEventData eventData)
    {
        currentMouseObject.transform.position = Input.mousePosition;
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        currentMouseObject = Instantiate(draggingPrefab, Input.mousePosition, Quaternion.identity, draggingParent);
        currentMouseObject.GetComponent<DiagnoseCardDrag>().label.text = cardLabel.text;
        GameManager.Instance.currentMouseDiagnose = this;
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (GameManager.Instance.currentMouseProbe)
        {
            UrinType urinType = GameManager.Instance.currentMouseProbe.urinType;
            if (urinType.diagnoseTypes.Contains(diagnoseType))
            {
                print("correct");
                GameManager.Instance.IncreaseScore();
            }
            else
            {
                print("uncorrect");
                GameManager.Instance.DecreaseScore();
            }
        }
        
        Destroy(currentMouseObject);
        GameManager.Instance.currentMouseDiagnose = null;
    }
}

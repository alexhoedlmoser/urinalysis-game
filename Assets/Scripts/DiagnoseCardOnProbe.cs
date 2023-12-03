using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DiagnoseCardOnProbe : MonoBehaviour
{
    public TMP_Text label;
    public Color correctColor;
    public Color uncorrectColor;
    private Image _backgroundImage;

    private void Awake()
    {
        _backgroundImage = GetComponent<Image>();
    }

    public void SetLabel(string text)
    {
        label.text = text;
    }

    public void SetBackgroundColor(bool correct)
    {
        _backgroundImage.color = correct ? correctColor : uncorrectColor;
    }
}

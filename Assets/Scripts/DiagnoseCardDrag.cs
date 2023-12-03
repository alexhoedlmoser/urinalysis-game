using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DiagnoseCardDrag : MonoBehaviour
{
    public TMP_Text label;
    
    public void SetLabel(string text)
    {
        label.text = text;
    }
}

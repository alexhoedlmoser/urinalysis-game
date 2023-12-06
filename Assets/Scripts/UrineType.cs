using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Urin Type", menuName = "New Urin Type")]
public class UrineType : ScriptableObject
{
    [FormerlySerializedAs("_diagnoseTypes")] public DiagnoseType[] diagnoseTypes; 
    //public DialogSequence[] diagnoseExplanations;
    public Color urinColor;
    public float urinAlpha;
    public Sprite urinFillImage;

}


using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "Urin Type", menuName = "New Urin Type")]
public class UrinType : ScriptableObject
{
    [FormerlySerializedAs("_diagnoseTypes")] public DiagnoseType[] diagnoseTypes; 
    public DialogSequence[] diagnoseExplanations;
    public Color urinColor;
    
}


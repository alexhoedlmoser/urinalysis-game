using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

[CreateAssetMenu(fileName = "New Dialog Line", menuName = "Dialog System/Dialog Line")]
public class DialogLine : ScriptableObject
{
    public AudioClip audioClip;

    [TextArea(5, 5)] public string englishText;
    [TextArea(5, 5)] public string germanText;

    public float duration;
}

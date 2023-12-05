using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dialog Sequence", menuName = "Dialog System/Dialog Sequence")]
public class DialogSequence : ScriptableObject
{
   public DialogLine[] dialogLines;

   //public InteractionCondition[] conditions;
   
   public bool onlyOnce; 
   public bool isFinished = false;
   public bool invokeEvent = false;
   public bool useQueue;
   
   // public bool CheckDialogConditions()
   // {
   //    //return conditions.All(condition => condition.Check());
   // }
}



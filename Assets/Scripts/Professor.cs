using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Professor : MonoBehaviour
{

    public DialogInteraction dialog;
    // Start is called before the first frame update
    void Start()
    {
        dialog = GetComponent<DialogInteraction>();
        dialog.StartDialogFromQueue();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

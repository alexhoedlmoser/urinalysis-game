using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class DialogInteraction : MonoBehaviour
{
    
    [Header("Dialog UI")] 
    [SerializeField] private DialogUI _dialogUI;
    [SerializeField] private string _promptLabelSkip;
    [SerializeField] private string _characterName;
    [SerializeField] private Color _headlineColor;
    
    [Header("Dialog")] 
    public DialogCategory[] dialogCategories;
    public int activeDialogCategoryIndex;

    [SerializeField] private DialogEvent[] _dialogEvents;
    
    private int _currentLineIndex;
    private int _currentPassiveDialogIndex = 0;
    private Coroutine _currentDialogCoroutine;
    
    private AudioSource _audioSource;
    private DialogSequence _currentDialog;
    private GameManager _gameManager;
   
    private bool _interactionComplete;

    //[HideInInspector] public bool lockPassiveDialog;
    
    private void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        ResetDialogs();
        InitDialogQueues();
    }

    private void OnEnable()
    {
        _gameManager = GameManager.Instance;
       //_gameManager.OnGamePaused += OnGamePausedHandler;
    }

    private void OnDisable()
    {
        //_gameManager.OnGamePaused -= OnGamePausedHandler;
    }

    private void ResetDialogs()
    {
        // Reset all dialogs to unfinished state
        // THIS BOOL SHOULD SAVE AND LOAD WITH PLAYER SAVE
        foreach (DialogSequence dialogSequence in dialogCategories[activeDialogCategoryIndex].dialogSequences)
        {
            dialogSequence.isFinished = false;
        }
    }
    
    
    public void SwitchDialogCategoryByKey(string key)
    {
        for (var index = 0; index < dialogCategories.Length; index++)
        {
            var dialogCategory = dialogCategories[index];
            if (dialogCategory.key == key)
            {
                activeDialogCategoryIndex = index;
            }
        }

        _currentPassiveDialogIndex = 0;
    }

    public void SwitchDialogCategoryByIndex(int index)
    {
        activeDialogCategoryIndex = index;
        _currentPassiveDialogIndex = 0;
    }
    private void InitDialogQueues()
    {
        // Populate the dialog queues
        foreach (DialogSequence dialogSequence in dialogCategories[activeDialogCategoryIndex].dialogSequences)
        {
            if (!dialogSequence.isFinished)
            {
                dialogCategories[activeDialogCategoryIndex]._activeDialogQueue.Add(dialogSequence);
            }
            
            else if (!dialogSequence.onlyOnce)
            {
                dialogCategories[activeDialogCategoryIndex]._passiveDialogQueue.Add(dialogSequence);
            }
        }
    }
    
    public void OnInteract()
    {
        //StartDialog();
    }

    public void StartDialogFromQueue()
    {
        StartDialog(PickDialog());
    }
    
    private DialogSequence PickDialog()
    {
        foreach (DialogSequence dialogSequence in dialogCategories[activeDialogCategoryIndex]._activeDialogQueue)
        {
            return dialogSequence;
        }

        if (dialogCategories[activeDialogCategoryIndex]._passiveDialogQueue.Count > 0)
        {
            
            // restart from the beginning if all passive dialogs have been spoken
            if (_currentPassiveDialogIndex >= dialogCategories[activeDialogCategoryIndex]._passiveDialogQueue.Count)
            {
                _currentPassiveDialogIndex = 0;
            }
            
            DialogSequence dialog = dialogCategories[activeDialogCategoryIndex]._passiveDialogQueue[_currentPassiveDialogIndex];
            _currentPassiveDialogIndex++;

            return dialog;
        }
        
        return null;
    }

    private void OnGamePausedHandler(bool paused)
    {
        if (paused)
        {
            _audioSource.Pause();
            return;
        }
        
        if (_currentDialogCoroutine != null)
        {
            _audioSource.Play();
        }
    }

    private IEnumerator RunSequence(int startingLineIndex, DialogSequence dialog)
    {
        if (startingLineIndex >= dialog.dialogLines.Length)
        {
            FinishDialog(dialog);
            yield break;
        }
            
        for (int i = startingLineIndex; i < dialog.dialogLines.Length; i++)
        {
            _currentLineIndex = i;
            _dialogUI.DisplayLine(dialog.dialogLines[i]);

            //_audioSource.clip = dialog.dialogLines[i].audioClip;
            //_audioSource.Play();

            if (dialog.dialogLines[i].audioClip)
            {
                yield return new WaitForSeconds(dialog.dialogLines[i].audioClip.length);
            }
            else
            {
                yield return new WaitForSeconds(dialog.dialogLines[i].duration);
            }
        }
        
        FinishDialog(dialog);
    }
    
    private IEnumerator RunOneShot(DialogSequence dialog)
    {
        
        for (int i = 0; i < dialog.dialogLines.Length; i++)
        {
            _currentLineIndex = i;
            //_playerUI.subtitleDisplay.DisplaySubtitle(dialog.dialogLines[i]);
            
            _audioSource.clip = dialog.dialogLines[i].audioClip;
            _audioSource.Play();

            if (dialog.dialogLines[i].audioClip)
            {
                yield return new WaitForSeconds(dialog.dialogLines[i].audioClip.length);
            }
            else
            {
                yield return new WaitForSeconds(5f);
            }
        }
        
        _currentDialogCoroutine = null;
        _audioSource.Stop();
        //_playerUI.subtitleCanvas.enabled = false;
    }


    public void StartDialog(DialogSequence dialogSequence)
    {
        GameManager.Instance.gameState = GameState.InDialog;
        print(GameManager.CurrentInputHandler);
        GameManager.CurrentInputHandler.playerInput.onActionTriggered += OnInteractHandler;
        
        _dialogUI.DisplayName(_characterName.ToUpper());
        //_dialogUI.SetNameColor(_headlineColor);
        _dialogUI.FadeInDialog();
        
        if (_currentDialogCoroutine != null)
        {
            StopCoroutine(_currentDialogCoroutine);
        }

        _currentDialog = dialogSequence;
        print(_currentDialog);
        _currentDialogCoroutine = StartCoroutine(RunSequence(0, _currentDialog));
    }
    
    private void FinishDialog(DialogSequence dialog)
    {
        _currentLineIndex = 0;
        _currentDialogCoroutine = null;
        if (!dialog.isFinished)
        {
            if (dialog.invokeEvent)
            {
                InvokeDialogEvent(dialog);
            }
            dialog.isFinished = true;
        }
        
        //_audioSource.Stop();
        dialogCategories[activeDialogCategoryIndex]._activeDialogQueue.Remove(dialog);
        
        if (!dialog.onlyOnce && !dialogCategories[activeDialogCategoryIndex]._passiveDialogQueue.Contains(dialog))
        {
            dialogCategories[activeDialogCategoryIndex]._passiveDialogQueue.Add(dialog);
        }

        _dialogUI.FadeOutDialog();
        _interactionComplete = true;
        
        GameManager.Instance.gameState = GameState.InProgress;
        
        GameManager.CurrentInputHandler.playerInput.onActionTriggered -= OnInteractHandler;

    }
    
    private void OnInteractHandler(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.performed) return;
      
        if (callbackContext.action == GameManager.CurrentInputHandler.skipAction)
        {
            if (_currentDialogCoroutine != null)
            {
                //_audioSource.Pause();
                StopCoroutine(_currentDialogCoroutine);
            }
        
            _currentDialogCoroutine = StartCoroutine(RunSequence(_currentLineIndex+1, _currentDialog));
        }
    }

    public void TestEvent()
    {
        Debug.Log("test event triggered");
    }

    private void InvokeDialogEvent(DialogSequence dialogSequence)
    {
        foreach (DialogEvent dialogFinishedEvent in _dialogEvents)
        {
            if (dialogFinishedEvent.dialogSequence == dialogSequence)
            {
                dialogFinishedEvent.eventToInvoke.Invoke();
                return;
            }
        }
    }
    
}

[System.Serializable]
public class DialogEvent
{
    public DialogSequence dialogSequence;
    public UnityEvent eventToInvoke;
}


[System.Serializable]
public class DialogCategory
{
    public string key;
    public DialogSequence[] dialogSequences;
    public List<DialogSequence> _activeDialogQueue = new List<DialogSequence>(); 
    public List<DialogSequence> _passiveDialogQueue = new List<DialogSequence>();
}
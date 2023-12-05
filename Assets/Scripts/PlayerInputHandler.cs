using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public PlayerInput playerInput;
    public InputAction skipAction;

    public event Action OnSkip; 

    // Start is called before the first frame update
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        skipAction = playerInput.actions["Skip"];
    }

    private void OnEnable()
    {
        playerInput.onActionTriggered += OnInputHandler;
    }

    private void OnDisable()
    {
        playerInput.onActionTriggered -= OnInputHandler;
    }

    private void OnInputHandler(InputAction.CallbackContext callbackContext)
    {
        if (!callbackContext.performed) return;
      
        if (callbackContext.action == GameManager.CurrentInputHandler.skipAction)
        {
            OnSkip?.Invoke();
        }
    }
}

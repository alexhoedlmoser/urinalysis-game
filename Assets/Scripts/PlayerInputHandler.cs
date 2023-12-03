using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputHandler : MonoBehaviour
{
    public PlayerInput playerInput;
    public InputAction skipAction;
    
    // Start is called before the first frame update
    void Awake()
    {
        playerInput = GetComponent<PlayerInput>();
        skipAction = playerInput.actions["Skip"];
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

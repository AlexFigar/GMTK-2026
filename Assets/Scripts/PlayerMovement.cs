using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(BoxCollider2D))]
public class PlayerMovement : MonoBehaviour
{


    private Vector2 velocity;
    private CharController charController;

    InputAction moveAction, jumpAction;

    private void Awake()
    {
        charController = GetComponent<CharController>();
        moveAction = InputSystem.actions["Move"];
        jumpAction = InputSystem.actions["Jump"];

        jumpAction.performed += Jump;
    }

    private void Jump(InputAction.CallbackContext context)
    {
        charController.Jump();
    }

    private void FixedUpdate()
    {
        
    }

    private void Update()
    {
        charController.Move(moveAction.ReadValue<Vector2>());
    }

}

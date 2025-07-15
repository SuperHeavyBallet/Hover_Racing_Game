using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input_Receiver : MonoBehaviour
{

    private Ship_Movement SCRIPT_Ship_Movement;

    private Vector2 moveInput = Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SCRIPT_Ship_Movement = GetComponent<Ship_Movement>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Move(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();

        SCRIPT_Ship_Movement.UpdateMovement(moveInput);

    }

    public void Boost(InputAction.CallbackContext ctx)
    {
       if(ctx.performed)
        {
            Debug.Log("BOOST!");
            SCRIPT_Ship_Movement.ActivateBoost(true);
        }
       else if (ctx.canceled)
        {
            SCRIPT_Ship_Movement.ActivateBoost(false);
        }
    }
}

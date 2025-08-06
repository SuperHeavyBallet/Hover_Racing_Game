using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class Input_Receiver : MonoBehaviour
{

    private Ship_Movement SCRIPT_Ship_Movement;
    private ShipWeaponRouter SCRIPT_Ship_WeaponRouter;

    private Vector2 moveInput = Vector2.zero;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        SCRIPT_Ship_Movement = GetComponent<Ship_Movement>();
        SCRIPT_Ship_WeaponRouter = GetComponent<ShipWeaponRouter>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Thrust(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            SCRIPT_Ship_Movement.UpdateThrust(true);
        }
        else if (ctx.canceled)
        {
            SCRIPT_Ship_Movement.UpdateThrust(false);
        }
    }


    public void Move(InputAction.CallbackContext ctx)
    {
        moveInput = ctx.ReadValue<Vector2>();

        SCRIPT_Ship_Movement.UpdateSteering(moveInput);

    }

    public void Boost(InputAction.CallbackContext ctx)
    {
       if(ctx.performed)
        {
            SCRIPT_Ship_Movement.ActivateBoost(true);
        }
       else if (ctx.canceled)
        {
            SCRIPT_Ship_Movement.ActivateBoost(false);
        }
    }

    public void SurgeBoost(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            SCRIPT_Ship_Movement.AddSurgeBoost();
        }
        else if(ctx.canceled)
        {
            SCRIPT_Ship_Movement.StopSurgeBoost();
        }
    }

    public void Limit(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            SCRIPT_Ship_Movement.ActivateLimit(true);
        }
        else if(ctx.canceled)
        {
            SCRIPT_Ship_Movement.ActivateLimit(false);
        }
    }

    public void BoostRight(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            SCRIPT_Ship_Movement.AddSideBoost_Right(true);
        }
        else if(ctx.canceled)
        {
            SCRIPT_Ship_Movement.AddSideBoost_Right(false);
        }
       
    }

    public void BoostLeft(InputAction.CallbackContext ctx)
    {
        if (ctx.performed)
        {
            SCRIPT_Ship_Movement.AddSideBoost_Left(true);
        }
        else if (ctx.canceled)
        {
            SCRIPT_Ship_Movement.AddSideBoost_Left(false);
        }

    }

    public void Fire(InputAction.CallbackContext ctx)
    {
        if(ctx.performed)
        {
            SCRIPT_Ship_WeaponRouter.ShipFireWeapon();
        }

        else if(ctx.canceled)
        {
            SCRIPT_Ship_WeaponRouter.ShipCeaseFireWeapon();
        }
    }
}

using System.Collections;
using UnityEngine;

public class PlayerGroundedState : PlayerBaseState, IRootState
{
    public PlayerGroundedState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(
        currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public void HandleGravity()
    {
        Ctx.CurrentMovementY = Ctx.GroundedGravity;
        Ctx.AppliedMovementY = Ctx.GroundedGravity;
    }

    public override void EnterState()
    {
        InitializeSubState();
        HandleGravity();
        Ctx.JetpackAlreadyUsed = false;
    }

    public override void UpdateState()
    {
        Ctx.DashRemainingCooldown -= Time.deltaTime;
        if (Ctx.DashRemainingCooldown <= 0)
        {
            Ctx.DashAlreadyUsed = false;
            Ctx.DashRemainingCooldown = 0;
        }
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.RemainingCoyoteTime = Ctx.CoyoteTime;
    }

    public override void CheckSwitchStates()
    {
        // Si el jugador esta en el suelo y se pulsa saltar, cambia el estado al PlayerJumpState
        if (Ctx.IsJumpPressed && !Ctx.RequireNewJumpPress)
        {
            SwitchState(Factory.Jump());
        }
        else if (!Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Fall());
        }else if (Ctx.IsEarthPressed && Ctx.Interactable is EarthWall)
        {
            SwitchState(Factory.Burrow());
        }
    }

    public override void InitializeSubState()
    {
        if (Ctx.DashPressed && !Ctx.DashAlreadyUsed)
        {
            SetSubState(Factory.Dash());
        }
        else if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Walk());
        }
    }
}
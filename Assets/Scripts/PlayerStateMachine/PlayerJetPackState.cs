using System;
using UnityEngine;

public class PlayerJetPackState : PlayerBaseState, IRootState
{
    private float jetPackDuration;

    public PlayerJetPackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(
        currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        jetPackDuration = Ctx.JetpackDuration;
        Ctx.JetpackAlreadyUsed = true;
        InitializeSubState();
    }

    public override void UpdateState()
    {
        jetPackDuration -= Time.deltaTime;
        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
    }

    public override void CheckSwitchStates()
    {
        if (jetPackDuration <= 0)
        {
            SwitchState(Factory.Fall());
        }

        if (Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Grounded());
        }
    }

    public override void InitializeSubState()
    {
        if (!Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Idle());
        }
        else if (Ctx.IsMovementPressed && !Ctx.IsRunPressed)
        {
            SetSubState(Factory.Walk());
        }
        else
        {
            SetSubState(Factory.Run());
        }
    }

    public void HandleGravity()
    {
        Ctx.CurrentMovementY = Mathf.Min(Ctx.CurrentMovementY + Ctx.JetpackForce * Time.deltaTime, Ctx.MaxJetpackVelocity);
        Ctx.AppliedMovementY = Ctx.CurrentMovementY;
        /*
        float previousYVelocity = Ctx.CurrentMovementY;
        Ctx.CurrentMovementY = Ctx.CurrentMovementY * Time.deltaTime;
        Ctx.AppliedMovementY = Mathf.Max((previousYVelocity + Ctx.CurrentMovementY) * .5f, -20.0f);
        */
        //Ctx.AppliedMovementY = previousYVelocity + Ctx.CurrentMovementY;
    }
}
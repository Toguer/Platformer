using System;
using UnityEngine;

public class PlayerJetPackState : PlayerBaseState, IRootState
{
    private float _jetpackBoostDuration;
    private float _jetpackGlideDuration;

    public PlayerJetPackState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(
        currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        _jetpackBoostDuration = Ctx.JetpackDuration * Ctx.JetpackBoostDuration;
        _jetpackGlideDuration = Ctx.JetpackDuration * Ctx.JetpackGlideDuration;

        //_jetPackDuration = Ctx.JetpackDuration;
        Ctx.JetpackAlreadyUsed = true;

        //Impulso inicial
        Ctx.CurrentMovementY = Ctx.JetpackForce * 1.5f;
        Ctx.AppliedMovementY = Ctx.CurrentMovementY * 1.5f;

        InitializeSubState();
    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
    }

    public override void CheckSwitchStates()
    {
        if (_jetpackBoostDuration <= 0 && _jetpackGlideDuration <= 0)
        {
            SwitchState(Factory.Fall());
        }
        else if (Ctx.CharacterController.isGrounded)
        {
            SwitchState(Factory.Grounded());
        }
        else if (!Ctx.IsJumpPressed)
        {
            SwitchState(Factory.Fall());
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
        else
        {
            SetSubState(Factory.Run());
        }
    }

    public void HandleGravity()
    {
        if (_jetpackBoostDuration > 0)
        {
            float t = Mathf.Clamp01(_jetpackBoostDuration / (Ctx.JetpackDuration * Ctx.JetpackBoostDuration));
            float smoothJetpackForce = Mathf.Lerp(0, Ctx.JetpackForce * 1.5f, t);

            Ctx.CurrentMovementY += smoothJetpackForce * Time.deltaTime;
            Ctx.CurrentMovementY = Mathf.Clamp(Ctx.CurrentMovementY, -Ctx.MinJetpackVelocity, Ctx.MaxJetpackVelocity);
            Ctx.AppliedMovementY = Ctx.CurrentMovementY;

            _jetpackBoostDuration -= Time.deltaTime;
        }
        else if (_jetpackGlideDuration > 0)
        {
            Debug.Log("Jetpack Gliding");

            float floatgravity = -Mathf.Abs(Ctx.Gravity) * Ctx.JetpackGlideForce;

            Ctx.CurrentMovementY += (Ctx.JetpackForce * Ctx.JetpackGlideForce + floatgravity) * Time.deltaTime;

            Ctx.CurrentMovementY = Mathf.Clamp(Ctx.CurrentMovementY, -Ctx.MinJetpackVelocity * Ctx.JetpackGlideForce,
                Ctx.MaxJetpackVelocity * Ctx.JetpackBoostDuration);

            Ctx.AppliedMovementY = Ctx.CurrentMovementY;

            _jetpackGlideDuration -= Time.deltaTime;
        }
        else
        {
            Ctx.CurrentMovementY += Ctx.Gravity * Time.deltaTime;
            Ctx.AppliedMovementY = Mathf.Max(Ctx.CurrentMovementY, -Ctx.MinJetpackVelocity);
        }
    }
}
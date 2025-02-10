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

        //Impulso inicial
        Ctx.CurrentMovementY = Ctx.JetpackForce;
        Ctx.AppliedMovementY = Ctx.JetpackForce;

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
        if (jetPackDuration <= 0)
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
        if (jetPackDuration > 0)
        {
            // Suavizar la aceleración inicial y desacelerar progresivamente
            float t = Mathf.Clamp01(jetPackDuration / Ctx.JetpackDuration); // Normalizar el tiempo restante
            float smoothJetpackForce = Mathf.Lerp(0, Ctx.JetpackForce, t); // Interpolar la fuerza de jetpack

            Ctx.CurrentMovementY += smoothJetpackForce + Time.deltaTime;
            
            Ctx.CurrentMovementY = Mathf.Clamp(Ctx.CurrentMovementY, -Ctx.MinJetpackVelocity, Ctx.MaxJetpackVelocity);

            Ctx.AppliedMovementY = Ctx.CurrentMovementY;
            jetPackDuration -= Time.deltaTime;
        }
        else
        {
            Ctx.CurrentMovementY += Ctx.Gravity * Time.deltaTime;
            Ctx.AppliedMovementY = Mathf.Max(Ctx.CurrentMovementY, -Ctx.MinJetpackVelocity);
        }
    }
}
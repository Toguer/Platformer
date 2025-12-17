using UnityEngine;

public class PlayerFallStateRb : PlayerBaseStateRb, IRootState
{
    public PlayerFallStateRb(RbPlayerStateMachine currentContext, FactoryRigidBody playerStateFactory) : base(
        currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Ctx.ShouldApplyHorizontalMovement = true;
        InitializeSubState();
        CurrentSubState?.EnterState();
    }

    public override void UpdateState()
    {
        HandleGravity();
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Ctx.ShouldApplyHorizontalMovement = false;
    }

    public override void CheckSwitchStates()
    {
        if (Ctx.DashEnabled && Ctx.DashPressed && !Ctx.DashAlreadyUsed)
        {
            SwitchState(Factory.Dash());
        }
        else if (Ctx.IsJumpPressed && Ctx.RemainingCoyoteTime > 0 && !Ctx.IsGrounded && Ctx.Velocity.y < -0.2f)
        {
            Ctx.LastJumpSource = JumpSource.Coyote;
            Ctx.RemainingCoyoteTime = 0;
            Ctx.RemainingJumpBufferTime = 0f;
            Ctx.CanUseCoyote = false;
            Debug.Log("Desde fall hacia Jump, usando el coyoteTime");
            SwitchState(Factory.Jump());
        }
        else if (Ctx.IsGrounded)
        {
            Ctx.RemainingCoyoteTime = 0;
            Ctx.DashAlreadyUsed = false;
            Debug.Log("Grounded from Fall");
            SwitchState(Factory.Grounded());
        }
        else if (Ctx.JetpackEnabled && !Ctx.JetpackAlreadyUsed && Ctx.JetpackDuration > 0 && !Ctx.RequireNewJumpPress)
        {
            if (Ctx.IsGamepad)
            {
                if (Ctx.JetpackTrigger > 0.1f)
                {
                    Ctx.CurrentMovementY = Ctx.JetpackForce;
                    Debug.Log("Jetpack from Fall");
                    SwitchState(Factory.Jetpack());
                }
            }
            else
            {
                if (Ctx.JetpackTrigger > 0.1f)
                {
                    Ctx.CurrentMovementY = Ctx.JetpackForce;
                    Debug.Log("Jetpack from Fall");
                    SwitchState(Factory.Jetpack());
                }
            }
        }
        else if (Ctx.IsInteractPressed)
        {
            RaycastHit _hit;
            if (Ctx.TryGetWallBurrowHit(out _hit))
            {
                Debug.Log("Fall -> WallBurrow");
                SwitchState(Factory.WallBurrow());
            }
        }
    }

    public override void InitializeSubState()
    {
        if (Ctx.IsMovementPressed)
        {
            SetSubState(Factory.AirMove());
        }
    }

    public void HandleGravity()
    {
        Vector3 velocity = Ctx.Velocity;

        if (velocity.y <= 0)
        {
            float newY = velocity.y + Physics.gravity.y * Time.deltaTime;

            //Si no pulsas el botón de saltar caes más rápido
            if (!Ctx.IsJumpPressed && velocity.y < 0 && Ctx.HorizontalSpeed < Ctx.SpeedThreshold)
            {
                newY += Physics.gravity.y * (Ctx.FallMultiplier - 1) * Time.deltaTime;
            }
            else if (!Ctx.IsJumpPressed && velocity.y < 0 && Ctx.HorizontalSpeed > Ctx.SpeedThreshold)
            {
                newY += Physics.gravity.y * (Ctx.FastJumpFallMultiplier - 1) * Time.deltaTime;
            }


            Ctx.Velocity = new Vector3(velocity.x, Mathf.Max(newY, -20f), velocity.z);
        }
    }
}
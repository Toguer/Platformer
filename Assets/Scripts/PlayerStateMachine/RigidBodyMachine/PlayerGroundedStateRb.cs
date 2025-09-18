using UnityEngine;

public class PlayerGroundedStateRb : PlayerBaseStateRb, IRootState
{
    public PlayerGroundedStateRb(RbPlayerStateMachine currentContext, FactoryRigidBody factoryRigidBody) : base(
        currentContext, factoryRigidBody)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Ctx.CanUseCoyote = true;
        Ctx.RemainingCoyoteTime = 0;
        
        //Ctx.AnimatorRef.SetBool("isJump", false);
        InitializeSubState();

        Ctx.JetpackAlreadyUsed = false;
        Ctx.RequireNewJumpPress = false;

        // Cancelar velocidad vertical y frenar horizontalmente
        Vector3 velocity = Ctx.Velocity;
        velocity.y = 0f;

        Ctx.Velocity = velocity;
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
    }

    public override void CheckSwitchStates()
    {
        // Si el jugador esta en el suelo y se pulsa saltar, cambia el estado al PlayerJumpState
        if (Ctx.RemainingJumpBufferTime > 0f && !Ctx.RequireNewJumpPress && Ctx.IsGrounded)
        {
            Ctx.RemainingJumpBufferTime = 0f;
            Ctx.RemainingCoyoteTime = 0f;
            Debug.Log("Grounded -> Jump");
            Ctx.LastJumpSource = JumpSource.Ground;
            SwitchState(Factory.Jump());
        }
        else if (Ctx.DashEnabled && Ctx.DashPressed && !Ctx.DashAlreadyUsed)
        {
            SwitchState(Factory.Dash());
        }
        else if (!Ctx.IsGrounded && Ctx.Velocity.y <= -0.2f && !Ctx.JustJumped && Ctx.CanUseCoyote)
        {
            Debug.Log("Grounded -> Fall");
            Ctx.RemainingCoyoteTime = Ctx.CoyoteTime;
            SwitchState(Factory.Fall());
        }
        else if (Ctx.IsInteractPressed && Ctx.IsNearSand())
        {
            Debug.Log("Entrando en BurrowState");
            SwitchState(Factory.Burrow());
        }
    }

    public override void InitializeSubState()
    {
        bool hasInput = Ctx.IsMovementPressed || Ctx.CurrentMovementInput.sqrMagnitude > 0.0001f;

        Vector2 horizontalVel = new Vector2(Ctx.Velocity.x, Ctx.Velocity.z);
        bool hasMomentum = horizontalVel.sqrMagnitude > 0.0001f;

        bool runKeyboard = Ctx.isRunning && !Ctx.IsGamepad;
        bool runGamepad = Ctx.IsGamepad && Ctx.CurrentMovementInput.magnitude > Ctx.RunMagnitude;
        if (!hasInput || hasMomentum)
        {
            SetSubState(Factory.Idle());
        }
        else if (runKeyboard ||runGamepad)
        {
            SetSubState(Factory.Run());
        }
        else
        {
            SetSubState(Factory.Walk());
        }
    }

    public void HandleGravity()
    {
    }
}
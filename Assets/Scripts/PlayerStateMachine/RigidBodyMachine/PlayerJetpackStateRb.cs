using UnityEngine;

public class PlayerJetpackStateRb : PlayerBaseStateRb, IRootState
{
    private float _timer;
    private float _impulseTime;
    private float _glideTime;

    public PlayerJetpackStateRb(RbPlayerStateMachine ctx, FactoryRigidBody factory) : base(ctx, factory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Ctx.RequireNewJumpPress = true;
        _timer = 0f;
        _impulseTime = Ctx.JetpackBoostDuration * Ctx.JetpackDuration;
        _glideTime = Ctx.JetpackGlideDuration * Ctx.JetpackDuration;

        Ctx.JetpackAlreadyUsed = true;

        Ctx.Velocity = new Vector3(Ctx.Velocity.x, Ctx.JetpackForce, Ctx.Velocity.z);

        if (Ctx.JetpackParticles1 != null) Ctx.JetpackParticles1.Play();
        if (Ctx.JetpackParticles2 != null) Ctx.JetpackParticles2.Play();

        InitializeSubState();
    }

    public override void UpdateState()
    {
        _timer += Time.deltaTime;

        if (_timer <= _impulseTime)
        {
            // Impulso fuerte
            ApplyJetpackForce(Ctx.JetpackForce);
        }
        else if (_timer <= _impulseTime + _glideTime)
        {
            // Planeo
            ApplyJetpackForce(Ctx.JetpackGlideForce);
        }

        Debug.Log(Ctx.Velocity.y);
        CheckSwitchStates();
    }

    public override void ExitState()
    {
        if (Ctx.JetpackParticles1 != null) Ctx.JetpackParticles1.Stop();
        if (Ctx.JetpackParticles2 != null) Ctx.JetpackParticles2.Stop();
        Ctx.RequireNewJumpPress = true;
        Vector3 velocity = Ctx.Velocity;
        velocity.y = 0f;
        Ctx.Velocity = velocity;
    }

    public override void CheckSwitchStates()
    {
        if (_impulseTime <= 0 && _glideTime <= 0)
        {
            Ctx.ContinueUseJetpack = true;
            Ctx.JetpackAlreadyUsed = true;
            Ctx.AppliedMovementY = 0;
            SwitchState(Factory.Fall());
        }
        else if (Ctx.IsGrounded)
        {
            SwitchState(Factory.Grounded());
        }
        else if (!Ctx.IsJumpPressed)
        {
            Ctx.ContinueUseJetpack = false;
            Ctx.AppliedMovementY = 0;
            SwitchState(Factory.Fall());
        }
    }

    public override void InitializeSubState()
    {
        /*
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
        */
    }

    private void ApplyJetpackForce(float force)
    {
        
        Vector3 velocity = Ctx.Velocity;

        // Suma fuerza vertical
        velocity.y += force * Time.deltaTime;

        // Clamp a límites seguros
        velocity.y = Mathf.Clamp(velocity.y, Ctx.MinJetpackVelocity, Ctx.MaxJetpackVelocity);

        Ctx.Velocity = velocity;
        
    }

    public void HandleGravity()
    {
       //No hace falta
    }
}
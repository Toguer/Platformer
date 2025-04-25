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

        // Primer impulso fuerte
        Ctx.Rb.AddForce(Vector3.up * Ctx.JetpackForce, ForceMode.Impulse);

        if (Ctx.JetpackParticles1 != null) Ctx.JetpackParticles1.Play();
        if (Ctx.JetpackParticles2 != null) Ctx.JetpackParticles2.Play();

        InitializeSubState();
    }

    public override void UpdateState()
    {
        _timer += Time.deltaTime;

        if (_timer <= _impulseTime)
        {
            ApplyJetpackForce(Ctx.JetpackForce);
        }
        else if (_timer <= _impulseTime + _glideTime)
        {
            ApplyJetpackForce(Ctx.JetpackGlideForce);
        }

        CheckSwitchStates();
    }

    public override void ExitState()
    {
        if (Ctx.JetpackParticles1) Ctx.JetpackParticles1.Stop();
        if (Ctx.JetpackParticles2) Ctx.JetpackParticles2.Stop();
        Ctx.RequireNewJumpPress = true;

        // 🚨 Limpieza de velocidad vertical al dejar de usar el jetpack
        Vector3 vel = Ctx.Velocity;
        if (Ctx.IsGrounded || vel.y > 0) // evita rebotes al tocar suelo
        {
            vel.y = 0f;
            Ctx.Velocity = vel;
        }
    }

    public override void CheckSwitchStates()
    {
        if (_timer > (_impulseTime + _glideTime))
        {
            SwitchState(Factory.Fall());
        }
        else if (Ctx.IsGrounded)
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
        if (Ctx.IsMovementPressed)
        {
            //SetSubState(Factory.AirWalk());
        }
        else
        {
            //SetSubState(Factory.Idle());
        }
    }

    private void ApplyJetpackForce(float force)
    {
        // Fuerza continua
        Ctx.Rb.AddForce(Vector3.up * force * Time.deltaTime, ForceMode.Force);
    }

    public void HandleGravity()
    {
        // NO hace falta
    }
}
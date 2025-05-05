using System.Collections.Generic;


enum PlayerStatesRb
{
    Idle,
    Walk,
    AirWalk,
    Run,
    Grounded,
    Jump,
    Fall,
    Jetpack,
    Dash,
    Burrow
}

public class FactoryRigidBody
{
    private RbPlayerStateMachine _context;
    private Dictionary<PlayerStatesRb, PlayerBaseStateRb> _states = new Dictionary<PlayerStatesRb, PlayerBaseStateRb>();

    public FactoryRigidBody(RbPlayerStateMachine currentContext)
    {
        _context = currentContext;
        _states[PlayerStatesRb.Idle] = new PlayerIdleStateRb(_context, this);
        _states[PlayerStatesRb.Walk] = new PlayerWalkStateRb(_context, this);
        _states[PlayerStatesRb.Run] = new PlayerRunStateRb(_context, this);
        _states[PlayerStatesRb.AirWalk] = new PlayerRunStateRb(_context, this);
        _states[PlayerStatesRb.Jump] = new PlayerJumpStateRb(_context, this);
        _states[PlayerStatesRb.Grounded] = new PlayerGroundedStateRb(_context, this);
        _states[PlayerStatesRb.Fall] = new PlayerFallStateRb(_context, this);
        _states[PlayerStatesRb.Jetpack] = new PlayerJetpackStateRb(_context, this);
        _states[PlayerStatesRb.Dash] = new PlayerDashStateRb(_context, this);
        //_states[PlayerStatesRb.Burrow] = new PlayerBurrowStateRb(_context, this);
    }

    public PlayerBaseStateRb Idle()
    {
        return _states[PlayerStatesRb.Idle];
    }

    public PlayerBaseStateRb Walk()
    {
        return _states[PlayerStatesRb.Walk];
    }
    public PlayerBaseStateRb AirWalk()
    {
        return _states[PlayerStatesRb.AirWalk];
    }
    public PlayerBaseStateRb Run()
    {
        return _states[PlayerStatesRb.Run];
    }

    public PlayerBaseStateRb Jump()
    {
        return _states[PlayerStatesRb.Jump];
    }

    public PlayerBaseStateRb Grounded()
    {
        return _states[PlayerStatesRb.Grounded];
    }

    public PlayerBaseStateRb Fall()
    {
        return _states[PlayerStatesRb.Fall];
    }

    public PlayerBaseStateRb Jetpack()
    {
        return _states[PlayerStatesRb.Jetpack];
    }

    public PlayerBaseStateRb Dash()
    {
        return _states[PlayerStatesRb.Dash];
    }

    public PlayerBaseStateRb Burrow()
    {
        return _states[PlayerStatesRb.Burrow];
    }
}
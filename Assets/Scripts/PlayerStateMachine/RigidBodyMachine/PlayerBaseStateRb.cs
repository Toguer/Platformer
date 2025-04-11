using UnityEngine;

public abstract class PlayerBaseStateRb
{
    
    private bool _isRootState = false;
    private RbPlayerStateMachine _ctx;
    private FactoryRigidBody _factory;

    private PlayerBaseStateRb _currentSubState;
    private PlayerBaseStateRb _currentSuperState;

    public PlayerBaseStateRb CurrentSubState
    {
        get { return _currentSubState; }
    }

    public PlayerBaseStateRb CurrentSuperState
    {
        get { return _currentSuperState; }
    }

    protected bool IsRootState
    {
        set { _isRootState = value; }
    }

    protected RbPlayerStateMachine Ctx
    {
        get { return _ctx; }
    }

    protected FactoryRigidBody Factory
    {
        get { return _factory; }
    }

    public PlayerBaseStateRb(RbPlayerStateMachine currentContext, FactoryRigidBody playerStateFactory)
    {
        _ctx = currentContext;
        _factory = playerStateFactory;
    }

    public abstract void EnterState();

    public abstract void UpdateState();

    public abstract void ExitState();

    public abstract void CheckSwitchStates();

    public abstract void InitializeSubState();

    public void UpdateStates()
    {
        UpdateState();
        if (_currentSubState != null)
        {
            _currentSubState.UpdateStates();
        }
    }

    public void ExitStates()
    {
        ExitState();
        if (_currentSubState != null)
        {
            _currentSubState.ExitStates();
        }
    }

    protected void SwitchState(PlayerBaseStateRb newState)
    {
        ExitState();

        newState.EnterState();
        if (_isRootState)
        {
            _ctx.CurrentState = newState;
        }
        else if (_currentSuperState != null)
        {
            _currentSuperState.SetSubState(newState);
        }

        //_ctx.CurrentState = newState;
    }

    protected void SetSuperState(PlayerBaseStateRb newSuperState)
    {
        _currentSuperState = newSuperState;
    }

    protected void SetSubState(PlayerBaseStateRb newSubState)
    {
        _currentSubState = newSubState;
        newSubState.SetSuperState(this);
    }
}

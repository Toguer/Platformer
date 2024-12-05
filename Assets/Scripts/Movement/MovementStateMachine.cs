using System;
using UnityEngine;
using UnityEngine.Serialization;

public class MovementStateMachine : MonoBehaviour
{
    [SerializeField] private State _initialState;
    private State _currentState;

    [Header("States")] [SerializeField] private State _idleState;
    [SerializeField] private State _jumpState;
    [SerializeField] private State _walkState;
    [SerializeField] private State _runState;
    [SerializeField] private State _dashState;
    [SerializeField] private State _wallJump;
    [SerializeField] private State _wallRun;

    private GameObject _owner;

    private void Start()
    {
        _owner = this.gameObject;
        ChangeState(_initialState);
    }

    public void ChangeState(State newState)
    {
        if (_currentState != null)
        {
            _currentState.Exit(_owner);
        }

        _currentState = newState;

        if (_currentState != null)
        {
            _currentState.Enter(_owner);
        }
    }

    public void Update()
    {
        _currentState?.Execute(_owner);
    }
}
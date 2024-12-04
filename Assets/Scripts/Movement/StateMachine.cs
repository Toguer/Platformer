using System;
using UnityEngine;

public class StateMachine : MonoBehaviour
{
    [SerializeField] private State initialState;
    private State _currentState;

    private GameObject _owner;

    private void Start()
    {
        _owner = this.gameObject;
        ChangeState(initialState);
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
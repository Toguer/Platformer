using UnityEngine;

public class MovementController : MonoBehaviour
{
    private StateMachine _stateMachine;
    [SerializeField] private IState _idleState;
    [SerializeField] private IState _walkState;
    [SerializeField] private IState _jumpState;
    
    
    void Start()
    {
        _stateMachine = new StateMachine();

        _idleState = new IdleState();
        _walkState = new WalkState();
        _jumpState = new JumpState();
        
        _stateMachine.ChangeState(_idleState);
    }

    // Update is called once per frame
    void Update()
    {
        _stateMachine.Update();
    }
}

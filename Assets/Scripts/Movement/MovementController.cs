using UnityEngine;

public class MovementController : MonoBehaviour
{
    private StateMachine _stateMachine;
    
    void Start()
    {
        _stateMachine.GetComponent<StateMachine>();    
        //_stateMachine.ChangeState(_stateMachine);
    }

    // Update is called once per frame
    void Update()
    {
        _stateMachine.Update();
    }
}

using UnityEngine;

[CreateAssetMenu(menuName = "States/IdleState")]
public class IdleState : State
{
    public override void UpdateState()
    {
    }
    public override void OnEnterState()
    {
        characterGame = stateMachine.gameObject;
    }

    public override void OnExitState()
    {
    }
}

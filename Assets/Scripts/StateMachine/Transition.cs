using UnityEngine;

[System.Serializable]
public class Transition
{
    [SerializeField] Decision decisionToBeMade;
    [SerializeField] State onDecisionTrueExitState;
    [SerializeField] State onDecisionFalseExitState;
    [SerializeField] float exitTime = -1;
    public State GetExitState(StateMachine stateMachine)
    {
        return decisionToBeMade.Decide(stateMachine) ? onDecisionTrueExitState : onDecisionFalseExitState;
    }

    public float GetExitTime()
    {
        return exitTime;
    }
}

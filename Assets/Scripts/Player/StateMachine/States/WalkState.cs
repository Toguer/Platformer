using UnityEngine;
[CreateAssetMenu(menuName = "States/WalkState")]
public class WalkState : State
{
    public override void OnEnterState()
    {
        characterGame = stateMachine.gameObject;
        //cambio de animacion
    }

    public override void OnExitState()
    {
        throw new System.NotImplementedException();
    }

    public override void UpdateState()
    {
        if (characterGame.GetComponent<PlayerInput>().GetMove() != Vector3.zero)
        {
            characterGame.transform.rotation = Quaternion.Slerp(characterGame.transform.rotation, Quaternion.LookRotation(characterGame.GetComponent<PlayerInput>().GetMove()), 0.15f);
        }
        characterGame.GetComponent<MovementBehaviour>().MoveIsometric(characterGame.GetComponent<PlayerInput>().GetMove());
    }
}

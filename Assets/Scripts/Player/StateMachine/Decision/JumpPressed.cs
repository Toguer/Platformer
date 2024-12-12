using UnityEngine;

[CreateAssetMenu(fileName = "JumpPressed", menuName = "Decisions/JumpPressed")]
public class JumpPressed : Decision
{
    public override bool Decide(StateMachine stateMachine)
    {
        PlayerInput playerInput = stateMachine.gameObject.GetComponent<PlayerInput>();

        if (playerInput.GetJump())
        {
            return true;
        }

        return false;
    }
}

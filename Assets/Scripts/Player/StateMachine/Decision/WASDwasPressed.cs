using UnityEngine;

[CreateAssetMenu(menuName = "Decisions/WASDwasPressed")]
public class WASDwasPressed : Decision
{
    public override bool Decide(StateMachine stateMachine)
    {
        PlayerInput playerInput = stateMachine.gameObject.GetComponent<PlayerInput>();
        Vector3 keyboardInput = playerInput.GetMove();
        if (keyboardInput != Vector3.zero)
        {
            return true;
        }
        return false;
    }
}

using UnityEngine;

[CreateAssetMenu(menuName = "States/JumpState")]
public class JumpState : State
{
    [SerializeField] private float _jumpForce;
    private Rigidbody _rb;
    public override void OnEnterState()
    {
        characterGame = stateMachine.gameObject;
        if (characterGame.TryGetComponent(out _rb))
        {
            _rb.AddForce(new Vector3(_rb.linearVelocity.x, _jumpForce, _rb.linearVelocity.z), ForceMode.Impulse);
        }
    }

    public override void UpdateState()
    {
        throw new System.NotImplementedException();
    }

    public override void OnExitState()
    {
        throw new System.NotImplementedException();
    }
}

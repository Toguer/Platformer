using UnityEngine;

public class PlayerBurrowState : PlayerBaseState, IRootState
{
    private Vector3 _burrowDirection;

    public PlayerBurrowState(PlayerStateMachine currentContext, PlayerStateFactory playerStateFactory) : base(
        currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Ctx.gameObject.layer = LayerMask.NameToLayer("Burrowing");

        Ctx.CharacterController.detectCollisions = false;
        Ctx.AppliedMovementY = 0;
        Debug.Log("Entrando en estado de excavar");
    }

    public override void UpdateState()
    {
        _burrowDirection = new Vector3(Ctx.CurrentMovementInput.x, 0, Ctx.CurrentMovementInput.y);
        Ctx.AppliedMovementX = _burrowDirection.x * Ctx.BurrowSpeed;
        Ctx.AppliedMovementZ = _burrowDirection.z * Ctx.BurrowSpeed;

        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Debug.Log("Saliendo en estado de excavar");

        Ctx.gameObject.layer = LayerMask.NameToLayer("Player");

        Ctx.CharacterController.detectCollisions = true;
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsEarthPressed && Ctx.CharacterController.isGrounded)
        {
            ExitState();
            SwitchState(Factory.Grounded());
        }
        else if (!Ctx.IsEarthPressed && !Ctx.CharacterController.isGrounded)
        {
            Debug.Log("Cayendo fuera de la arena");
            SwitchState(Factory.Fall());
        }
        else if (Ctx.IsJumpPressed)
        {
            Debug.Log("Saltando fuera de la arena");
            SwitchState(Factory.Jump());
        }
        // Dash dentro de la arena
        else if (Ctx.DashPressed && !Ctx.DashAlreadyUsed)
        {
            Debug.Log("Dashando dentro de la arena");
            SwitchState(Factory.Dash());
        }
    }

    public override void InitializeSubState()
    {
        throw new System.NotImplementedException();
    }

    public void HandleGravity()
    {
    }
}
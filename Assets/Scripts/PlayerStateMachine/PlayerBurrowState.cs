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
        Vector3 forwardDirection = Ctx.Interactable.transform.forward;
        Debug.Log("EarthWall forward direction: " + Ctx.Interactable.transform.forward);
        //forwardDirection.y = 0; // Evita que el personaje se incline hacia arriba o abajo


        //_burrowDirection = new Vector3(Ctx.CurrentMovementInput.x, 0, Ctx.CurrentMovementInput.y);

        Ctx.AppliedMovementX = forwardDirection.x * Ctx.BurrowSpeed;
        Ctx.AppliedMovementZ = forwardDirection.z * Ctx.BurrowSpeed;

        CheckSwitchStates();
    }


    public override void ExitState()
    {
        Debug.Log("Saliendo en estado de excavar");

        Ctx.gameObject.layer = LayerMask.NameToLayer("Player");
        Ctx.AppliedMovementX = 0;
        Ctx.AppliedMovementZ = 0;
        Ctx.CharacterController.detectCollisions = true;
    }

    public override void CheckSwitchStates()
    {
        if (!Ctx.IsNearSand())
        {
            if (Ctx.CharacterController.isGrounded)
            {
                ExitState();
                Debug.Log("Saliendo de Burrow - Regresando a Grounded");
                SwitchState(Factory.Grounded());
            }
            else
            {
                ExitState();
                Debug.Log("Saliendo de Burrow - Cayendo");
                SwitchState(Factory.Fall());
            }
        }
        else if (Ctx.DashPressed && !Ctx.DashAlreadyUsed)
        {
            ExitState();
            Debug.Log("Dashendo dentro de la arena");
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
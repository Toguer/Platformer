using UnityEngine;

public class WallBurrowStateRb : PlayerBaseStateRb, IRootState
{
    private Vector3 _moveDirection;
    private RaycastHit _currentWallHit;

    private Collider _playerCollider;
    private Collider _currentWallCollider;

    public WallBurrowStateRb(RbPlayerStateMachine currentContext, FactoryRigidBody playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Debug.Log("Enter WallBurrow");

        // Buscamos pared mirando hacia delante (para entrar al modo)
        if (!Ctx.TryGetWallBurrowHit(out _currentWallHit))
        {
            if (Ctx.IsGrounded)
            {
                SwitchState(Factory.Grounded());
            }
            else
            {
                SwitchState(Factory.Fall());
            }

            return;
        }

        Ctx.WallBurrowNormal = _currentWallHit.normal;

        // Desactivamos gravedad mientras estamos excavando en pared
        Ctx.Rb.useGravity = false;

        // colliders e ignorar colisión con esta pared ---
        if (_playerCollider == null)
        {
            _playerCollider = Ctx.GetComponent<Collider>();
        }

        _currentWallCollider = _currentWallHit.collider;


        if (_playerCollider != null && _currentWallCollider != null)
        {
            Physics.IgnoreCollision(_playerCollider, _currentWallCollider, true);
        }

        // Dirección inicial: forward proyectado en el plano de la pared
        Vector3 _forwardOnWall = Vector3.ProjectOnPlane(Ctx.transform.forward, Ctx.WallBurrowNormal);

        if (_forwardOnWall.sqrMagnitude < 0.0001f)
        {
            _forwardOnWall = Vector3.Cross(Ctx.WallBurrowNormal, Vector3.up);
            if (_forwardOnWall.sqrMagnitude < 0.0001f)
            {
                _forwardOnWall = Vector3.Cross(Ctx.WallBurrowNormal, Vector3.right);
            }
        }

        _moveDirection = _forwardOnWall.normalized;

        // El estado de pared gestiona completamente la velocidad
        Ctx.ShouldApplyHorizontalMovement = false;

        // Colocamos al jugador "dentro" de la pared en el punto de impacto
        Vector3 _insidePos = _currentWallHit.point - Ctx.WallBurrowNormal * Ctx.WallBurrowInsetDepth;
        Ctx.transform.position = _insidePos;

        // Velocidad inicial: moverse automáticamente en la pared
        Ctx.Velocity = _moveDirection * Ctx.WallBurrowSpeed;

        // Mirar en la dirección de movimiento
        Quaternion _targetRotation = Quaternion.LookRotation(_moveDirection, Vector3.up);
        Ctx.transform.rotation = _targetRotation;
    }

    public override void UpdateState()
    {
        // 1) Desde la posición actual (dentro de la pared), buscamos otra vez la superficie
        //    Raycast a lo largo de la normal de la pared, hacia fuera
        Vector3 _origin = Ctx.transform.position - Ctx.WallBurrowNormal * (Ctx.WallBurrowInsetDepth * 0.5f);
        float _maxDistance = Ctx.WallBurrowDetectionDistance + Ctx.WallBurrowInsetDepth;

        if (!Physics.Raycast(
                _origin,
                Ctx.WallBurrowNormal,
                out _currentWallHit,
                _maxDistance,
                Ctx.WallBurrowLayerMask,
                QueryTriggerInteraction.Ignore))
        {
            // Si ya no hay pared en esa dirección, salimos del modo
            //ExitToGroundOrFall();
            //return;
        }

        Ctx.WallBurrowNormal = _currentWallHit.normal;

        if (_playerCollider != null)
        {
            if (_currentWallHit.collider != _currentWallCollider)
            {
                // Dejar de ignorar la pared anterior
                if (_currentWallCollider != null)
                {
                    if (_currentWallHit.collider.GetType() == typeof(MeshCollider))
                    {
                        _currentWallHit.collider.gameObject.GetComponent<MeshCollider>().convex = false;
                        _currentWallHit.collider.gameObject.GetComponent<MeshCollider>().isTrigger = false;
                    }
                    else
                    {
                        _currentWallHit.collider.isTrigger = false;
                    }

                    //Physics.IgnoreCollision(_playerCollider, _currentWallCollider, false);
                }

                _currentWallCollider = _currentWallHit.collider;

                // Ignorar la nueva pared
                if (_currentWallCollider != null)
                {
                    if (_currentWallHit.collider.GetType() == typeof(MeshCollider))
                    {
                        _currentWallHit.collider.gameObject.GetComponent<MeshCollider>().convex = true;
                        _currentWallHit.collider.gameObject.GetComponent<MeshCollider>().isTrigger = true;
                    }
                    else
                    {
                        _currentWallHit.collider.isTrigger = true;
                    }
                    
                    //Physics.IgnoreCollision(_playerCollider, _currentWallCollider, true);
                }
            }
        }

        // 2) Girar sobre la pared según el input horizontal (X)
        float _horizontalInput = Ctx.CurrentMovementInput.x;
        if (Mathf.Abs(_horizontalInput) > 0.01f)
        {
            float _rotationAmount = _horizontalInput * Ctx.WallBurrowTurnSpeed * Time.deltaTime;
            Quaternion _turnRotation = Quaternion.AngleAxis(_rotationAmount, Ctx.WallBurrowNormal);
            _moveDirection = _turnRotation * _moveDirection;

            // Seguridad: reproyección en el plano de la pared
            _moveDirection = Vector3.ProjectOnPlane(_moveDirection, Ctx.WallBurrowNormal).normalized;
        }

        // 3) Movimiento automático constante (no puede parar)
        Vector3 _desiredVelocity = _moveDirection * Ctx.WallBurrowSpeed;
        Ctx.Velocity = _desiredVelocity;

        // 4) SOLO rotación visual, sin tocar posición
        Quaternion _desiredRotation = Quaternion.LookRotation(_moveDirection, Vector3.up);
        Ctx.transform.rotation = Quaternion.Slerp(
            Ctx.transform.rotation,
            _desiredRotation,
            Ctx.WallBurrowRotationLerp * Time.deltaTime
        );

        CheckSwitchStates();
    }

    public override void ExitState()
    {
        Debug.Log("Exit WallBurrow");
        Ctx.WallBurrowNormal = Vector3.zero;
        Ctx.ShouldApplyHorizontalMovement = true;
        Ctx.DashFromWallBurrow = false;

        // Volvemos a activar la gravedad al salir
        Ctx.Rb.useGravity = true;

        // --- NUEVO: reactivar colisión con la pared actual ---
        if (_playerCollider != null && _currentWallCollider != null)
        {
            Physics.IgnoreCollision(_playerCollider, _currentWallCollider, false);
        }

        _currentWallCollider = null;
    }

    public override void CheckSwitchStates()
    {
        // Salir al soltar interactuar
        if (!Ctx.IsInteractPressed)
        {
            ExitToGroundOrFall();
            return;
        }

        // Dash desde la pared en la dirección actual (incluye Y si estás sobre una pared inclinada)
        if (Ctx.DashEnabled && Ctx.DashPressed && !Ctx.DashAlreadyUsed)
        {
            Ctx.DashFromWallBurrow = true;
            Debug.Log("WallBurrow -> Dash");
            SwitchState(Factory.Dash());
            return;
        }
    }

    public override void InitializeSubState()
    {
        // Sin subestados de momento
    }

    private void ExitToGroundOrFall()
    {
        if (Ctx.IsGrounded)
        {
            SwitchState(Factory.Grounded());
        }
        else
        {
            SwitchState(Factory.Fall());
        }
    }

    public void HandleGravity()
    {
        throw new System.NotImplementedException();
    }
}
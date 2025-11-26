using UnityEngine;

public class WallBurrowStateRb : PlayerBaseStateRb, IRootState
{
    private Vector3 _moveDirection;
    private Vector3 _stableWallNormal;
    private Vector3 _lastValidPosition;
    private bool _hasValidWall;
    
    // Para el sistema de colisiones
    private Collider _playerCollider;

    public WallBurrowStateRb(RbPlayerStateMachine currentContext, FactoryRigidBody playerStateFactory)
        : base(currentContext, playerStateFactory)
    {
        IsRootState = true;
    }

    public override void EnterState()
    {
        Debug.Log("=== ENTER WallBurrow ===");

        // Buscar pared inicial
        RaycastHit initialHit;
        if (!Ctx.TryGetWallBurrowHit(out initialHit))
        {
            Debug.LogWarning("No se detectó pared al entrar a WallBurrow");
            ExitToGroundOrFall();
            return;
        }

        _stableWallNormal = initialHit.normal;
        _hasValidWall = true;

        // CLAVE: Convertir a kinematic para control total
        Ctx.Rb.isKinematic = true;
        Ctx.Rb.useGravity = false;
        Ctx.ShouldApplyHorizontalMovement = false;

        // Obtener el collider del jugador
        if (_playerCollider == null)
        {
            _playerCollider = Ctx.GetComponent<Collider>();
        }

        // DESACTIVAR el collider completamente para evitar físicas
        if (_playerCollider != null)
        {
            _playerCollider.enabled = false;
        }

        // Calcular dirección inicial proyectada en el plano de la pared
        CalculateInitialDirection();

        // Posicionar al jugador dentro de la pared
        Vector3 insidePosition = initialHit.point - _stableWallNormal * Ctx.WallBurrowInsetDepth;
        Ctx.transform.position = insidePosition;
        _lastValidPosition = insidePosition;

        // Orientar al jugador
        UpdatePlayerRotation();

        Debug.Log($"WallBurrow iniciado | Normal: {_stableWallNormal} | Dir: {_moveDirection} | Pos: {insidePosition}");
    }

    private void CalculateInitialDirection()
    {
        // Proyectar el forward actual en el plano de la pared
        Vector3 projectedForward = Vector3.ProjectOnPlane(Ctx.transform.forward, _stableWallNormal);

        if (projectedForward.sqrMagnitude < 0.0001f)
        {
            // Si el forward es perpendicular a la pared, usar un vector lateral
            projectedForward = Vector3.Cross(_stableWallNormal, Vector3.up);
            
            if (projectedForward.sqrMagnitude < 0.0001f)
            {
                projectedForward = Vector3.Cross(_stableWallNormal, Vector3.right);
            }
        }

        _moveDirection = projectedForward.normalized;
    }

    public override void UpdateState()
    {
        // 1. Verificar si seguimos en contacto con la pared
        UpdateWallDetection();

        if (!_hasValidWall)
        {
            Debug.Log("Perdido contacto con la pared");
            ExitToGroundOrFall();
            return;
        }

        // 2. Procesar input de rotación
        ProcessTurningInput();

        // 3. Aplicar movimiento manual (kinematic)
        ApplyWallMovement();

        // 4. Actualizar rotación visual
        UpdatePlayerRotation();

        // 5. Comprobar transiciones de estado
        CheckSwitchStates();
    }

    private void UpdateWallDetection()
    {
        // Múltiples raycasts para detectar la pared desde nuestra posición actual
        Vector3 rayOrigin = Ctx.transform.position;
        
        // Ray principal: hacia fuera
        RaycastHit hit;
        float maxDist = Ctx.WallBurrowDetectionDistance + (Ctx.WallBurrowInsetDepth * 2f);
        bool hitDetected = Physics.Raycast(
            rayOrigin, 
            _stableWallNormal, 
            out hit, 
            maxDist, 
            Ctx.WallBurrowLayerMask, 
            QueryTriggerInteraction.Ignore
        );

        // Ray secundario: hacia dentro (en caso de estar muy afuera)
        if (!hitDetected)
        {
            hitDetected = Physics.Raycast(
                rayOrigin, 
                -_stableWallNormal, 
                out hit, 
                maxDist, 
                Ctx.WallBurrowLayerMask, 
                QueryTriggerInteraction.Ignore
            );
            
            if (hitDetected)
            {
                hit.normal = -hit.normal;
            }
        }

        // Ray adicional: desde arriba del jugador
        if (!hitDetected)
        {
            Vector3 topOrigin = rayOrigin + Vector3.up * 0.5f;
            hitDetected = Physics.Raycast(
                topOrigin, 
                _stableWallNormal, 
                out hit, 
                maxDist, 
                Ctx.WallBurrowLayerMask, 
                QueryTriggerInteraction.Ignore
            );
        }

        if (hitDetected)
        {
            _hasValidWall = true;
            
            // Actualizar normal suavemente
            float normalBlend = Time.deltaTime * 3f;
            _stableWallNormal = Vector3.Slerp(_stableWallNormal, hit.normal, normalBlend);
            
            // Reproyectar la dirección de movimiento en el nuevo plano
            _moveDirection = Vector3.ProjectOnPlane(_moveDirection, _stableWallNormal).normalized;

            // Mantener distancia de la pared
            Vector3 targetPosition = hit.point - _stableWallNormal * Ctx.WallBurrowInsetDepth;
            _lastValidPosition = targetPosition;
            Ctx.transform.position = targetPosition;
        }
        else
        {
            _hasValidWall = false;
        }

        // Debug visual
        Debug.DrawRay(rayOrigin, _stableWallNormal * maxDist, _hasValidWall ? Color.green : Color.red);
        Debug.DrawRay(rayOrigin, -_stableWallNormal * maxDist, _hasValidWall ? Color.cyan : Color.magenta);
        Debug.DrawRay(rayOrigin, _moveDirection * 2f, Color.yellow);
    }

    private void ProcessTurningInput()
    {
        float horizontalInput = Ctx.CurrentMovementInput.x;

        if (Mathf.Abs(horizontalInput) > 0.01f)
        {
            // Rotar la dirección de movimiento alrededor de la normal de la pared
            float rotationAmount = horizontalInput * Ctx.WallBurrowTurnSpeed * Time.deltaTime;
            Quaternion rotation = Quaternion.AngleAxis(rotationAmount, _stableWallNormal);
            
            _moveDirection = rotation * _moveDirection;
            
            // Asegurar que la dirección permanece en el plano de la pared
            _moveDirection = Vector3.ProjectOnPlane(_moveDirection, _stableWallNormal).normalized;
        }
    }

    private void ApplyWallMovement()
    {
        // Movimiento manual ya que somos kinematic
        Vector3 movement = _moveDirection * Ctx.WallBurrowSpeed * Time.deltaTime;
        Ctx.transform.position += movement;
    }

    private void UpdatePlayerRotation()
    {
        // Rotar para mirar en la dirección de movimiento
        // El "up" del jugador apunta hacia fuera de la pared
        Vector3 up = -_stableWallNormal;
        Quaternion targetRotation = Quaternion.LookRotation(_moveDirection, up);
        
        Ctx.transform.rotation = Quaternion.Slerp(
            Ctx.transform.rotation,
            targetRotation,
            Ctx.WallBurrowRotationLerp * Time.deltaTime
        );
    }

    public override void ExitState()
    {
        Debug.Log("=== EXIT WallBurrow ===");
        
        // Restaurar el Rigidbody a modo normal
        Ctx.Rb.isKinematic = false;
        Ctx.Rb.useGravity = true;

        // Reactivar el collider
        if (_playerCollider != null)
        {
            _playerCollider.enabled = true;
        }

        // Limpiar flags
        _hasValidWall = false;
        Ctx.WallBurrowNormal = Vector3.zero;
        Ctx.ShouldApplyHorizontalMovement = true;

        // Calcular velocidad de salida basada en la dirección de movimiento
        Vector3 exitVelocity = _moveDirection * Ctx.WallBurrowSpeed;
        
        // Añadir componente hacia fuera de la pared
        exitVelocity += _stableWallNormal * 3f;
        
        // Si hacemos dash desde aquí, no modificar la velocidad
        if (!Ctx.DashFromWallBurrow)
        {
            Ctx.Velocity = exitVelocity;
        }
        else
        {
            Ctx.DashFromWallBurrow = false;
        }

        Debug.Log($"Velocidad de salida: {exitVelocity}");
    }

    public override void CheckSwitchStates()
    {
        // verificar si perdemos contacto con la pared
        if (!_hasValidWall)
        {
            Debug.Log("WallBurrow -> Saliendo por pérdida de contacto");
            ExitToGroundOrFall();
            return;
        }

        // Salir al soltar el botón de interactuar
        if (!Ctx.IsInteractPressed)
        {
            Debug.Log("WallBurrow -> Saliendo por soltar botón");
            ExitToGroundOrFall();
            return;
        }

        // Dash desde la pared
        if (Ctx.DashEnabled && Ctx.DashPressed && !Ctx.DashAlreadyUsed)
        {
            Ctx.DashFromWallBurrow = true;
            Debug.Log("WallBurrow -> Dash en dirección: " + _moveDirection);
            SwitchState(Factory.Dash());
            return;
        }
    }

    public override void InitializeSubState()
    {
        // No necesita subestados
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
        // No aplicar gravedad en este estado
    }
}
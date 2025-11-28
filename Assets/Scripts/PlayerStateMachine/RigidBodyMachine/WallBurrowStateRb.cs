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

        // CORREGIDO: Posicionar DENTRO de la pared correctamente
        // La normal apunta HACIA FUERA del collider, así que restamos para ir hacia dentro
        Vector3 insidePosition = initialHit.point + _stableWallNormal * Ctx.WallBurrowInsetDepth;
        Ctx.transform.position = insidePosition;
        _lastValidPosition = insidePosition;

        // Orientar al jugador
        UpdatePlayerRotation();

        Debug.Log($"WallBurrow iniciado | HitPoint: {initialHit.point} | Normal: {_stableWallNormal} | Dir: {_moveDirection} | InsidePos: {insidePosition}");
        Debug.Log($"Distancia desde hit point: {Vector3.Distance(initialHit.point, insidePosition):F3}");
    }

    private void CalculateInitialDirection()
    {
        // Obtener la velocidad actual del jugador antes de entrar
        Vector3 currentVelocity = Ctx.Velocity;
        
        // Si hay velocidad horizontal significativa, usarla
        if (currentVelocity.sqrMagnitude > 0.1f)
        {
            // Proyectar la velocidad en el plano de la pared
            Vector3 projectedVelocity = Vector3.ProjectOnPlane(currentVelocity, _stableWallNormal);
            
            if (projectedVelocity.sqrMagnitude > 0.01f)
            {
                _moveDirection = projectedVelocity.normalized;
                Debug.Log($"Dirección desde velocidad: {_moveDirection}");
                return;
            }
        }
        
        // Si no hay velocidad, usar el "right" del jugador (perpendicular a forward)
        // Esto hace que vaya de lado en lugar de hacia la pared
        Vector3 lateralDirection = Ctx.transform.right;
        Vector3 projectedLateral = Vector3.ProjectOnPlane(lateralDirection, _stableWallNormal);
        
        if (projectedLateral.sqrMagnitude > 0.01f)
        {
            _moveDirection = projectedLateral.normalized;
            Debug.Log($"Dirección lateral: {_moveDirection}");
            return;
        }
        
        // Fallback: usar cross product con Vector3.up
        Vector3 fallbackDirection = Vector3.Cross(_stableWallNormal, Vector3.up);
        
        if (fallbackDirection.sqrMagnitude < 0.0001f)
        {
            fallbackDirection = Vector3.Cross(_stableWallNormal, Vector3.right);
        }
        
        _moveDirection = fallbackDirection.normalized;
        Debug.Log($"Dirección fallback: {_moveDirection}");
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
        // Hacer múltiples raycasts desde diferentes puntos para asegurar detección
        Vector3 center = Ctx.transform.position;
        Vector3 top = center + Vector3.up * 0.5f;
        Vector3 bottom = center - Vector3.up * 0.3f;
        
        float maxDist = Ctx.WallBurrowInsetDepth * 4f;
        
        RaycastHit hit;
        bool hitDetected = false;
        
        // CORREGIDO: Buscar hacia DENTRO del collider (dirección opuesta a la normal)
        // La normal apunta FUERA, así que buscamos en -normal
        if (Physics.Raycast(center, -_stableWallNormal, out hit, maxDist, 
            Ctx.WallBurrowLayerMask, QueryTriggerInteraction.Ignore))
        {
            hitDetected = true;
            Debug.DrawRay(center, -_stableWallNormal * hit.distance, Color.green);
        }
        // También buscar hacia fuera por si acaso
        else if (Physics.Raycast(center, _stableWallNormal, out hit, maxDist, 
            Ctx.WallBurrowLayerMask, QueryTriggerInteraction.Ignore))
        {
            hitDetected = true;
            hit.normal = -hit.normal;
            Debug.DrawRay(center, _stableWallNormal * hit.distance, Color.cyan);
        }
        // Intentar desde arriba hacia dentro
        else if (Physics.Raycast(top, -_stableWallNormal, out hit, maxDist, 
            Ctx.WallBurrowLayerMask, QueryTriggerInteraction.Ignore))
        {
            hitDetected = true;
            Debug.DrawRay(top, -_stableWallNormal * hit.distance, Color.green);
        }
        // Intentar desde arriba hacia fuera
        else if (Physics.Raycast(top, _stableWallNormal, out hit, maxDist, 
            Ctx.WallBurrowLayerMask, QueryTriggerInteraction.Ignore))
        {
            hitDetected = true;
            hit.normal = -hit.normal;
            Debug.DrawRay(top, _stableWallNormal * hit.distance, Color.cyan);
        }
        // Intentar desde abajo hacia dentro
        else if (Physics.Raycast(bottom, -_stableWallNormal, out hit, maxDist, 
            Ctx.WallBurrowLayerMask, QueryTriggerInteraction.Ignore))
        {
            hitDetected = true;
            Debug.DrawRay(bottom, -_stableWallNormal * hit.distance, Color.green);
        }
        // Intentar desde abajo hacia fuera
        else if (Physics.Raycast(bottom, _stableWallNormal, out hit, maxDist, 
            Ctx.WallBurrowLayerMask, QueryTriggerInteraction.Ignore))
        {
            hitDetected = true;
            hit.normal = -hit.normal;
            Debug.DrawRay(bottom, _stableWallNormal * hit.distance, Color.cyan);
        }
        // SphereCast como último recurso
        else if (Physics.SphereCast(center, 0.3f, -_stableWallNormal, out hit, maxDist, 
            Ctx.WallBurrowLayerMask, QueryTriggerInteraction.Ignore))
        {
            hitDetected = true;
            Debug.DrawRay(center, -_stableWallNormal * hit.distance, Color.yellow);
        }

        if (hitDetected)
        {
            _hasValidWall = true;
            
            // Actualizar normal MUY suavemente
            _stableWallNormal = Vector3.Slerp(_stableWallNormal, hit.normal, Time.deltaTime * 2f);
            
            // Reproyectar la dirección de movimiento SOLO si cambió mucho la normal
            float normalChange = Vector3.Angle(_moveDirection, Vector3.ProjectOnPlane(_moveDirection, _stableWallNormal));
            if (normalChange > 5f)
            {
                _moveDirection = Vector3.ProjectOnPlane(_moveDirection, _stableWallNormal).normalized;
            }

            // CORREGIDO: Ajustar posición hacia DENTRO del collider
            // Queremos estar a WallBurrowInsetDepth dentro de la superficie
            Vector3 targetPosition = hit.point + _stableWallNormal * Ctx.WallBurrowInsetDepth;
            float currentDist = Vector3.Distance(Ctx.transform.position, hit.point);
            float idealDist = Ctx.WallBurrowInsetDepth;
            float distanceError = Mathf.Abs(currentDist - idealDist);
            
            if (distanceError > 0.15f)
            {
                Ctx.transform.position = Vector3.Lerp(Ctx.transform.position, targetPosition, Time.deltaTime * 3f);
            }
            
            _lastValidPosition = Ctx.transform.position;
            
            // Debug info
            Debug.Log($"Wall detected | HitPoint: {hit.point} | MyPos: {Ctx.transform.position} | Dist: {currentDist:F3} | Error: {distanceError:F3}");
        }
        else
        {
            _hasValidWall = false;
            Debug.LogWarning($"No se detecta pared | Pos: {center} | Normal: {_stableWallNormal}");
            
            // Rayos de debug cuando no detecta
            Debug.DrawRay(center, _stableWallNormal * maxDist, Color.red);
            Debug.DrawRay(center, -_stableWallNormal * maxDist, Color.red);
        }

        // Debug visual de la dirección de movimiento
        Debug.DrawRay(center, _moveDirection * 2f, Color.yellow);
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
        // SIEMPRE aplicar movimiento automático en la dirección actual
        Vector3 movement = _moveDirection * Ctx.WallBurrowSpeed * Time.deltaTime;
        
        // Si hay input vertical (W/S), modificar la velocidad
        float verticalInput = Ctx.CurrentMovementInput.y;
        if (Mathf.Abs(verticalInput) > 0.01f)
        {
            // Ajustar velocidad según input: W acelera, S frena/reversa
            float speedMultiplier = 1f + verticalInput; // W = +1, S = -1
            speedMultiplier = Mathf.Clamp(speedMultiplier, 0.5f, 2f); // Entre 50% y 200%
            movement *= speedMultiplier;
        }
        
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
        
        // CORREGIDO: Añadir componente FUERA de la pared (dirección opuesta a la normal)
        exitVelocity -= _stableWallNormal * 3f;
        
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
        // PRIMERO: verificar si perdemos contacto con la pared
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
using System;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.Serialization;

public enum JumpSource
{
    Ground,
    Coyote,
    Buffer,
    Unknown
}

public class RbPlayerStateMachine : MonoBehaviour
{
    #region variables

    [Header("States")] [SerializeField] private string _actualState;

    [ReadOnly] [SerializeField] private string _actualSubState;
    [ReadOnly] [SerializeField] private string _actualSubSubState;

    [Header("Desactivables")] [SerializeField]
    private bool _snapToGround = false;

    [SerializeField] private bool _dashEnabled = true;
    [SerializeField] private bool _jetpackEnabled = true;

    private Rigidbody _rb;
    private InputSystem_Actions _playerInput;


    [Header("GroundChecker")] [SerializeField]
    private LayerMask _groundMask;

    [ReadOnly] [SerializeField] private bool _isGrounded;
    private bool _groundedByCollision = false;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundDistance = 0.4f;


    private Animator _animator;

    //Variables para inputs del jugador
    private Vector2 _currentMovementInput;
    private Vector3 _currentMovement;
    private Vector3 _currentRunMovement;
    private Vector3 _cameraRelativeMovement;

    [ReadOnly] [SerializeField] private bool _isMovementPressed;
    private bool _isRunning;
    [Range(0f, 1f)] [SerializeField] private float _runMagnitude;


    private bool _isGamepad;

    [Tooltip("Es la velocidad a la que el personaje rota para adaptarse a la dirección en la que camina.")]
    [SerializeField]
    private float _rotationFactorPerFrame = 15.0f;

    [Header("Jump Variables")] [Tooltip("Altura maxima de salto")] [SerializeField]
    private float _maxJumpHeight = 4.0f;

    [Tooltip("Duración maxima del salto")] [SerializeField]
    private float _maxJumpTime = 0.75f;

    [Tooltip("Duración minima del salto")] [SerializeField]
    private float _minJumpTime = 0.1f;

    [Tooltip("Velocidad adicional a la que el jugador caerá cuando no pulsa saltar.")] [SerializeField]
    private float _fallMultiplier = 2.0f;

    [Tooltip("Es el margen de error que tiene un jugador para saltar despues de caer de una esquina.")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float _coyoteTime = 0.1f;

    private bool _canUseCoyote = true;

    [Tooltip("El tiempo que el input de salto se guarda")] [SerializeField]
    private float _jumpBufferTime = 0.2f;

    private float _remainingJumpBufferTime = 0f;

    private float _remainingCoyoteTime;
    private bool _justJumped;

    [Header("Corner Correction")] [SerializeField]
    private float _correctionDistance;

    [SerializeField] private float _correctionRayLenght;

    [Header("Jetpack")] private float _jetpackTrigger;

    [Tooltip("La duración del efecto jetpack")] [SerializeField] [Range(0f, 10.0f)]
    private float _jetpackDuration;

    [Tooltip("La fuerza que tiene el Jetpack, cuanto más alta sea más alto llegará")] [SerializeField]
    private float _jetpackForce = 0.2f;

    [Tooltip("La fuerza que tiene el Jetpack, cuanto más alta sea más alto llegará")]
    [SerializeField]
    [Range(0.05f, 0.5f)]
    private float _jetpackGlideForce = 0.1f;

    [SerializeField] [Tooltip("Porcentaje sobre la duración maxima que durará la subida del jetpack")] [Range(0, 1)]
    private float _jetpackBoostDuration;

    [SerializeField] [Tooltip("Porcentaje sobre la duración maxima que durará la bajada del jetpack")] [Range(0, 1)]
    private float _jetpackGlideDuration;

    //[SerializeField] [Tooltip("Fuerza maxima que se aplica cuando apretas el gatillo al maximo")] [Range(1, 100)]
    //private float _jetpackTriggerMaxForce;

    private bool _jetpackAlreadyUsed;


    private float _initialJumpVelocity;

    private bool _isJumpPressed;
    private bool _requireNewJumpPress;
    private bool _suppressGravityFrame;

    //BURROW
    private bool _isInteractPressed;

    private float _burrowSpeed = 2f;
    private float _detectionRadius = 0.5f;

    private Interactable _interactable;

    [FormerlySerializedAs("_speed")]
    [Header("Movement Variables")]
    //movement variables
    [SerializeField]
    [Range(1.5f, 20f)]
    private float _walkSpeed = 5f;

    [SerializeField] [Range(1.5f, 20f)] private float _runSpeed = 10;
    [ReadOnly] [SerializeField] private float _airSpeed;

    public float AirSpeed
    {
        get => _airSpeed;
        set => _airSpeed = value;
    }

    [SerializeField] private float _acceleration = 5f;

    [FormerlySerializedAs("_airAcceleration")]
    [Tooltip(
        "Cuanto más bajo sea este valor menos capacidad de maniobrar tienes en el aire, con un 0 no puedes cambiar la dirección.")]
    [SerializeField]
    private float _airForce = 3f;

    [SerializeField] private float _groundDrag;
    [SerializeField] private float _airDrag = 0;
    private float _usedHorizontalAccel;

    [Header("Velocidad")] [ReadOnly] [SerializeField]
    private float _speed; // m/s total

    [ReadOnly] [SerializeField] private float _horizontalSpeed; // m/s solo XZ
    [ReadOnly] [SerializeField] private float _targetHorizontalSpeed; // objetivo

    [Tooltip("La distancia con la que se detecta como de cerca esta el suelo para engancharse a el")] [SerializeField]
    private float _rayLength = 0.5f;

    [SerializeField] private float _snapToGroundForce = 0.5f;

    // state variables
    private PlayerBaseStateRb _currentState;
    private FactoryRigidBody _states;

    [Header("Dash")] [SerializeField] private float _dashDuration;
    [SerializeField] private float _dashSpeed;
    private bool _dashPressed;
    private bool _dashAlreadyUsed;
    [SerializeField] private float _dashCooldown = 1;
    private float _dashRemainingCooldown;

    [SerializeField] private ParticleSystem _jetpack1;
    [SerializeField] private ParticleSystem _jetpack2;

    [SerializeField] private ParticleSystem _dashParticles;


    private AudioPlayer _audioPlayer;

    #endregion

    #region getters and setters

    public PlayerBaseStateRb CurrentState
    {
        get { return _currentState; }
        set { _currentState = value; }
    }

    public InputSystem_Actions PlayerInput
    {
        get { return _playerInput; }
    }

    public bool IsJumpPressed
    {
        get { return _isJumpPressed; }
    }

    public bool RequireNewJumpPress
    {
        get { return _requireNewJumpPress; }
        set { _requireNewJumpPress = value; }
    }

    public float CorrectionDistance
    {
        get { return _correctionDistance; }
    }

    public float CorrectionRayLenght
    {
        get { return _correctionRayLenght; }
    }

    public bool IsMovementPressed
    {
        get { return _isMovementPressed; }
    }

    public bool isRunning
    {
        get { return _isRunning; }
    }

    public float RunMagnitude
    {
        get { return _runMagnitude; }
    }

    public Vector3 CurrentMovementInput
    {
        get { return _currentMovementInput; }
        set { _currentMovementInput = value; }
    }

    public float CurrentMovementY
    {
        get { return _currentMovement.y; }
        set { _currentMovement.y = value; }
    }

    public float InitialJumpVelocity
    {
        get { return _initialJumpVelocity; }
    }


    public float FallMultiplier
    {
        get { return _fallMultiplier; }
    }

    public float CoyoteTime
    {
        get { return _coyoteTime; }
    }

    public float RemainingCoyoteTime
    {
        get { return _remainingCoyoteTime; }
        set { _remainingCoyoteTime = value; }
    }

    public bool CanUseCoyote
    {
        get => _canUseCoyote;
        set => _canUseCoyote = value;
    }

    public bool JustJumped
    {
        get => _justJumped;
        set => _justJumped = value;
    }

    public bool SuppressGravityFrame
    {
        get => _suppressGravityFrame;
        set => _suppressGravityFrame = value;
    }

    public float MinJumpTime
    {
        get => _minJumpTime;
    }

    public float JetpackDuration
    {
        get { return _jetpackDuration; }
    }

    public float JetpackForce
    {
        get { return _jetpackForce; }
    }

    public float JetpackGlideForce
    {
        get { return _jetpackGlideForce; }
    }

    public float JetpackBoostDuration
    {
        get { return _jetpackBoostDuration; }
    }

    public float JetpackGlideDuration
    {
        get { return _jetpackGlideDuration; }
    }

    public bool JetpackAlreadyUsed
    {
        get { return _jetpackAlreadyUsed; }
        set { _jetpackAlreadyUsed = value; }
    }

    public bool JetpackEnabled
    {
        get { return _jetpackEnabled; }
    }

    public bool DashEnabled
    {
        get { return _dashEnabled; }
    }

    public float DashDuration
    {
        get { return _dashDuration; }
    }

    public float DashSpeed
    {
        get { return _dashSpeed; }
    }

    public bool DashAlreadyUsed
    {
        get { return _dashAlreadyUsed; }
        set { _dashAlreadyUsed = value; }
    }

    public bool DashPressed
    {
        get { return _dashPressed; }
        set { _dashPressed = value; }
    }

    public float DashCooldown
    {
        get { return _dashCooldown; }
        set { _dashCooldown = value; }
    }

    public float DashRemainingCooldown
    {
        get { return _dashRemainingCooldown; }
        set { _dashRemainingCooldown = value; }
    }

    public float RunSpeed
    {
        get { return _runSpeed; }
    }


    public float WalkSpeed
    {
        get { return _walkSpeed; }
    }

    public float Acceleration
    {
        get { return _acceleration; }
    }

    public float JetpackTrigger
    {
        get { return _jetpackTrigger; }
    }

    public bool IsGamepad
    {
        get { return _isGamepad; }
    }

    public bool IsInteractPressed
    {
        get { return _isInteractPressed; }
        set { _isInteractPressed = value; }
    }

    public float BurrowSpeed
    {
        get { return _burrowSpeed; }
    }

    public Interactable Interactable
    {
        get { return _interactable; }
    }

    public bool IsGrounded
    {
        get { return _isGrounded; }
    }

    public Animator AnimatorRef
    {
        get { return _animator; }
    }

    public AudioPlayer AudioPlayerRef
    {
        get { return _audioPlayer; }
    }

    public ParticleSystem JetpackParticles1
    {
        get { return _jetpack1; }
    }

    public ParticleSystem JetpackParticles2
    {
        get { return _jetpack2; }
    }

    public Vector3 Velocity
    {
        get => _rb.linearVelocity;
        set => _rb.linearVelocity = value;
    }

    public float RemainingJumpBufferTime
    {
        get { return _remainingJumpBufferTime; }
        set { _remainingJumpBufferTime = value; }
    }

    public Rigidbody Rb => _rb;

    public ParticleSystem DashParticles => _dashParticles;

    public bool ShouldApplyHorizontalMovement { get; set; } = false;

    public Vector3 TargetHorizontalVelocity { get; set; } = Vector3.zero;

    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _groundDrag = _rb.linearDamping;
        _playerInput = new InputSystem_Actions();

        SetupJumpVariables();

        //setup
        _states = new FactoryRigidBody(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();

        //Player Input Callbacks

        print(_playerInput);

        _playerInput.Player.Move.started += OnMovementInput;
        _playerInput.Player.Move.canceled += OnMovementInput;
        _playerInput.Player.Move.performed += OnMovementInput;
        _playerInput.Player.Jump.started += OnJump;
        _playerInput.Player.Jump.canceled += OnJump;
        _playerInput.Player.Dash.started += OnDash;
        _playerInput.Player.Dash.canceled += OnDash;
        _playerInput.Player.Jump.started += onJetpack;
        _playerInput.Player.Jump.canceled += onJetpack;
        _playerInput.Player.JetPack.started += onJetpack;
        _playerInput.Player.JetPack.performed += onJetpack;
        _playerInput.Player.JetPack.canceled += onJetpack;
        _playerInput.Player.Run.started += OnRunPress;
        _playerInput.Player.State.started += stateCheck;
        _usedHorizontalAccel = _acceleration;
    }

    // Update is called once per frame
    void Update()
    {
        _actualState = _currentState.ToString();
        if (_currentState.CurrentSubState != null)
        {
            _actualSubState = _currentState.CurrentSubState.ToString();
            if (_currentState.CurrentSubState.CurrentSubState != null)
            {
                _actualSubSubState = _currentState.CurrentSubState.CurrentSubState.ToString();
            }
            else
            {
                _actualSubSubState = "";
            }
        }
        else
        {
            _actualSubState = "";
        }

        _isGrounded = _groundedByCollision;
        _cameraRelativeMovement =
            ConvertToCameraSpace(new Vector3(_currentMovementInput.x, 0f, _currentMovementInput.y));


        if (_remainingCoyoteTime > 0)
        {
            _remainingCoyoteTime -= Time.deltaTime;
        }

        if (_remainingJumpBufferTime > 0f)
        {
            _remainingJumpBufferTime -= Time.deltaTime;
        }

        if (_justJumped)
        {
            _justJumped = false; // solo dura un frame
        }

        if (RemainingCoyoteTime > 0 && !CanUseCoyote)
        {
            Debug.LogWarning(
                $"CoyoteTime activo ilegalmente | TimeLeft: {RemainingCoyoteTime:F3} | CanUseCoyote: {CanUseCoyote}");
        }

        _currentState.UpdateStates();
    }

    void SetupJumpVariables()
    {
        _initialJumpVelocity = (2 * _maxJumpHeight) / (_maxJumpTime / 2);
    }

    public void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = _cameraRelativeMovement.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = _cameraRelativeMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (_isMovementPressed && positionToLookAt != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(positionToLookAt);
            transform.rotation =
                Quaternion.Slerp(currentRotation, targetRotation, _rotationFactorPerFrame * Time.deltaTime);
        }
    }

    public Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
    {
        float currentYValue = vectorToRotate.y;

        Vector3 cameraForward = Camera.main.transform.forward;
        Vector3 cameraRight = Camera.main.transform.right;

        cameraForward.y = 0;
        cameraRight.y = 0;

        cameraForward = cameraForward.normalized;
        cameraRight = cameraRight.normalized;

        Vector3 cameraForwardZProduct = vectorToRotate.z * cameraForward;
        Vector3 cameraRightXProduct = vectorToRotate.x * cameraRight;

        Vector3 vectorRotatedToCamearSpace = cameraForwardZProduct + cameraRightXProduct;
        vectorRotatedToCamearSpace.y = currentYValue;
        return vectorRotatedToCamearSpace;
    }


    private void FixedUpdate()
    {
        // Mantén drag 0 en suelo; usa algo de drag en aire si quieres.
    _rb.linearDamping = _isGrounded ? 0f : _airDrag;

    Vector3 currentVel = _rb.linearVelocity;

    // Horizontales (XZ)
    Vector3 currentH = new Vector3(currentVel.x, 0f, currentVel.z);
    Vector3 targetH  = ShouldApplyHorizontalMovement
        ? new Vector3(TargetHorizontalVelocity.x, 0f, TargetHorizontalVelocity.z)
        : Vector3.zero;

    // Aceleraciones base
    float accel = _isGrounded ? _acceleration : _airForce;

    // Frenada más fuerte que acelerar (clave para giro inmediato con drag 0)
    // Si no tienes variables dedicadas, usa multiplicadores:
    float brake = accel * (_isGrounded ? 3.0f : 1.5f); // ajusta 2–5x en suelo al gusto

    // ¿Tenemos objetivo real?
    bool hasTarget = targetH.sqrMagnitude > 0.0001f;
    bool hasSpeed  = currentH.sqrMagnitude > 0.0001f;

    Vector3 newH = currentH;

    if (ShouldApplyHorizontalMovement)
    {
        if (hasSpeed && hasTarget)
        {
            float dot = Vector3.Dot(currentH.normalized, targetH.normalized);

            if (dot < 0f)
            {
                // 1) Dirección opuesta: primero FRENAR fuerte hacia 0
                newH = Vector3.MoveTowards(currentH, Vector3.zero, brake * Time.fixedDeltaTime);

                // 2) Luego (en ticks sucesivos) acelerar hacia la nueva dirección
                //    (cuando ya esté cerca de 0, la siguiente rama de abajo se encargará)
            }
            else
            {
                // Misma dirección o similar:
                // - Si vamos más rápido que el target, aplicamos frenada suave (coast)
                // - Si vamos más lento, aceleramos
                if (currentH.magnitude > targetH.magnitude + 0.01f)
                    newH = Vector3.MoveTowards(currentH, targetH, brake * Time.fixedDeltaTime);
                else
                    newH = Vector3.MoveTowards(currentH, targetH, accel * Time.fixedDeltaTime);
            }
        }
        else if (hasTarget)
        {
            // Estábamos prácticamente parados: acelera directo al target
            newH = Vector3.MoveTowards(currentH, targetH, accel * Time.fixedDeltaTime);
        }
        else
        {
            // No hay input/objetivo: en suelo, para en seco; en aire, conserva
            if (_isGrounded && !_isMovementPressed)
                newH = Vector3.zero;
            else
                newH = currentH;
        }
    }
    else
    {
        // El estado indica que no apliquemos movimiento horizontal:
        if (_isGrounded && !_isMovementPressed)
            newH = Vector3.zero;
        else
            newH = currentH;
    }

    // Aplica nueva velocidad manteniendo la Y actual
    _rb.linearVelocity = new Vector3(newH.x, currentVel.y, newH.z);

    // (Si tu lógica de aire necesita “conservar crucero” o limitar lateral, hazlo en los estados,
    //  pero aquí ya garantizamos giro contundente en suelo con drag=0.)

    // Métricas auxiliares (si las usas en tu SM)
    Vector3 v = _rb.linearVelocity;
    _speed                 = v.magnitude;
    _horizontalSpeed       = new Vector2(v.x, v.z).magnitude;
    _targetHorizontalSpeed = TargetHorizontalVelocity.magnitude;

    // Rotación y stickiness como lo tengas
    HandleRotation();
    if (_snapToGround) ApplyGroundStickiness();
    }


    private void ApplyGroundStickiness()
    {
        Vector3 vel = Velocity;

        if (IsGrounded)
        {
            // Suaviza la caída si estas bajando rápido
            if (vel.y < -1f)
            {
                vel.y = -1f;
                Velocity = vel;
            }
        }
        else
        {
            // casi tocando el suelo
            Vector3 origin = _groundCheck.position + Vector3.up * 0.1f;
            float rayLength = 0.5f;

            if (Physics.Raycast(origin, Vector3.down, out RaycastHit hit, rayLength, _groundMask))
            {
                // Snap al suelo desde el aire si estamos cerca y cayendo despacio
                if (vel.y <= 0f)
                {
                    vel.y = -_snapToGroundForce; // fuerza hacia abajo para pegarse
                    Velocity = vel;
                }
            }
        }
    }

    public void TryCornerCorrection()
    {
        Vector3 topLeft = transform.position + transform.right * -0.5f + Vector3.up * 1.0f;
        Vector3 topRight = transform.position + transform.right * 0.5f + Vector3.up * 1.0f;

        bool leftHit = Physics.Raycast(topLeft, Vector3.up, _correctionRayLenght, _groundMask);
        bool rightHit = Physics.Raycast(topRight, Vector3.up, _correctionRayLenght, _groundMask);

        if (leftHit && !rightHit)
        {
            transform.position += transform.right * _correctionDistance;
        }
        else if (!leftHit && rightHit)
        {
            transform.position += -transform.right * _correctionDistance;
        }
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        var device = context.control.device;

        _isGamepad = device is Gamepad;

        _currentMovementInput = context.ReadValue<Vector2>();
        _currentMovement.x = _currentMovementInput.x;
        _currentMovement.z = _currentMovementInput.y;
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
    }

    void OnRunPress(InputAction.CallbackContext context)
    {
        _isRunning = !isRunning;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isGamepad = context.control.device is Gamepad;
        _isJumpPressed = context.ReadValueAsButton();

        if (_isJumpPressed)
        {
            _remainingJumpBufferTime = _jumpBufferTime;
        }
        else
        {
            _requireNewJumpPress = false;
        }
    }

    void OnDash(InputAction.CallbackContext context)
    {
        _isGamepad = context.control.device is Gamepad;

        _dashPressed = context.ReadValueAsButton();
    }

    void onJetpack(InputAction.CallbackContext context)
    {
        if (!context.ReadValueAsButton())
        {
            _jetpackTrigger = 0;
        }
        else if (!_requireNewJumpPress)
        {
            _jetpackTrigger = context.ReadValue<float>();
        }

        //_isGamepad = true;
        //print(_jetpackTrigger);
    }

    void onInteract(InputAction.CallbackContext context)
    {
        IsInteractPressed = context.ReadValueAsButton();
        if (_interactable != null)
        {
            _interactable.Interact(this.gameObject.GetComponent<PlayerController>());
        }
    }


    public bool IsNearSand()
    {
        Collider[] hitColliders =
            Physics.OverlapSphere(transform.position, _detectionRadius, LayerMask.GetMask("Sand"));
        return hitColliders.Length > 0;
    }

    void stateCheck(InputAction.CallbackContext context)
    {
        print("El estado actual es: " + _currentState);
        if (_currentState.CurrentSuperState != null)
        {
            print("El estado super es: " + _currentState.CurrentSuperState);
        }

        if (_currentState.CurrentSubState != null)
        {
            print("El estado sub es: " + _currentState.CurrentSubState);
        }
    }

    private void OnEnable()
    {
        _playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            _interactable = other.GetComponent<Interactable>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            if (!IsNearSand())
            {
                Debug.Log("Saliendo de la arena");
                _interactable = null;
                IsInteractPressed = false;
            }
        }
    }

    private void OnCollisionStay(Collision other)
    {
        foreach (ContactPoint contact in other.contacts)
        {
            // Verifica que la colisión sea con el suelo (por capa) y que el contacto esté orientado hacia arriba
            if (((1 << other.gameObject.layer) & _groundMask) != 0 && contact.normal.y > 0.5f)
            {
                _groundedByCollision = true;
                return;
            }
        }
    }

    private void OnCollisionExit(Collision other)
    {
        if (((1 << other.gameObject.layer) & _groundMask) != 0)
        {
            _groundedByCollision = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_groundCheck.position, _groundDistance);

        Gizmos.color = Color.red;
        Gizmos.DrawRay(transform.position + transform.right * -0.5f + Vector3.up * 1.0f, Vector3.up * 0.5f);
        Gizmos.DrawRay(transform.position + transform.right * 0.5f + Vector3.up * 1.0f, Vector3.up * 0.5f);

        Debug.DrawRay(transform.position, Vector3.down * 0.5f, _isGrounded ? Color.green : Color.red);
    }

    private void OnValidate()
    {
        SetupJumpVariables();
    }

    public void LogJumpDebug(string origin)
    {
        Debug.Log(
            $"[{origin}] velY: {Velocity.y:F3} | isGrounded: {IsGrounded} | suppressGravity: {SuppressGravityFrame} | justJumped: {JustJumped} | remainingCoyote: {RemainingCoyoteTime:F3} | buffer: {RemainingJumpBufferTime:F3} | requireNewPress: {RequireNewJumpPress}");
    }

    public JumpSource LastJumpSource { get; set; } = JumpSource.Unknown;
}
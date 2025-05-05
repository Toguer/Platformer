using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.Serialization;

public class RbPlayerStateMachine : MonoBehaviour
{
    #region variables

    [Header("Desactivables")] [SerializeField]
    private bool _snapToGround = false;

    private Rigidbody _rb;
    private InputSystem_Actions _playerInput;

    [Header("GroundChecker")] [SerializeField]
    private LayerMask _groundMask;

    private bool _isGrounded;
    [SerializeField] private Transform _groundCheck;
    [SerializeField] private float _groundDistance = 0.4f;


    private Animator _animator;

    //Variables para inputs del jugador
    private Vector2 _currentMovementInput;
    private Vector3 _currentMovement;
    private Vector3 _currentRunMovement;
    private Vector3 _cameraRelativeMovement;

    private bool _isMovementPressed;
    private bool _isRunning;
    [Range(0f, 1f)] [SerializeField] private float _runMagnitude;
    private float _jetpackTrigger;

    private bool _isGamepad;

    [Tooltip("Es la velocidad a la que el personaje rota para adaptarse a la dirección en la que camina.")]
    [SerializeField]
    private float _rotationFactorPerFrame = 15.0f;

    [Header("Jump Variables")] [Tooltip("Altura maxima de salto")] [SerializeField]
    private float _maxJumpHeight = 4.0f;

    [Tooltip("Duración maxima del salto")] [SerializeField]
    private float _maxJumpTime = 0.75f;

    [Tooltip("Velocidad adicional a la que el jugador caerá cuando no pulsa saltar.")] [SerializeField]
    private float _fallMultiplier = 2.0f;

    [Tooltip("Es el margen de error que tiene un jugador para saltar despues de caer de una esquina.")]
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float _coyoteTime = 0.1f;

    [Tooltip("El tiempo que el input de salto se guarda")] [SerializeField]
    private float _jumpBufferTime = 0.2f;

    private float _remainingJumpBufferTime = 0f;

    private float _remainingCoyoteTime;

    [Header("Jetpack")] [Tooltip("La duración del efecto jetpack")] [SerializeField] [Range(0f, 10.0f)]
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
    [SerializeField] private float _acceleration = 5f;
    [SerializeField] private float _airAcceleration = 3f;

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

    public bool shouldApplyHorizontalMovement { get; set; } = false;

    #endregion

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
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
    }

    // Update is called once per frame
    void Update()
    {
        _isGrounded = Physics.CheckSphere(_groundCheck.position, _groundDistance, _groundMask);
        _cameraRelativeMovement =
            ConvertToCameraSpace(new Vector3(_currentMovementInput.x, 0f, _currentMovementInput.y));

        HandleRotation();


        _currentState.UpdateStates();

        if (_remainingCoyoteTime > 0)
        {
            _remainingCoyoteTime -= Time.deltaTime;
        }

        if (_remainingJumpBufferTime > 0f)
        {
            _remainingJumpBufferTime -= Time.deltaTime;
        }
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
        Vector3 currentVelocity = _rb.linearVelocity;
        Vector3 moveDir = _cameraRelativeMovement.normalized;
        float inputMagnitude = _currentMovementInput.magnitude;
        Vector3 targetVelocity;

        if (CurrentState is PlayerGroundedStateRb)
        {
            if (shouldApplyHorizontalMovement)
            {
                currentVelocity.y = _rb.linearVelocity.y; // Mantener Y!
                float targetSpeed = Mathf.Lerp(_walkSpeed, _runSpeed, inputMagnitude);
                targetVelocity = moveDir * targetSpeed;

                Vector3 horizontalVelocity = Vector3.Lerp(
                    new Vector3(_rb.linearVelocity.x, 0f, _rb.linearVelocity.z),
                    targetVelocity,
                    _acceleration * Time.fixedDeltaTime);
                _rb.linearVelocity = new Vector3(horizontalVelocity.x, _rb.linearVelocity.y, horizontalVelocity.z);
            }
        }
        else
        {
            targetVelocity = new Vector3(currentVelocity.x, currentVelocity.y, currentVelocity.z);

            if (shouldApplyHorizontalMovement && inputMagnitude > 0.1f)
            {
                Vector3 desiredDirection = _cameraRelativeMovement.normalized;
                float targetSpeed = Mathf.Lerp(_walkSpeed, _runSpeed, inputMagnitude);
                Vector3 desiredVelocity = desiredDirection * targetSpeed;

                // En el aire: interpolar, pero más lento que en suelo

                targetVelocity.x = Mathf.Lerp(currentVelocity.x, desiredVelocity.x,
                    _airAcceleration * Time.fixedDeltaTime);
                targetVelocity.z = Mathf.Lerp(currentVelocity.z, desiredVelocity.z,
                    _airAcceleration * Time.fixedDeltaTime);
            }
            else
            {
                // No input ➔ mantenemos momentum, sin frenar en seco
                targetVelocity.x = Mathf.Lerp(currentVelocity.x, 0f, 0.02f); // Fricción casi nula
                targetVelocity.z = Mathf.Lerp(currentVelocity.z, 0f, 0.02f);
            }

            _rb.linearVelocity = targetVelocity;
        }

        if (_snapToGround)
            ApplyGroundStickiness();
    }

    private void ApplyGroundStickiness()
    {
        Vector3 vel = Velocity;

        if (IsGrounded)
        {
            // Ya estás grounded: suaviza la caída si vienes bajando rápido
            if (vel.y < -1f)
            {
                vel.y = -1f;
                Velocity = vel;
            }
        }
        else
        {
            // No grounded, pero... ¿casi tocando el suelo?
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

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(_groundCheck.position, _groundDistance);
    }

    private void OnValidate()
    {
        SetupJumpVariables();
    }
}
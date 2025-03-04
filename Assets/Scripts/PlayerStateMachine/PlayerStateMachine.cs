using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;

public class PlayerStateMachine : MonoBehaviour
{
    #region variables

    private InputSystem_Actions _playerInput;
    private CharacterController _characterController;

    //Variables para inputs del jugador
    private Vector2 _currentMovementInput;
    private Vector3 _currentMovement;
    private Vector3 _currentRunMovement;
    private Vector3 _appliedMovement;
    private Vector3 _cameraRelativeMovement;

    private bool _isMovementPressed;
    private bool _isRunPressed;
    private float _jetpackTrigger;

    private bool _isGamepad;

    private float _rotationFactorPerFrame = 15.0f;

    [Header("Jump Variables")] [Tooltip("Altura maxima de salto")] [SerializeField]
    private float _maxJumpHeight = 4.0f;

    [Tooltip("Duración maxima del salto")] [SerializeField]
    private float _maxJumpTime = 0.75f;

    [Tooltip("Velocidad adicional a la que el jugador caerá cuando no pulsa saltar.")] [SerializeField]
    private float _fallMultiplier = 2.0f;

    [Tooltip("Es el margen de error que tiene un jugador para saltar despues de caer de una esquina.")]
    [SerializeField]
    [Range(0.1f, 1.0f)]
    private float _coyoteTime = 0.1f;

    private float _remainingCoyoteTime;

    [Header("Jetpack")] [Tooltip("La duración del efecto jetpack")] [SerializeField] [Range(0f, 10.0f)]
    private float _jetpackDuration;

    [Tooltip("La fuerza que tiene el Jetpack, cuanto más alta sea más alto llegará")] [SerializeField]
    private float _jetpackForce = 0.2f;

    [Tooltip("La fuerza que tiene el Jetpack, cuanto más alta sea más alto llegará")]
    [SerializeField]
    [Range(0.05f, 0.5f)]
    private float _jetpackGlideForce = 0.1f;

    [Tooltip("Velocidad maxima a la que puede ir el Jetpack")] [SerializeField]
    private float _maxJetpackVelocity = 1f;

    [Tooltip("Velocidad Minima a la que puede ir el Jetpack")] [SerializeField]
    private float _minJetpackVelocity = -5f;

    [SerializeField] [Tooltip("Porcentaje sobre la duración maxima que durará la subida del jetpack")] [Range(0, 1)]
    private float _jetpackBoostDuration;

    [SerializeField] [Tooltip("Porcentaje sobre la duración maxima que durará la bajada del jetpack")] [Range(0, 1)]
    private float _jetpackGlideDuration;

    [SerializeField] [Tooltip("Fuerza maxima que se aplica cuando apretas el gatillo al maximo")] [Range(1, 100)]
    private float _jetpackTriggerMaxForce;

    [SerializeField] private bool _jetpackAlreadyUsed = false;


    private float _initialJumpVelocity;

    [SerializeField] private bool _isJumpPressed;
    private bool _requireNewJumpPress = false;

    [Header("Burrow")] [SerializeField] private bool _isEarthPressed;

    [SerializeField] private float _burrowSpeed = 2f;
    [SerializeField] private float _detectionRadius = 0.5f;

    private Interactable _interactable;

    [Header("Movement Variables")]
    //movement variables
    [SerializeField]
    private float _runMultiplier = 3.0f;

    [SerializeField] [Range(1.5f, 10f)] private float _speed = 1.5f;

    //gravity
    private float _gravity = -9.8f;
    private float _groundedGravity = -0.5f;

    // state variables
    [SerializeField] private PlayerBaseState _currentState;
    private PlayerStateFactory _states;

    [Header("Dash")] [SerializeField] private float _dashDuration;
    [SerializeField] private float _dashSpeed;
    [SerializeField] private bool _dashPressed;
    [SerializeField] private bool _dashAlreadyUsed;
    [SerializeField] private float _dashCooldown = 1;
    private float _dashRemainingCooldown;

    #endregion

    #region getters and setters

    public PlayerBaseState CurrentState
    {
        get { return _currentState; }
        set { _currentState = value; }
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

    public bool IsRunPressed
    {
        get { return _isRunPressed; }
    }

    public CharacterController CharacterController
    {
        get { return _characterController; }
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

    public float CurrentRunMovementY
    {
        get { return _currentRunMovement.y; }
        set { _currentRunMovement.y = value; }
    }

    public float AppliedMovementX
    {
        get { return _appliedMovement.x; }
        set { _appliedMovement.x = value; }
    }

    public float AppliedMovementY
    {
        get { return _appliedMovement.y; }
        set { _appliedMovement.y = value; }
    }

    public float AppliedMovementZ
    {
        get { return _appliedMovement.z; }
        set { _appliedMovement.z = value; }
    }

    public float GroundedGravity
    {
        get { return _groundedGravity; }
    }

    public float Gravity
    {
        get { return _gravity; }
    }


    public float InitialJumpVelocity
    {
        get { return _initialJumpVelocity; }
    }

    public float RunMultiplier
    {
        get { return _runMultiplier; }
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

    public float MaxJetpackVelocity
    {
        get { return _maxJetpackVelocity; }
    }

    public float MinJetpackVelocity
    {
        get { return _minJetpackVelocity; }
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

    public float JetpackTrigger
    {
        get { return _jetpackTrigger; }
    }

    public bool IsGamepad
    {
        get { return _isGamepad; }
    }

    public bool IsEarthPressed
    {
        get { return _isEarthPressed; }
        set { _isEarthPressed = value; }
    }

    public float BurrowSpeed
    {
        get { return _burrowSpeed; }
    }

    public Interactable Interactable
    {
        get { return _interactable; }
    }

    public float RotationSpeed
    {
        get { return _rotationFactorPerFrame; }
    }

    #endregion


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();

        _playerInput = new InputSystem_Actions();

        // setup state
        _states = new PlayerStateFactory(this);
        _currentState = _states.Grounded();
        _currentState.EnterState();
        //Player Input Callbacks

        _playerInput.Player.Move.started += OnMovementInput;
        _playerInput.Player.Move.canceled += OnMovementInput;
        _playerInput.Player.Move.performed += OnMovementInput;
        _playerInput.Player.Jump.started += OnJump;
        _playerInput.Player.Jump.canceled += OnJump;
        _playerInput.Player.Dash.started += OnDash;
        _playerInput.Player.Dash.canceled += OnDash;
        _playerInput.Player.JetPack.started += onJetpack;
        _playerInput.Player.JetPack.performed += onJetpack;
        _playerInput.Player.JetPack.canceled += onJetpack;
        _playerInput.Player.EarthPower.started += onEarth;
        _playerInput.Player.EarthPower.canceled += onEarth;
        SetupJumpVariables();
    }

// Update is called once per frame
    void Update()
    {
        if ((_currentState is PlayerBurrowState))
        {
            _characterController.Move(new Vector3(0, _appliedMovement.y * Time.deltaTime, 0));
            _characterController.Move(new Vector3(_appliedMovement.x * _speed, 0, _appliedMovement.z * _speed) * Time.deltaTime);
        }
        else
        {
            _cameraRelativeMovement = ConvertToCameraSpace(_appliedMovement);
            HandleRotation();

            Vector3 horizontalMovement = new Vector3(_cameraRelativeMovement.x, 0, _cameraRelativeMovement.z);

            _characterController.Move(horizontalMovement * (_speed * Time.deltaTime));
        }


        _currentState.UpdateStates();

        if (_remainingCoyoteTime > 0)
        {
            _remainingCoyoteTime -= Time.deltaTime;
        }
    }

    public void HandleRotation()
    {
        Vector3 positionToLookAt;

        positionToLookAt.x = _cameraRelativeMovement.x;
        positionToLookAt.y = 0;
        positionToLookAt.z = _cameraRelativeMovement.z;

        Quaternion currentRotation = transform.rotation;

        if (_isMovementPressed)
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

    void SetupJumpVariables()
    {
        float timeToApex = _maxJumpTime / 2;
        _gravity = (-2 * _maxJumpHeight) / Mathf.Pow(timeToApex, 2);
        _initialJumpVelocity = (2 * _maxJumpHeight) / timeToApex;
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        _currentMovementInput = context.ReadValue<Vector2>();
        _currentMovement.x = _currentMovementInput.x;
        _currentMovement.z = _currentMovementInput.y;
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        if (_requireNewJumpPress && !_isJumpPressed)
        {
            _requireNewJumpPress = false;
        }
    }

    void OnDash(InputAction.CallbackContext context)
    {
        _dashPressed = context.ReadValueAsButton();
    }

    void onJetpack(InputAction.CallbackContext context)
    {
        if (!context.ReadValueAsButton())
        {
            _jetpackTrigger = 0;
        }
        else
        {
            float rawTrigger = context.ReadValue<float>();
            _jetpackTrigger = Mathf.Lerp(0.5f, _jetpackTriggerMaxForce, rawTrigger);
        }

        _isGamepad = true;
        //print(_jetpackTrigger);
    }

    void onEarth(InputAction.CallbackContext context)
    {
        IsEarthPressed = context.ReadValueAsButton();
    }

    public bool IsNearSand()
    {
        Collider[] hitColliders =
            Physics.OverlapSphere(transform.position, _detectionRadius, LayerMask.GetMask("Sand"));
        return hitColliders.Length > 0;
    }

    private void OnEnable()
    {
        _playerInput.Player.Enable();
    }

    private void OnDisable()
    {
        _playerInput.Player.Disable();
    }

    private void OnValidate()
    {
        SetupJumpVariables();
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
                IsEarthPressed = false;
            }
        }
    }
}
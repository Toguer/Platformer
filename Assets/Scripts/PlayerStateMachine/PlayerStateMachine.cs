using System;
using UnityEngine;
using UnityEngine.InputSystem;

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

    [Tooltip("La duración del efecto jetpack")] [SerializeField] [Range(0f, 2.0f)]
    private float _jetpackDuration;

    [Tooltip("La fuerza que tiene el Jetpack, cuanto más alta sea más alto llegará")] [SerializeField]
    private float _jetpackForce = 5f;

    [SerializeField] private float _maxJetpackVelocity = 1f;

    [SerializeField] private bool _jetpackAlreadyUsed = false;

    private float _initialJumpVelocity;

    [SerializeField] private bool _isJumpPressed;
    private bool _requireNewJumpPress = false;

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

    public float MaxJetpackVelocity
    {
        get { return _maxJetpackVelocity; }
    }

    public bool JetpackAlreadyUsed
    {
        get { return _jetpackAlreadyUsed; }
        set { _jetpackAlreadyUsed = value; }
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
        _playerInput.Player.Run.started += onRun;
        _playerInput.Player.Run.canceled += onRun;
        _playerInput.Player.Jump.started += OnJump;
        _playerInput.Player.Jump.canceled += OnJump;

        SetupJumpVariables();
    }

// Update is called once per frame
    void Update()
    {
        _cameraRelativeMovement = ConvertToCameraSpace(_appliedMovement);
        HandleRotation();
        
        Vector3 horizontalMovement = new Vector3(_cameraRelativeMovement.x, 0, _cameraRelativeMovement.z);
        _characterController.Move(horizontalMovement * (_speed * Time.deltaTime));
        _characterController.Move(new Vector3(0, _appliedMovement.y * Time.deltaTime, 0));
        //_characterController.Move(_cameraRelativeMovement * (_speed * Time.deltaTime));
        //HandleGravity();

        _currentState.UpdateStates();

        if (_remainingCoyoteTime > 0)
        {
            _remainingCoyoteTime -= Time.deltaTime;
        }
    }

    void HandleRotation()
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

    Vector3 ConvertToCameraSpace(Vector3 vectorToRotate)
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

    void HandleGravity()
    {
        bool isFalling = _currentMovement.y <= 0.0f || !_isJumpPressed;
        float fallMultiplier = 2.0f;
        if (_characterController.isGrounded)
        {
            _currentMovement.y = _groundedGravity;
            _currentRunMovement.y = _groundedGravity;
        }
        else if (isFalling)
        {
            float previousYVelocity = _currentMovement.y;
            _currentMovement.y = _currentMovement.y + (_gravity * fallMultiplier * Time.deltaTime);
            _appliedMovement.y = Mathf.Max((previousYVelocity + _currentMovement.y) * 0.5f, -20.0f);
        }
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
        _currentRunMovement.x = _currentMovementInput.x * _runMultiplier;
        _currentRunMovement.z = _currentMovementInput.y * _runMultiplier;
        _isMovementPressed = _currentMovementInput.x != 0 || _currentMovementInput.y != 0;
    }

    void OnJump(InputAction.CallbackContext context)
    {
        _isJumpPressed = context.ReadValueAsButton();
        if (_requireNewJumpPress && !_isJumpPressed)
        {
            _requireNewJumpPress = false;
        }
        //_requireNewJumpPress = true;
    }

    void onRun(InputAction.CallbackContext context)
    {
        _isRunPressed = context.ReadValueAsButton();
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
}
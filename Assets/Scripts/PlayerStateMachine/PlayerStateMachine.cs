using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerStateMachine : MonoBehaviour
{
    private InputSystem_Actions _playerInput;
    private CharacterController _characterController;
    private Rigidbody _rb;


    //Variables para inputs del jugador
    private Vector2 _currentMovementInput;
    private Vector3 _currentMovement;
    private Vector3 _currentRunMovement;
    private Vector3 _appliedMovement;
    private bool _isMovementPressed;
    private bool _isRunPressed;
    private float _rotationFactorPerFrame = 15.0f;


    //jump variables
    private float _initialJumpVelocity;
    private float _maxJumpHeight = 4.0f;
    private float _maxJumpTime = 0.75f;
    private bool _isJumpPressed;
    private bool _requireNewJumpPress = false;

    //movement variables
    private float _runMultiplier = 3.0f;

    //gravity
    private float _gravity = -9.8f;
    private float _groundedGravity = -0.5f;

    // state variables
    [SerializeField] private PlayerBaseState _currentState;
    private PlayerStateFactory _states;

    // getters and setters


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


    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _rb = GetComponent<Rigidbody>();

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
        if (_isRunPressed)
        {
            _appliedMovement.x = _currentMovement.x;
            _appliedMovement.z = _currentMovement.z;

            //_characterController.Move(_currentRunMovement * Time.deltaTime);
        }
        else
        {
            _appliedMovement.x = _currentRunMovement.x;
            _appliedMovement.z = _currentRunMovement.z;
            //_characterController.Move(_currentMovement * Time.deltaTime);
        }

        _characterController.Move(_appliedMovement * Time.deltaTime);

        HandleGravity();
        _currentState.UpdateStates();
    }

    void HandleGravity()
    {
        bool isFalling = _currentMovement.y <= 0.0f;
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
        _requireNewJumpPress = false;
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
}
using System.Collections.Generic;
using Singleton;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

public class GameManager : PersistentSingleton<GameManager>
{
    #region variables

    [SerializeField] private int _frameCap;
    [SerializeField] private RbPlayerStateMachine _playerStateMachine;

    [Header("Score")] public int _coinsScore;
    [SerializeField] private int _maxCoins;

    [FormerlySerializedAs("updateCoinCanvas")] [SerializeField]
    private UnityEvent<int> _updateCoinCanvas;

    private List<string> _objCollected = new List<string>();

    [FormerlySerializedAs("WinCanvas")] [SerializeField]
    private GameObject _winCanvas;

    [SerializeField] private GameObject _pauseCanvas;
    [SerializeField] private GameObject _settingsCanvas;


    private InputSystem_Actions _inputSystemActions;

    #endregion

    #region getters and setters

    public void SetCoinsScore(int coinsScore)
    {
        this._coinsScore = coinsScore;
    }

    public int GetCoinsScore()
    {
        return this._coinsScore;
    }

    public void SetObjList(List<string> objCollected)
    {
        this._objCollected = objCollected;
    }

    public List<string> GetObjList()
    {
        return this._objCollected;
    }

    #endregion

    #region enable and disable

    private void OnEnable()
    {
        CollectObject.OnUpdateCoinsScore += UpdateCoinsScore;
    }

    private void OnDisable()
    {
        CollectObject.OnUpdateCoinsScore -= UpdateCoinsScore;
    }

    #endregion

    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Application.targetFrameRate = _frameCap;
        _playerStateMachine = FindObjectsByType<RbPlayerStateMachine>(FindObjectsSortMode.None)[0]
            .GetComponent<RbPlayerStateMachine>();
        Time.timeScale = 1;
    }

    private void Start()
    {
        _coinsScore = 0;
        _inputSystemActions = _playerStateMachine.PlayerInput;
        print(_inputSystemActions);
        _inputSystemActions.UI.Pause.started += Pause;
        _inputSystemActions.Player.Pause.started += Pause;
    }

    public void UpdateCoinsScore(int coinsScore)
    {
        this._coinsScore += coinsScore;
        _updateCoinCanvas.Invoke(this._coinsScore);
    }

    public void FinalDor(GameObject Door)
    {
        if (_coinsScore == _maxCoins)
        {
            Door.GetComponent<Animator>().SetBool("isOpen", true);
            _winCanvas.SetActive(true);
        }
    }

    private void Pause(InputAction.CallbackContext context)
    {
        Pause();
    }

    public void Pause()
    {
        if (_pauseCanvas)
        {
            _pauseCanvas.SetActive(!_pauseCanvas.activeSelf);
            if (_pauseCanvas.activeSelf)
            {
                Time.timeScale = 0;
                _inputSystemActions.Player.Disable();
                _inputSystemActions.UI.Enable();
                Cursor.lockState = CursorLockMode.None;
                if (Gamepad.all.Count <= 0)
                {
                    Cursor.visible = true;
                }
            }
            else
            {
                Time.timeScale = 1;

                _inputSystemActions.UI.Disable();
                _inputSystemActions.Player.Enable();
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }
            
        if (_settingsCanvas)
            _settingsCanvas.SetActive(false);
        
    }

    public void Settings()
    {
        _settingsCanvas.SetActive(!_settingsCanvas.activeSelf);
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
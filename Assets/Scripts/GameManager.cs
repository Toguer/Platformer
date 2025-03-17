using System.Collections.Generic;
using Singleton;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : PersistentSingleton<GameManager>
{
    #region variables

    [SerializeField] private int _frameCap;

    [Header("Score")]
    private int _coinsScore;
    [SerializeField] private UnityEvent<int> updateCoinCanvas;

    private List<string> _objCollected = new List<string>();
    
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
        Application.targetFrameRate = _frameCap;
    }
    private void Start()
    {
        _coinsScore = 0;
    }

    public void UpdateCoinsScore(int coinsScore)
    {
        this._coinsScore += coinsScore;
        updateCoinCanvas.Invoke(this._coinsScore);
    }
}
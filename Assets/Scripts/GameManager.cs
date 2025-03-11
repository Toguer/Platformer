using System;
using Singleton;
using UnityEngine;
using UnityEngine.Events;

public class GameManager : PersistentSingleton<GameManager>
{
    #region variables

    [SerializeField] private int _frameCap;

    [Header("Score")]
    private int coinsScore;
    [SerializeField] private UnityEvent<int> updateCoinCanvas;

    #endregion

    #region getters and setters

    public void SetCoinsScore(int coinsScore)
    {
        this.coinsScore = coinsScore;
    }
    public int GetCoinsScore()
    {
        return this.coinsScore;
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
        coinsScore = 0;
    }

    public void UpdateCoinsScore(int coinsScore)
    {
        this.coinsScore += coinsScore;
        updateCoinCanvas.Invoke(this.coinsScore);
    }
}
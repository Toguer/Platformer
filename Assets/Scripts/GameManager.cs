using System;
using Singleton;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{
    [SerializeField] private int _frameCap;


    private void Awake()
    {
        Application.targetFrameRate = _frameCap;
    }
}
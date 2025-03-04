using System;
using Singleton;
using UnityEngine;

public class GameManager : PersistentSingleton<GameManager>
{
    [SerializeField] private int _frameCap;


    private void Awake()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Application.targetFrameRate = _frameCap;
    }
}
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    private string currentLevel;

    
    public List<Scene> sceneList = new List<Scene>();

    [SerializeField] private GameObject _settingsMenu;
    [SerializeField] private GameObject _mainMenu;

    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Debug.Log("Este singleton ya exisate borrando objeto:" + gameObject.name);
            Destroy(gameObject);
        }
    }

    //a�adir el nombre de la escena para para cambiar, y se guarda la escena a la que cambies 
    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        currentLevel = sceneName;
    }

    public void UnloadLevel(string sceneName)
    {
        SceneManager.LoadSceneAsync(sceneName);
    }

    // recargas la escena en la que estas
    public void UnloadCurrentLevel()
    {
        SceneManager.LoadScene(currentLevel);
    }

    public string GetCurrentLevel()
    {
        return currentLevel;
    }

    public void Exit()
    {
        Application.Quit();
    }

    public void OnEscape(InputAction.CallbackContext context)
    {
        foreach (var canvas in FindObjectsByType<Canvas>(FindObjectsSortMode.None))
        {
            canvas.gameObject.SetActive(false);
        }

        _mainMenu.SetActive(true);
    }

    public void Settings()
    {
        _settingsMenu.SetActive(!_settingsMenu.activeSelf);
        _mainMenu.SetActive(!_mainMenu.activeSelf);
    }

    public void DiscordLink()
    {
        Application.OpenURL("https://discord.gg/vGpTuDd7mj");
    }

    public void ItchioLink()
    {
        Application.OpenURL("https://bit-bounty.itch.io/voidborn");
    }
}
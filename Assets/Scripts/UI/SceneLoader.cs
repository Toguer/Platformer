using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public static SceneLoader Instance;
    private string currentLevel;
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
    //añadir el nombre de la escena para para cambiar, y se guarda la escena a la que cambies 
    public void LoadLevel(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
        currentLevel = sceneName;
    }

    //cambiar de escena sin guardarte el nombre 
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
}

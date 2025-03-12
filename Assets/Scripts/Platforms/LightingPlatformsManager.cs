using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightingPlatformsManager : MonoBehaviour
{
    #region variables
    [SerializeField] private GameObject dor;
    [SerializeField] private List<GameObject> lightPlatform = new List<GameObject>();

    public bool isGroudedFlor;
    public bool isGetLight;
    #endregion
    #region getters and setters
    public bool GetisGroudedFlor()
    {
        return isGroudedFlor;
    }
    public void SetisGroudedFlor(bool isGroudedFlor)
    {
        this.isGroudedFlor = isGroudedFlor;
    }
    public bool GetisLight()
    {
        return isGetLight;
    }
    public void SetisGetLight(bool isGetLight)
    {
        this.isGetLight = isGetLight;
    }
    #endregion
    void Start()
    {
        lightPlatform.Clear();

        // Recorrer todos los hijos del objeto padre y agregarlos a la lista
        foreach (Transform child in this.transform)
        {
            lightPlatform.Add(child.gameObject);
        }

        //for(int i = 0; i < lightPlatform.Count; i++) { Debug.Log(lightPlatform[i]); }
    }

    void Update()
    {
        if (isGroudedFlor)
        {
            for (int i = 0; i < lightPlatform.Count; i++)
            {
                lightPlatform[i].GetComponent<LightPlatform>().RestartMaterial();
            }
            isGroudedFlor = false;
        }
    }
    private bool ComprobePlatforms()
    {
        foreach(GameObject platform in lightPlatform)
        {
            if (!platform.GetComponent<LightPlatform>().GetIsLighting())
            {
                return false;
            }
        }
        return true;
    }
    public void OpenDors()
    {
        if (ComprobePlatforms())
        {
            dor.gameObject.SetActive(false);
        }
    }
}

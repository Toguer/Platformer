using NUnit.Framework;
using System.Collections.Generic;
using System.Security;
using Unity.VisualScripting;
using UnityEngine;

public class LightingPathManager : MonoBehaviour
{
    #region variables
    [SerializeField] private GameObject door;
    [SerializeField] private GameObject lightFeedback;
    [SerializeField] private List<GameObject> lightPlatform = new List<GameObject>();

    [Header("Materials")]
    [SerializeField] private Material _material;
    [SerializeField] private Material _emissiveMaterial;
    
    public bool isGetLight;
    #endregion
    #region getters and setters
    public bool GetisLight()
    {
        return isGetLight;
    }
    public void SetisGetLight(bool isGetLight)
    {
        this.isGetLight = isGetLight;
        whenLightOn();
    }
    #endregion

    private void Start()
    {
        lightPlatform.Clear();

        foreach (Transform child in this.transform)
        {
            lightPlatform.Add(child.transform.GetChild(0).gameObject);
        }
    }
    private void whenLightOn()
    {
        if (isGetLight)
        {
            for (int i = 0; i < lightPlatform.Count; i++)
            {
                lightPlatform[i].GetComponent<Renderer>().material = _emissiveMaterial;
            }
            lightFeedback.SetActive(true);
        }
        else
        {
            for (int i = 0; i < lightPlatform.Count; i++)
            {
                lightPlatform[i].GetComponent<Renderer>().material = _material;
            }
            lightFeedback.SetActive(false);
        }
    }
    public void OpenDors()
    {
        if (isGetLight)
        {
            door.gameObject.SetActive(false);
            lightFeedback.SetActive(false);
        }
    }
}

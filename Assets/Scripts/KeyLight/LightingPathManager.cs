using NUnit.Framework;
using System.Collections.Generic;
using System.Security;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;

public class LightingPathManager : MonoBehaviour
{
    #region variables

    [SerializeField] private GameObject door;
    [SerializeField] private GameObject lightFeedback;
    [SerializeField] private List<GameObject> lightPlatform = new List<GameObject>();

    [Header("Materials")] [SerializeField] private Material _material;
    [SerializeField] private Material _emissiveMaterial;

    [Tooltip("esto dice si el player tiene la luz o no")]public bool isGetLight;

    [Tooltip("si el trayecto no tiene plataformas que iluminar hay que poner a true esta opcion")][SerializeField] private bool _isntPlatforms;

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
        if (!_isntPlatforms)
        {
            lightPlatform.Clear();

            foreach (Transform child in this.transform)
            {
                lightPlatform.Add(child.transform.GetChild(0).gameObject);
            }
        }
    }

    private void whenLightOn()
    {
        if (isGetLight)
        {
            if(!_isntPlatforms)
            {
                for (int i = 0; i < lightPlatform.Count; i++)
                {
                    lightPlatform[i].GetComponent<Renderer>().material = _emissiveMaterial;
                }
            }
                
            lightFeedback.SetActive(true);
        }
        else
        {
            if (!_isntPlatforms)
            {
                for (int i = 0; i < lightPlatform.Count; i++)
                {
                    lightPlatform[i].GetComponent<Renderer>().material = _material;
                }
            }

            lightFeedback.SetActive(false);
        }
        
    }

    public void OpenDors()
    {
        if (isGetLight)
        {
            //door.GetComponent<Animator>().SetBool("isOpen", true);
            //door.GetComponent<BoxCollider>().enabled = false;
            //GetComponent<StudioEventEmitter>().Play();

            door.SetActive(false);
            lightFeedback.SetActive(false);
        }
    }
}
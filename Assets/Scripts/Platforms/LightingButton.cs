using NUnit.Framework;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LightingButton : MonoBehaviour
{
    #region variables
    [SerializeField] private GameObject dor;

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
    }
    #endregion
    public void OpenDors()
    {
        if (isGetLight)
        {
            dor.gameObject.SetActive(false);
        }
    }
}

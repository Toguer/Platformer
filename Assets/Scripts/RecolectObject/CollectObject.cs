using System;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;

public class CollectObject : MonoBehaviour
{
    [SerializeField] private string objectName;
    [SerializeField] private int cointsValue;
    [SerializeField] private bool unicObj;

    public static event Action<int> OnUpdateCoinsScore = delegate { };
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            ComprobeObject();
        }
    }
    private void ComprobeObject()
    {
        switch(objectName)
        {
            case "coins":
                OnUpdateCoinsScore.Invoke(cointsValue);

                break;
            case "key":
                break;
        }
    }
}

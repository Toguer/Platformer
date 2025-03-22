using System.Collections.Generic;
using System;
using UnityEngine;

public class CollectObject : MonoBehaviour
{
    [SerializeField] private string objectName;
    [SerializeField] private bool unicObj;

    [Header("If is coin")]
    [SerializeField] private int cointsValue;

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
        if(unicObj)
        {
            List<string> objList = GameManager.Instance.GetObjList();

            objList.Add(objectName);
            GameManager.Instance.SetObjList(objList);
        }
        else
        {
            OnUpdateCoinsScore.Invoke(cointsValue); 
        }
        this.gameObject.SetActive(false);
    }
}

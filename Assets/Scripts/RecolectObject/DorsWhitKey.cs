using UnityEngine;
using System.Collections.Generic;

public class DorsWhitKey : MonoBehaviour
{
    [SerializeField] private string keyName;

    private List<string> _objCollected = new List<string>();
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            _objCollected = GameManager.Instance.GetObjList();
            for(int i = 0; i < _objCollected.Count; i++)
            {
                if (_objCollected[i] == keyName)
                {
                    this.gameObject.SetActive(false);
                }
            }
        }
    }
}

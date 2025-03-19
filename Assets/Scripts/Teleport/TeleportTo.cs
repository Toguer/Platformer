using System.Collections;
using UnityEngine;

public class TeleportTo : MonoBehaviour
{
    [SerializeField] private GameObject pointToGo;
    void Start()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<CharacterController>().enabled = false;
           
            other.gameObject.transform.position = pointToGo.transform.position;
            pointToGo.GetComponent<Collider>().enabled = false;
            StartCoroutine(_DesactiveCollider());

            other.gameObject.GetComponent<CharacterController>().enabled = true;
        }
    }

    private IEnumerator _DesactiveCollider()
    {
        yield return new WaitForSeconds(1f);
        pointToGo.GetComponent<Collider>().enabled = true;
    }
}

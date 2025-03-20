using System.Collections;
using UnityEngine;

public class TeleportTo : MonoBehaviour
{
    [SerializeField] private GameObject pointToGo;
    [SerializeField] private bool isFirstPoint;
    void Start()
    {
        if (isFirstPoint)
        {
            GetComponent<Collider>().isTrigger = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponent<CharacterController>().enabled = false;
           
            other.gameObject.transform.position = pointToGo.transform.position;
            pointToGo.GetComponent<Collider>().enabled = false;
            if (!isFirstPoint)
            {
                if (pointToGo.GetComponent<TeleportTo>())
                {
                    pointToGo.GetComponent<Collider>().isTrigger = true;
                } 
            }
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

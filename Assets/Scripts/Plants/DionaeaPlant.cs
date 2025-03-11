using System.Collections;
using UnityEngine;

public class DionaeaPlant : MonoBehaviour
{
    [Header("Plants Parts")]
    [SerializeField] GameObject boca1;
    [SerializeField] GameObject boca2;

    [Header("Variables")]
    [SerializeField] private float timeToClose;
    [SerializeField] private float timeToOpen;
  
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (!other.gameObject.GetComponent<CharacterController>().isGrounded)
            {
                other.GetComponent<SpawnPointManager>().Onhurt();
                Debug.Log("grounded");
            }
            else
            {
                StartCoroutine(closePlant());
            }
        }
    }

    private IEnumerator closePlant()
    {
        yield return new WaitForSeconds(timeToClose);
        boca1.GetComponent<Animator>().SetBool("isClosed", true);
        boca2.GetComponent<Animator>().SetBool("isClosed", true);
        GetComponent<BoxCollider>().enabled = false;
        StartCoroutine(openPlant());
    }
    private IEnumerator openPlant()
    {
        yield return new WaitForSeconds(timeToOpen);
        boca1.GetComponent<Animator>().SetBool("isClosed", false);
        boca2.GetComponent<Animator>().SetBool("isClosed", false);
        GetComponent<BoxCollider>().enabled = true;
    }
}

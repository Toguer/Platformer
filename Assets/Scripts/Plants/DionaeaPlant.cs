using System.Collections;
using UnityEngine;

public class DionaeaPlant : MonoBehaviour
{
    [Header("Plants Parts")]
    [SerializeField] GameObject boca1;
    [SerializeField] GameObject boca2;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
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
        yield return new WaitForSeconds(.3f);
        boca1.GetComponent<Animator>().SetBool("isClosed", true);
        boca2.GetComponent<Animator>().SetBool("isClosed", true);
        GetComponent<BoxCollider>().enabled = false;
        StartCoroutine(openPlant());
    }
    private IEnumerator openPlant()
    {
        yield return new WaitForSeconds(3f);
        boca1.GetComponent<Animator>().SetBool("isClosed", false);
        boca2.GetComponent<Animator>().SetBool("isClosed", false);
        GetComponent<BoxCollider>().enabled = true;
    }
}

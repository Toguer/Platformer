using UnityEngine;

public class DroseraCapensis : MonoBehaviour
{
    [SerializeField] private float distanceArea;

    // esto sera remplazado por la animacion de la planta
    [SerializeField] private GameObject estadoA;
    [SerializeField] private GameObject estadoB;
    void Start()
    {
        SphereCollider sphere = GetComponent<SphereCollider>();
        sphere.radius = distanceArea;
        sphere.isTrigger = true;
        //this.GetComponent<BoxCollider>().isTrigger = true;
        //this.GetComponent<BoxCollider>().isTrigger = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //animacion
            //this.GetComponent<BoxCollider>().enabled = true;
            estadoB.SetActive(false);
            estadoA.SetActive(true);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            //animacion
            //this.GetComponent<BoxCollider>().enabled = false;
            estadoA.SetActive(false);
            estadoB.SetActive(true);
            Debug.Log("salgo");
        }
    }
}

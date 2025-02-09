using UnityEngine;

public class MimosaPlant : MonoBehaviour
{
    
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            // aqui haria la animacion de la flor recogiendose
        }
    }
}

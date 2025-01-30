using UnityEngine;

public class DamageBehaviour : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<SpawnPointManager>().Onhurt();
        }
    }
}

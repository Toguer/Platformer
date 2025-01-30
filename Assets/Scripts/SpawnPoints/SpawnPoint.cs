using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<SpawnPointManager>().SetLastpoint(transform.position);
        }
    }
}

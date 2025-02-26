using UnityEngine;

public class DamageBehaviour : MonoBehaviour
{
    [SerializeField] private int damage;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            other.GetComponent<SpawnPointManager>().Onhurt();

        }
    }
    public void Damage(GameObject player)
    {
        player.GetComponent<SpawnPointManager>().Onhurt();
    }
}

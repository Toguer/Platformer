using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPointManager : MonoBehaviour
{
    [SerializeField] GameObject firstSpawnPoint;
    private Vector3 lastSpawnPoint;
    public GameObject player;
    public void SetLastpoint(Vector3 LCP)
    {
        lastSpawnPoint = LCP;
    }
    void Start()
    {
        lastSpawnPoint = firstSpawnPoint.transform.position;
    }

    public void Onhurt()
    {
        StartCoroutine(DelayGravity());

    }

    IEnumerator DelayGravity()
    {
        // animacion de morir
        yield return new WaitForSeconds(0.1f);

        player.GetComponent<CharacterController>().enabled = false;
        // Corregir la posici�n
        player.transform.position = lastSpawnPoint;
        Debug.Log("Posici�n restablecida a: " + lastSpawnPoint);
        player.GetComponent<CharacterController>().enabled = true;
    }
}

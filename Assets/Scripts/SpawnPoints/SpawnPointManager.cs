using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SpawnPointManager : MonoBehaviour
{
    [SerializeField] GameObject firstSpawnPoint;
    private GameObject lastSpawnPoint;
    private Vector3 lastSpawnPointVector;
    public GameObject player;
    public void SetLastpoint(GameObject LCP, Vector3 LCV)
    {
        lastSpawnPoint = LCP;
        lastSpawnPointVector = LCV;
    }
    void Start()
    {
        lastSpawnPointVector = firstSpawnPoint.transform.position;
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
        player.transform.position = lastSpawnPointVector;
        player.transform.rotation = Quaternion.LookRotation(lastSpawnPoint.transform.forward);

        player.GetComponent<CharacterController>().enabled = true;
    }
}

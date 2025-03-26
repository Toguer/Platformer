using System.Collections;
using UnityEngine;

public class TeleportTo : Interactable
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
    private IEnumerator _DesactiveCollider()
    {
        yield return new WaitForSeconds(1f);
        pointToGo.GetComponent<Collider>().enabled = true;
    }

    public override void Interact(PlayerController player)
    {
        player.gameObject.GetComponent<CharacterController>().enabled = false;

        player.gameObject.transform.position = pointToGo.transform.position;
        pointToGo.GetComponent<Collider>().enabled = false;
        if (!isFirstPoint)
        {
            if (pointToGo.GetComponent<TeleportTo>())
            {
                pointToGo.GetComponent<Collider>().isTrigger = true;
            }
        }
        StartCoroutine(_DesactiveCollider());

        player.gameObject.GetComponent<CharacterController>().enabled = true;
    }
}

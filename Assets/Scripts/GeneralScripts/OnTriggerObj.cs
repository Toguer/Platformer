using UnityEngine;
using UnityEngine.Events;

public class OnTriggerObj : MonoBehaviour
{
    [SerializeField] private UnityEvent onTouchPlayer;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            onTouchPlayer.Invoke();
        }
    }
}

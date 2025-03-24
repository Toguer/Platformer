using UnityEngine;

public class Interactable : MonoBehaviour
{
    public virtual void Interact()
    {
        Debug.Log("Interactuando con: " + gameObject.name);
    }
}

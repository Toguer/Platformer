using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputSystem_Actions _playerInput;

    private Interactable _interactable; 
    void Awake()
    {
        _playerInput = new InputSystem_Actions();

        _playerInput.Player.Interact.started += onInteract;
    }
    void Update()
    {
        Shader.SetGlobalVector("Player", transform.position);
    }
    
    void onInteract(InputAction.CallbackContext context)
    {
        _interactable.Interact();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            _interactable = other.GetComponent<Interactable>();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            _interactable = null;
        }
    }
}

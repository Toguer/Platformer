using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public InputSystem_Actions _playerInput;

    private Interactable _interactable;

    [SerializeField] private GameObject _canvasE;
    void Awake()
    {
        _playerInput = new InputSystem_Actions();
        _playerInput.Player.Interact.performed += onInteract;
    }
    void Update()
    {
        Shader.SetGlobalVector("Player", transform.position);
    }
    
    void onInteract(InputAction.CallbackContext context)
    {
        Debug.Log("Pressed e");
        _interactable.Interact();
    }
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            if(_interactable == null)
            {
                _interactable = other.GetComponent<Interactable>();
                _canvasE.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Interactable"))
        {
            _interactable = null;
            _canvasE.SetActive(false);
        }
    }
}

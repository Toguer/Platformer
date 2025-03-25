using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    private InputSystem_Actions _playerInput;

    private Interactable _interactable;

    [Header("Canvas")]
    [SerializeField] private GameObject _canvasE;

    void Start()
    {
        _playerInput = GetComponent<PlayerStateMachine>().PlayerInput;
        print("Start on PlayerController");
        _playerInput.Player.Interact.started += onInteract;
    }

    void Update()
    {
        Shader.SetGlobalVector("Player", transform.position);
    }

    void onInteract(InputAction.CallbackContext context)
    {
        if (_interactable != null)
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
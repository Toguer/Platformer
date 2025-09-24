using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;
using static UnityEngine.UI.Image;

public class PlayerController : MonoBehaviour
{
    private InputSystem_Actions _playerInput;

    private Interactable _interactable;

    [Header("RayCast Variables")]
    [SerializeField] private float raycastDistance = 10f;
    [SerializeField] private LayerMask layerMask;
    [SerializeField] private Vector3 rayOrigin;


    [Header("Canvas")]
    [SerializeField] private GameObject _canvasE;

    void Start()
    {
        _playerInput = GetComponent<RbPlayerStateMachine>().PlayerInput;
        print("Start on PlayerController");
        _playerInput.Player.Interact.started += onInteract;
    }

    void Update()
    {
        Shader.SetGlobalVector("Player", transform.position);

        
        Vector3 rayDirection = transform.forward;

        RaycastHit hit;
        bool hasHit = Physics.Raycast(rayOrigin, rayDirection, out hit, raycastDistance, layerMask);


    }

    void onInteract(InputAction.CallbackContext context)
    {
        if (_interactable != null)
            _interactable.Interact(this);
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
    void OnDrawGizmosSelected()
    {

    }
}
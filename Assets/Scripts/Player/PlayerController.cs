using Unity.VisualScripting;
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
    [SerializeField] private LayerMask layerMask = 0;
    [SerializeField] private Vector3 rayOrigin;
    [SerializeField] private float alturaDeOrigen = 1.5f;
    private GameObject currentTarget;


    [Header("Canvas Interactive")]
    [SerializeField] private GameObject _canvasE;
    [SerializeField] private bool ifRayCast = true;

    [Header("Light")]
    [Tooltip("Hay que añadir el script de la zona")] private LightingPathManager lightingPathManager;    

    void Start()
    {
        _playerInput = GetComponent<RbPlayerStateMachine>().PlayerInput;
        print("Start on PlayerController");
        _playerInput.Player.Interact.started += onInteract;
    }
    public void SetLightManager(LightingPathManager lightingPathManager)
    {
        this.lightingPathManager = lightingPathManager;
    }

    void Update()
    {
        Shader.SetGlobalVector("Player", transform.position);
        if (ifRayCast)
        {
            Vector3 basePosition = transform.position;
            Vector3 rayOrigin = basePosition + Vector3.up * alturaDeOrigen;
        
            Vector3 rayDirection = transform.forward;

            RaycastHit hit;

            bool hasHit = Physics.Raycast(rayOrigin, rayDirection, out hit, raycastDistance, layerMask);

            if (hasHit)
            {
                if (hit.collider.gameObject.CompareTag("Interactable"))
                {
                    currentTarget = hit.collider.gameObject;
                    Debug.Log($"Mirando a: {currentTarget.name} a una distancia de: {hit.distance:F2}");
                    if (_interactable == null)
                    {
                        _interactable = currentTarget.GetComponent<Interactable>();
                        _canvasE.SetActive(true);
                    }
                }
                else
                {
                    currentTarget = null;
                    _interactable = null;
                    _canvasE.SetActive(false);
                }
            }
            else
            {
                currentTarget = null;
                _interactable = null;
                _canvasE.SetActive(false);
            }

            Color rayColor = hasHit ? Color.red : Color.green;
            Debug.DrawRay(rayOrigin, rayDirection * raycastDistance, rayColor);
        }
        
    }

    void onInteract(InputAction.CallbackContext context)
    {
        //solo se cumplira cuando el player pulse a la e
        //comprobamos si el objeto interactable esta vacio, es decir, que el player solo puede detectar uno a la vez, para que se interactue solo con 1
        if (_interactable != null)
            //cuando se pulsa, se manda al player(this) y se cumplira lo que este en la funcion interact del obj interactuable asignado
            _interactable.Interact(this);
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log(other.name);
        if (other.gameObject.CompareTag("Interactable"))
        {
            if(_interactable == null)
            {
                _interactable = other.GetComponent<Interactable>();
                _canvasE.SetActive(true);
            }   
        }
        else if (other.gameObject.CompareTag("ground"))
        {
            Debug.Log("TocansoSUelo");
            if(lightingPathManager != null)
            {
                lightingPathManager.SetisGetLight(false);
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
    
    public void saveLight(LightingPathManager lpm)
    {
        lightingPathManager = lpm;
    }
}
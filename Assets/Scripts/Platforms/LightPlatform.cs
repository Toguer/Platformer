using UnityEngine;

public class LightPlatform : MonoBehaviour
{
    [SerializeField] private Material _newMaterial;
    private Material _oldMaterial;
    private MeshRenderer _renderer;

    public MeshRenderer GetRenderer() { return _renderer; }
    void Start()
    {
        _renderer = GetComponent<MeshRenderer>();
        _oldMaterial = _renderer.material;
    }
    /*private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        { 
            if (_renderer != null)
            {
                _renderer.material = _newMaterial;
            }   
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if (_renderer != null)
            {
                _renderer.material = _oldMaterial;
            }
        }
    }*/
}

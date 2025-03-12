using UnityEngine;

public class LightPlatform : MonoBehaviour
{
    [SerializeField] private Material _newMaterial;
    private Material _oldMaterial;
    private MeshRenderer _renderer;

    private GameObject pather;
    private bool isLighting;
    public MeshRenderer GetRenderer() { return _renderer; }
    public bool GetIsLighting() { return isLighting; }
    void Start()
    {
        pather = gameObject.transform.parent.gameObject;
        _renderer = GetComponent<MeshRenderer>();
        _oldMaterial = _renderer.material;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            if(pather.GetComponent<LightingPlatformsManager>().GetisLight())
            {
                _renderer = GetComponent<MeshRenderer>();

                if (_renderer != null)
                {
                    _renderer.material = _newMaterial;
                    isLighting = true;
                }
            }
        }    
    }
    public void RestartMaterial()
    {
        _renderer.material = _oldMaterial;
    }
}

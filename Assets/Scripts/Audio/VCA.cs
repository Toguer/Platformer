using UnityEngine;
using UnityEngine.Serialization;

public class VCA : MonoBehaviour
{
    private FMOD.Studio.VCA _vca;
    [SerializeField] [Range(-80f, 10f)]
    private float _vcaVolume;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _vca = FMODUnity.RuntimeManager.GetVCA("vca:/Music");
    }

    // Update is called once per frame
    void Update()
    {
        _vca.setVolume(DecibelToLinear(_vcaVolume));
    }

    private float DecibelToLinear(float dB)
    {
        float linear = Mathf.Pow(10.0f, dB / 20f);
        return linear;
    }
}

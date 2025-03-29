using UnityEngine;

public class Bus : MonoBehaviour
{
    private FMOD.Studio.Bus _bus;
    [SerializeField] [Range(-80f, 10f)]
    private float _busVolume;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _bus = FMODUnity.RuntimeManager.GetBus("bus:/Bus");
    }

    // Update is called once per frame
    void Update()
    {
        _bus.setVolume(DecibelToLinear(_busVolume));
    }

    private float DecibelToLinear(float dB)
    {
        float linear = Mathf.Pow(10.0f, dB / 20f);
        return linear;
    }
}

using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    private VCA _vca;

    [SerializeField] private Slider _sliderMusic;
    [SerializeField] private Slider _sliderSfx;
    [SerializeField] private Slider _sliderUi;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _vca = GetComponent<VCA>();
    }

    public void OnValueChanged()
    {
        float normalizedValue = Mathf.Clamp(_sliderMusic.value / 100f, 0.001f, 1f);
        _vca.MusicVolume = Mathf.Log10(normalizedValue) * 20;
        normalizedValue = Mathf.Clamp(_sliderSfx.value / 100f, 0.001f, 1f);
        _vca.SfxVolume = Mathf.Log10(normalizedValue) * 20;
        normalizedValue = Mathf.Clamp(_sliderUi.value / 100f, 0.001f, 1f);
        _vca.UiVolume = Mathf.Log10(normalizedValue) * 20;
    }
}
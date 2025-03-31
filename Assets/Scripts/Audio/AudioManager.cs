using Singleton;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : PersistentSingleton<AudioManager>
{
    private VCA _vca;

    [SerializeField] private Slider _sliderMusic;

    public Slider SliderMusic
    {
        get => _sliderMusic;
        set => _sliderMusic = value;
    }

    public Slider SliderSfx
    {
        get => _sliderSfx;
        set => _sliderSfx = value;
    }

    public Slider SliderUi
    {
        get => _sliderUi;
        set => _sliderUi = value;
    }

    [SerializeField] private Slider _sliderSfx;
    [SerializeField] private Slider _sliderUi;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _vca = GetComponent<VCA>();
        print(_vca);
        float linearValue = Mathf.Pow(10f, _vca.MusicVolume / 20f);
        _sliderMusic.value = linearValue;
        linearValue = Mathf.Pow(10f, _vca.SfxVolume / 20f);
        _sliderSfx.value = linearValue;
        linearValue = Mathf.Pow(10f, _vca.UiVolume / 20f);
        _sliderUi.value = linearValue;
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
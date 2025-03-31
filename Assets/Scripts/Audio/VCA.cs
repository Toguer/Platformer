using UnityEngine;
using UnityEngine.Serialization;

public class VCA : MonoBehaviour
{
    #region variables

    private FMOD.Studio.VCA _musicVca;
    [SerializeField] [Range(-80f, 10f)] private float _musicVolume;
    private FMOD.Studio.VCA _sfxVca;
    [SerializeField] [Range(-80f, 10f)] private float _sfxVolume;
    private FMOD.Studio.VCA _uiVca;
    [SerializeField] [Range(-80f, 10f)] private float _uiVolume;

    [Tooltip("El formato es \"vca:/Music\"")] [SerializeField]
    private string _musicPath;

    [SerializeField] private string _sfxPath;
    [SerializeField] private string _uiPath;

    #endregion

    #region getters & setters

    public float MusicVolume
    {
        get { return _musicVolume; }
        set
        {
            _musicVolume = value;
            OnChange();
        }
    }

    public float SfxVolume
    {
        get { return _sfxVolume; }
        set
        {
            _sfxVolume = value;
            OnChange();
        }
    }

    public float UiVolume
    {
        get { return _uiVolume; }
        set { _uiVolume = value;
            OnChange();
        }
    }

    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DontDestroyOnLoad(this);
        _musicVca = FMODUnity.RuntimeManager.GetVCA(_musicPath);
        _sfxVca = FMODUnity.RuntimeManager.GetVCA(_sfxPath);
        _uiVca = FMODUnity.RuntimeManager.GetVCA(_uiPath);
    }

    private void OnChange()
    {
        _musicVca.setVolume(DecibelToLinear(_musicVolume));
        _sfxVca.setVolume(DecibelToLinear(_sfxVolume));
        _uiVca.setVolume(DecibelToLinear(_uiVolume));
    }
    

    private float DecibelToLinear(float dB)
    {
        float linear = Mathf.Pow(10.0f, dB / 20f);
        return linear;
    }
}
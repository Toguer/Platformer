using UnityEngine;
using FMODUnity;

public class PlayGenericOneShot : MonoBehaviour
{
    [SerializeField] private EventReference _sound;

    public void PlaySoundEvent()
    {
        if (!_sound.IsNull)
        {
            RuntimeManager.PlayOneShot(_sound);
        }
    }
    
    
    
}

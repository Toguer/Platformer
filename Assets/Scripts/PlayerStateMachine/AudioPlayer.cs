using System;
using FMODUnity;
using UnityEngine;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private EventReference _steps;
    [SerializeField] private EventReference _jump;
    [SerializeField] private EventReference _jetpack;

    private StudioEventEmitter _eventEmitter;

    private void Start()
    {
        if (!TryGetComponent<StudioEventEmitter>(out _eventEmitter))
        {
            _eventEmitter = gameObject.AddComponent<StudioEventEmitter>();
        }
    }

    public void PlaySteps()
    {
        _eventEmitter.EventReference = _steps;
        _eventEmitter.Play();
    }

    public void PlayJump()
    {
        _eventEmitter.EventReference = _jump;
        _eventEmitter.Play();
    }

    public void PlayJetpack()
    {
        print("Jetpack sound");
        _eventEmitter.EventReference = _steps;
        _eventEmitter.Play();
    }

    public void StopSteps()
    {
        _eventEmitter.EventReference = _steps;
        _eventEmitter.Stop();
    }
}
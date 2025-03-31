using System;
using FMODUnity;
using Unity.VisualScripting;
using UnityEngine;
using STOP_MODE = FMOD.Studio.STOP_MODE;

public class AudioPlayer : MonoBehaviour
{
    [SerializeField] private EventReference _steps;
    [SerializeField] private EventReference _jump;
    [SerializeField] private EventReference _jetpack;

    private FMOD.Studio.EventInstance _stepSounds;
    private FMOD.Studio.EventInstance _jumpSounds;
    private FMOD.Studio.EventInstance _jetpackSounds;
    private StudioEventEmitter _eventEmitter;

    private void Start()
    {
        if (!_stepSounds.IsUnityNull())
        {
            _stepSounds = FMODUnity.RuntimeManager.CreateInstance(_steps);
        }

        if (!_jumpSounds.IsUnityNull())
        {
            _jumpSounds = FMODUnity.RuntimeManager.CreateInstance(_jump);
        }

        if (!_jetpackSounds.IsUnityNull())
        {
            _jetpackSounds = FMODUnity.RuntimeManager.CreateInstance(_jetpack);
        }

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
        print("Jump Sound");
        _jumpSounds.start();
        //_eventEmitter.EventReference = _jump;
        //_eventEmitter.Play();
    }

    public void PlayJetpack()
    {
        print("Jetpack sound");
        _jetpackSounds.start();
        //_eventEmitter.EventReference = _steps;
        //_eventEmitter.Play();
    }

    public void StopSteps()
    {
        print("Stop steps");


        _eventEmitter.EventReference = _steps;
        _eventEmitter.Stop();
    }
}
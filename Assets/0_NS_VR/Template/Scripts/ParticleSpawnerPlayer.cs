using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class ParticleSpawnerPlayer : MonoBehaviour
{
    [SerializeField] WaterFlowCustom _waterFlowCustom;

    public bool IsGrabbed = false;

    bool _isLeftGrabbing = false;
    bool _isRightGrabbing = false;
    bool _isLeftGenerating => _isLeftGrabbing && VRInputManager.Instance.IsTriggerPressed(ControllerSide.Left, true);
    bool _isRightGenerating => _isRightGrabbing && VRInputManager.Instance.IsTriggerPressed(ControllerSide.Right, true);


    float _vibrateInterval = 0.1f;

    float _vibrateTimer = 0;

    // Start is called before the first frame update
    void Start()
    {
        _isLeftGrabbing = false;
        _isRightGrabbing = false;
    }

    // Update is called once per frame
    void Update()
    {
        if(_isLeftGrabbing && _isRightGrabbing) {
            return;
        }
        if( (_isLeftGrabbing && VRInputManager.Instance.IsTriggerPressed(ControllerSide.Left, true)) || (_isRightGrabbing && VRInputManager.Instance.IsTriggerPressed(ControllerSide.Right, true)))
        {
            Play();
        }
        
        if((_isLeftGrabbing && !VRInputManager.Instance.IsTriggerPressed(ControllerSide.Left, true)) || (_isRightGrabbing && !VRInputManager.Instance.IsTriggerPressed(ControllerSide.Right, true))) {
            Stop();
        }

        if(_isLeftGenerating || _isRightGenerating) {
            _vibrateTimer += Time.deltaTime;
            if(_vibrateTimer >= _vibrateInterval) {
                if(_isLeftGenerating) VibrationController.Instance.Play("click", ControllerSide.Left);
                if(_isRightGenerating) VibrationController.Instance.Play("click", ControllerSide.Right);
                _vibrateTimer = 0;
            }
        }

        
    }

    public void Play() {
        // foreach(var spawner in _particleSpawners){
        //     spawner.Play();
        // }
        // _particleSystem.Play();
        Debug.Log("Play");
        _waterFlowCustom.StartGenerating();
    }

    public void Stop() {
        // foreach(var spawner in _particleSpawners){
        //     spawner.Stop();
        // }
        // _particleSystem.Stop();
        Debug.Log("Stop");
        _waterFlowCustom.StopGenerating();
    }

    public void OnGrab(SelectEnterEventArgs args) {
        var interactor = args.interactorObject as XRBaseInteractor;
        var handType = interactor.handedness;
        var controllerSide = handType == InteractorHandedness.Left ? ControllerSide.Left : ControllerSide.Right;
        if(controllerSide == ControllerSide.Left) {
            _isLeftGrabbing = true;
        } else if(controllerSide == ControllerSide.Right) {
            _isRightGrabbing = true;
        }
        Debug.Log("OnGrab: " + controllerSide);
    }

    public void OnUngrab(SelectExitEventArgs args) {
        var interactor = args.interactorObject as XRBaseInteractor;
        var handType = interactor.handedness;
        var controllerSide = handType == InteractorHandedness.Left ? ControllerSide.Left : ControllerSide.Right;
        if(controllerSide == ControllerSide.Left) {
            _isLeftGrabbing = false;
        } else if(controllerSide == ControllerSide.Right) {
            _isRightGrabbing = false;
        }
        Debug.Log("OnUngrab: " + controllerSide);
    }
}

public enum Hand {
    None,
    Left,
    Right,
}
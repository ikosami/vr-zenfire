using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Ev;

public class Grabbable : MonoBehaviour
{
    [SerializeField] bool _hasPose;
    [SerializeField] string _poseName;
    [SerializeField] Transform _grabbedPoint_L;
    [SerializeField] Transform _grabbedPoint_R;
    [SerializeField] bool _grabMirror;
    [SerializeField] Transform _rotateObject;
    [SerializeField] XRGrabInteractable _grabInteractable;

    Vector3 _originalPosition_L;
    Quaternion _originalRotation_L;
    Vector3 _originalPosition_R;
    Quaternion _originalRotation_R;

    Quaternion _originalRotation;
    Quaternion _mirrorRotation;

    void Start()
    {
        if (_grabbedPoint_L != null)
        {
            _originalPosition_L = _grabbedPoint_L.localPosition;
            _originalRotation_L = _grabbedPoint_L.localRotation;
        }
        if (_grabbedPoint_R != null)
        {
            _originalPosition_R = _grabbedPoint_R.localPosition;
            _originalRotation_R = _grabbedPoint_R.localRotation;
        }
        if (_rotateObject != null)
        {
            _originalRotation = _rotateObject.localRotation;
            _mirrorRotation = _rotateObject.localRotation * Quaternion.Euler(0, 180, 0);
        }
    }

    public void OnHover(HoverEnterEventArgs args) {
        // if (_grabMirror)
        // {
        //     var interactor = args.interactorObject as XRBaseInteractor;
        //     var handType = interactor.handedness;
        //     var controllerSide = handType == InteractorHandedness.Left ? ControllerSide.Left : ControllerSide.Right;
        //     _grabInteractable.attachTransform = controllerSide == ControllerSide.Left ? _grabbedPoint_L : _grabbedPoint_R;
        // }else {
        //     _grabInteractable.attachTransform = _grabbedPoint_L;
        // }
    }

    public void OnGrab(SelectEnterEventArgs args) {
        var interactor = args.interactorObject as XRBaseInteractor;
        var handType = interactor.handedness;
        var controllerSide = handType == InteractorHandedness.Left ? ControllerSide.Left : ControllerSide.Right;

        if ( _rotateObject != null)
        {
            _rotateObject.localRotation = controllerSide == ControllerSide.Left ? _originalRotation : _mirrorRotation;
        }

        if(_hasPose) {
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, new Ev.Events.HandPoseChange() { PoseName = _poseName, ControllerSide = controllerSide });
        }
    }

    public void OnRelease(SelectExitEventArgs args) {
        if(_hasPose) {
            var interactor = args.interactorObject as XRBaseInteractor;
            var handType = interactor.handedness;
            var controllerSide = handType == InteractorHandedness.Left ? ControllerSide.Left : ControllerSide.Right;
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_RESET, new Ev.Events.HandPoseReset(){ControllerSide = controllerSide});
        }
    }
}

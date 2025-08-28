using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Ev;

public class ControllerHandPose : MonoBehaviour
{
    [SerializeField] ControllerSide _controllerSide;

    [SerializeField] string _triggerPoseName;

    bool _prevPressed = false;

    Dictionary<VRInputType, bool> _prevPressedStates = new Dictionary<VRInputType, bool>();
    // Dictionary<VRInputType, bool> _rightPrevPressed = new Dictionary<VRInputType, bool>();
    // Start is called before the first frame update
    void Start()
    {
        _prevPressedStates = new Dictionary<VRInputType, bool>() {
            { VRInputType.StickTouch, false },
            { VRInputType.ButtonPrimary, false },
            { VRInputType.ButtonSecondary, false },
            { VRInputType.Trigger, false },
            { VRInputType.Grip, false },
            { VRInputType.Menu, false },
        };
    }

    // Update is called once per frame
    void Update()
    {
        if(!_prevPressedStates[VRInputType.StickTouch] && VRInputManager.Instance.StickTouchPressed(_controllerSide)) {
            _prevPressedStates[VRInputType.StickTouch] = true;
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, new Ev.Events.HandPoseChange() { PoseName = _triggerPoseName, ControllerSide = _controllerSide, HandPartType = HandPartType.Thumb});
        }else if(_prevPressedStates[VRInputType.StickTouch] && !VRInputManager.Instance.StickTouchPressed(_controllerSide)) {
            _prevPressedStates[VRInputType.StickTouch] = false;
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_RESET, new Ev.Events.HandPoseReset() { ControllerSide = _controllerSide, HandPartType = HandPartType.Thumb});
        }

        if(!_prevPressedStates[VRInputType.ButtonSecondary] && VRInputManager.Instance.ButtonSecondaryPressed(_controllerSide)) {
            _prevPressedStates[VRInputType.ButtonSecondary] = true;
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, new Ev.Events.HandPoseChange() { PoseName = _triggerPoseName, ControllerSide = _controllerSide, HandPartType = HandPartType.Thumb | HandPartType.Index | HandPartType.Ring | HandPartType.Little});
        }else if(_prevPressedStates[VRInputType.ButtonSecondary] && !VRInputManager.Instance.ButtonSecondaryPressed(_controllerSide)) {
            _prevPressedStates[VRInputType.ButtonSecondary] = false;
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_RESET, new Ev.Events.HandPoseReset() { ControllerSide = _controllerSide, HandPartType = HandPartType.Thumb | HandPartType.Index | HandPartType.Ring | HandPartType.Little});
        }

        if(!_prevPressedStates[VRInputType.Trigger] && VRInputManager.Instance.TriggerValue(_controllerSide) > 0.5f) {
            _prevPressedStates[VRInputType.Trigger] = true;
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, new Ev.Events.HandPoseChange() { PoseName = _triggerPoseName, ControllerSide = _controllerSide, HandPartType = HandPartType.Index});
        }else if(_prevPressedStates[VRInputType.Trigger] && VRInputManager.Instance.TriggerValue(_controllerSide) < 0.5f) {
            _prevPressedStates[VRInputType.Trigger] = false;
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_RESET, new Ev.Events.HandPoseReset() { ControllerSide = _controllerSide, HandPartType = HandPartType.Index});
        }

        if(!_prevPressedStates[VRInputType.Grip] && VRInputManager.Instance.GripValue(_controllerSide) > 0.5f) {
            _prevPressedStates[VRInputType.Grip] = true;
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, new Ev.Events.HandPoseChange() { PoseName = _triggerPoseName, ControllerSide = _controllerSide, HandPartType = HandPartType.Middle | HandPartType.Ring | HandPartType.Little });
        }else if(_prevPressedStates[VRInputType.Grip] && VRInputManager.Instance.GripValue(_controllerSide) < 0.5f) {
            _prevPressedStates[VRInputType.Grip] = false;
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_RESET, new Ev.Events.HandPoseReset() { ControllerSide = _controllerSide, HandPartType = HandPartType.Middle | HandPartType.Ring | HandPartType.Little});
        }

        // if(!_prevPressed && _triggerAction.action.IsPressed()) {
        //     _prevPressed = true;
        //     EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, new Ev.Events.HandPoseChange() { PoseName = _triggerPoseName, ControllerSide = _controllerSide });
        // }
        // else if(_prevPressed && !_triggerAction.action.IsPressed()) {
        //     _prevPressed = false;
        //     EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_RESET, new Ev.Events.HandPoseReset() { ControllerSide = _controllerSide });
        // }

    }
}

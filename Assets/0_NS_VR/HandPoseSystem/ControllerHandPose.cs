using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Ev;

public class ControllerHandPose : MonoBehaviour
{
    [SerializeField] ControllerSide _controllerSide;
    [SerializeField] InputActionReference _triggerAction;

    [SerializeField] string _triggerPoseName;

    bool _prevPressed = false;
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if(!_prevPressed && _triggerAction.action.IsPressed()) {
            _prevPressed = true;
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, new Ev.Events.HandPoseChange() { PoseName = _triggerPoseName, ControllerSide = _controllerSide });
        }
        else if(_prevPressed && !_triggerAction.action.IsPressed()) {
            _prevPressed = false;
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_RESET, new Ev.Events.HandPoseReset() { ControllerSide = _controllerSide });
        }

    }
}

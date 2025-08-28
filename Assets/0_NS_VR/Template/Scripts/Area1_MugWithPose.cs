using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ev;
using Ev.Events;

public class Area1_MugWithPose : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
{
    [SerializeField] private string grabPoseName = "GrabMug";
    [SerializeField] private string defaultPoseName = "Default";
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        var controllerSide = args.interactorObject.transform.CompareTag("RightHand") ? ControllerSide.Right : ControllerSide.Left;
        EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, 
            new Ev.Events.HandPoseChange() { 
                PoseName = grabPoseName,
                ControllerSide = controllerSide,
                HandPartType = HandPartType.All
            });
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        var controllerSide = args.interactorObject.transform.CompareTag("RightHand") ? ControllerSide.Right : ControllerSide.Left;
        EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_RESET,
            new Ev.Events.HandPoseReset() {
                ControllerSide = controllerSide,
                HandPartType = HandPartType.All
            });
    }
}

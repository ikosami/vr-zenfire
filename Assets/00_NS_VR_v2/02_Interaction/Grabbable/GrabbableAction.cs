using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class GrabbableAction : MonoBehaviour
{

    public bool isGrab;
    protected ControllerSide side = ControllerSide.Left;
    public virtual void OnGrab(SelectEnterEventArgs args, ControllerSide controllerSide)
    {
        side = controllerSide;
        isGrab = true;
    }
    public virtual void OnRelease(SelectExitEventArgs args, ControllerSide controllerSide)
    {
        side = controllerSide;
        isGrab = false;
    }


}

using Cysharp.Threading.Tasks.Triggers;
using Ev;
using UnityEngine;

public class PlayerGun : MonoBehaviour
{
    [SerializeField] Transform RayOrigin;
    [SerializeField] Transform GunParent;

    [SerializeField] GunDataComponent data;
    GunDataComponent haveGun = null;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        var _poseName = "Gun";
        var controllerSide = ControllerSide.Left;
        EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, new Ev.Events.HandPoseChange() { PoseName = _poseName, ControllerSide = controllerSide, HandPartType = HandPartType.All });
        EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_LOCK, new Ev.Events.HandPoseLock() { ControllerSide = controllerSide, HandPartType = HandPartType.All });

    }

    //// Update is called once per frame
    //void Update()
    //{
    //    if (!VRInputManager.Instance.IsTriggerPressed(ControllerSide.Right, false))
    //    {
    //        return;
    //    }

    //    if (haveGun != null)
    //    {
    //        //haveGun.Release();
    //        //haveGun = null;
    //        return;
    //    }

    //    var _poseName = "Gun";
    //    var controllerSide = ControllerSide.Left;
    //    EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, new Ev.Events.HandPoseChange() { PoseName = _poseName, ControllerSide = controllerSide, HandPartType = HandPartType.All });
    //    EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_LOCK, new Ev.Events.HandPoseLock() { ControllerSide = controllerSide, HandPartType = HandPartType.All });

    //    data.Have(GunParent);
    //    haveGun = data;
    //}

}

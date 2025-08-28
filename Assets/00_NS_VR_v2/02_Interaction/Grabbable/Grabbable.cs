using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactors;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using Ev;
using System;

public class Grabbable : MonoBehaviour
{
    public static Action<Grabbable, SelectEnterEventArgs> OnGrabEvent;
    public static Action<Grabbable, SelectExitEventArgs> OnReleaseEvent;

    public bool _hasPose;
    public string _poseName;
    public Transform _grabbedPoint_L;
    public Transform _grabbedPoint_R;
    public bool isAutoMirror = true;
    [SerializeField] XRGrabInteractable _grabInteractable;


    void Start()
    {
        SetLayerRecursively(transform, "GrabItem");

        CreateMirror();
    }

    public void CreateMirror()
    {
        if (_grabbedPoint_R == null && _grabbedPoint_L != null)
        {
            _grabbedPoint_R = Instantiate(_grabbedPoint_L.gameObject, _grabbedPoint_L.transform.parent).transform;

            if (isAutoMirror)
            {
                // 座標と回転を、X軸を中心に反転
                _grabbedPoint_R.localPosition = new Vector3(-_grabbedPoint_L.localPosition.x, _grabbedPoint_L.localPosition.y, _grabbedPoint_L.localPosition.z);
                _grabbedPoint_R.localRotation = new Quaternion(_grabbedPoint_L.localRotation.x, -_grabbedPoint_L.localRotation.y, -_grabbedPoint_L.localRotation.z, _grabbedPoint_L.localRotation.w);
            }
            _grabbedPoint_R.name = _grabbedPoint_L.name.Replace("L", "R");
        }

        if (_grabbedPoint_R != null && _grabbedPoint_L == null)
        {
            _grabbedPoint_L = Instantiate(_grabbedPoint_R.gameObject, _grabbedPoint_R.transform.parent).transform;

            if (isAutoMirror)
            {
                // 座標と回転を、X軸を中心に反転
                _grabbedPoint_L.localPosition = new Vector3(-_grabbedPoint_R.localPosition.x, _grabbedPoint_R.localPosition.y, _grabbedPoint_R.localPosition.z);
                _grabbedPoint_L.localRotation = new Quaternion(_grabbedPoint_R.localRotation.x, -_grabbedPoint_R.localRotation.y, -_grabbedPoint_R.localRotation.z, _grabbedPoint_R.localRotation.w);
            }
            _grabbedPoint_L.name = _grabbedPoint_R.name.Replace("R", "L");
        }
    }

    public void OnHover(HoverEnterEventArgs args)
    {
        var interactor = args.interactorObject as XRBaseInteractor;
        var handType = interactor.handedness;
        if (handType == InteractorHandedness.None) return;
        var controllerSide = handType == InteractorHandedness.Left ? ControllerSide.Left : ControllerSide.Right;
        SetAttach(controllerSide);
    }
    void SetAttach(ControllerSide controllerSide)
    {
        _grabInteractable.attachTransform = controllerSide == ControllerSide.Left && _grabbedPoint_L != null ? _grabbedPoint_L : _grabbedPoint_R;
    }
    public void OnGrab(SelectEnterEventArgs args)
    {
        var interactor = args.interactorObject as XRBaseInteractor;
        var handType = interactor.handedness;
        if (handType == InteractorHandedness.None) return;
        var controllerSide = handType == InteractorHandedness.Left ? ControllerSide.Left : ControllerSide.Right;

        SetLayerRecursively(transform, "Ignore Raycast");
        transform.parent = null;

        OnGrabEvent?.Invoke(this, args);

        SetAttach(controllerSide);
        //_grabInteractable.interactorsSelecting.Clear();
        //_grabInteractable.interactorsSelecting.Add(interactor);

        if (_hasPose)
        {
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, new Ev.Events.HandPoseChange() { PoseName = _poseName, ControllerSide = controllerSide, HandPartType = HandPartType.All });
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_LOCK, new Ev.Events.HandPoseLock() { ControllerSide = controllerSide, HandPartType = HandPartType.All });
        }

        var grabbable = GetComponentsInChildren<GrabbableAction>();
        foreach (var g in grabbable)
        {
            g.OnGrab(args, controllerSide);
        }

    }
    public void OnSelectEnterEvent(SelectEnterEventArgs args)
    {
        var interactor = args.interactorObject as XRBaseInteractor;
        var handType = interactor.handedness;
        var controllerSide = handType == InteractorHandedness.Left ? ControllerSide.Left : ControllerSide.Right;
        if (handType == InteractorHandedness.None) return;
        SetAttach(controllerSide);
    }

    public void OnRelease(SelectExitEventArgs args)
    {
        SetLayerRecursively(transform, "GrabItem");
        OnReleaseEvent?.Invoke(this, args);
        var interactor = args.interactorObject as XRBaseInteractor;
        var handType = interactor.handedness;
        var controllerSide = handType == InteractorHandedness.Left ? ControllerSide.Left : ControllerSide.Right;
        if (handType == InteractorHandedness.None) return;
        if (_hasPose)
        {
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_RESET, new Ev.Events.HandPoseReset() { ControllerSide = controllerSide, HandPartType = HandPartType.All });
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_UNLOCK, new Ev.Events.HandPoseUnlock() { ControllerSide = controllerSide, HandPartType = HandPartType.All });
        }

        var grabbable = GetComponentsInChildren<GrabbableAction>();
        foreach (var g in grabbable)
        {
            g.OnRelease(args, controllerSide);
        }
    }

    public static void SetLayerRecursively(Transform root, string layerName)
    {
        int layer = LayerMask.NameToLayer(layerName);
        if (layer == -1)
        {
            Debug.LogError($"Layer '{layerName}' does not exist.");
            return;
        }

        SetLayerRecursive(root, layer);
    }

    private static void SetLayerRecursive(Transform obj, int layer)
    {
        obj.gameObject.layer = layer;
        foreach (Transform child in obj)
        {
            SetLayerRecursive(child, layer);
        }
    }

}

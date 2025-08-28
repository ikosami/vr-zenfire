using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ev;

public class HandPoseManager : MonoBehaviour
{
    public ControllerSide ControllerSide;
    public List<HandPoseScriptable> HandPoseScriptables;
    public Transform HandPoseRoot;

    public Dictionary<string, Transform> HandRigs;

    public HandPoseScriptable CurrentPose;
    public float AnimateDuration = 0.5f;
    public string ResetPoseName = "Neutral";

    Coroutine _animateCoroutine;
    // Start is called before the first frame update
    void Start()
    {
        EventManager.StartListening(Ev.Events.Name.HAND_POSE_CHANGE, OnHandPoseChange);
        EventManager.StartListening(Ev.Events.Name.HAND_POSE_RESET, OnHandPoseReset);
        GetTransforms();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResetPose() {
        SelectPose(ResetPoseName, duration: AnimateDuration);
    }

    void OnHandPoseChange(IEvent ev) {
        if(ev is Ev.Events.HandPoseChange handPoseChange) {
            if(handPoseChange.ControllerSide == ControllerSide) {
                NS.ED.Log("OnHandPoseChange: " + handPoseChange.PoseName);
                SelectPose(handPoseChange.PoseName, duration: 0.5f);
            }
        }
    }

    void OnHandPoseReset(IEvent ev) {
        ResetPose();
    }

    public void GetTransforms() {
        HandRigs = new Dictionary<string, Transform>() { { HandPoseRoot.name, HandPoseRoot } };
        foreach(var kvp in GetChildTransforms(HandPoseRoot)) {
            HandRigs.Add(kvp.Key, kvp.Value);
        }
    }

    public void SelectPose(string poseName, bool isImmediate = false, float duration = 0.5f) {
        CurrentPose = HandPoseScriptables.Find(p => p.PoseName == poseName);
        if(_animateCoroutine != null) {
            StopCoroutine(_animateCoroutine);
        }
        if(isImmediate) {
            ApplyPoseToTransforms(HandRigs, CurrentPose);
        } else {
            _animateCoroutine = StartCoroutine(AnimatePoseCoroutine(CurrentPose, duration));
        }
    }

    private Dictionary<string, Transform> GetChildTransforms(Transform transform) {
        Dictionary<string, Transform> transforms = new Dictionary<string, Transform>();
        foreach(Transform child in transform) {
            transforms.Add(child.name, child);
            var childTransforms = GetChildTransforms(child);
            foreach(var kvp in childTransforms) {
                transforms.Add(kvp.Key, kvp.Value);
            }
        }

        return transforms;
    }



    private void ApplyPoseToTransforms(Dictionary<string, Transform> toTransforms, HandPoseScriptable fromPose) {
        foreach(HandRigData rig in fromPose.Rigs) {
            Transform transform = toTransforms[rig.ObjectName];
            transform.localPosition = rig.Position;
            transform.localRotation = Quaternion.Euler(rig.Rotation);
            transform.localScale = rig.Scale;
        }
    }

    private void ApplyPoseToTransforms(Dictionary<string, Transform> toTransforms, List<HandRigData> fromPose, float rate = 1.0f) {
        foreach(HandRigData rig in fromPose) {
            Transform transform = toTransforms[rig.ObjectName];
            transform.localPosition = Vector3.Lerp(transform.localPosition, rig.Position, rate);
            transform.localRotation = Quaternion.Slerp(transform.localRotation, Quaternion.Euler(rig.Rotation), rate);
            transform.localScale = Vector3.Lerp(transform.localScale, rig.Scale, rate);
        }
    }

    public void ApplyTransformsToPoseDatas(Dictionary<string, Transform> fromTransforms, HandPoseScriptable toPoseDatas) {
        List<HandRigData> poseDatas = toPoseDatas.CloneRigs();
        foreach(var kvp in fromTransforms) {
            HandRigData poseData = new HandRigData();
            poseData.ObjectName = kvp.Key;
            poseData.Position = kvp.Value.localPosition;
            poseData.Rotation = kvp.Value.localRotation.eulerAngles;
            poseData.Scale = kvp.Value.localScale;
            poseDatas.Add(poseData);
        }
        toPoseDatas.Rigs = poseDatas;
    }

    // void AnimatePose(string poseName) {
    //     HandPoseScriptable pose = HandPoseScriptables.Find(p => p.PoseName == poseName);
    //     StartCoroutine(AnimatePoseCoroutine(pose, AnimateDuration));
    // }

    IEnumerator AnimatePoseCoroutine(HandPoseScriptable pose, float duration) {
        var time = 0.0f;
        while(time < duration) {
            ApplyPoseToTransforms(HandRigs, pose.Rigs, time / duration);
            time += Time.deltaTime;
            yield return null;
        }
        ApplyPoseToTransforms(HandRigs, pose.Rigs);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Ev;

public class HandPoseManager : MonoBehaviour
{
    public ControllerSide ControllerSide;
    public List<HandPoseScriptable> HandPoseScriptables;
    public Transform HandPoseRoot;
    
    [System.Serializable]
    public class FingerRig
    {
        public Transform root;
        public Transform[] joints;
    }

    [SerializeField]
    private FingerRig[] fingerRigs = new FingerRig[5]; // thumb, index, middle, ring, pinky

    public Dictionary<string, Transform> HandRigs;

    public HandPoseScriptable CurrentPose;
    public float AnimateDuration = 0.5f;
    public string ResetPoseName = "Neutral";

    private Coroutine[] _fingerAnimateCoroutines = new Coroutine[5];
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
        
        // 既存のアニメーションを停止
        if(_animateCoroutine != null) {
            StopCoroutine(_animateCoroutine);
        }
        for (int i = 0; i < _fingerAnimateCoroutines.Length; i++) {
            if (_fingerAnimateCoroutines[i] != null) {
                StopCoroutine(_fingerAnimateCoroutines[i]);
                _fingerAnimateCoroutines[i] = null;
            }
        }

        if(isImmediate) {
            ApplyPoseToTransforms(HandRigs, CurrentPose);
        } else {
            // 指ごとにアニメーションを開始
            for (int i = 0; i < fingerRigs.Length; i++) {
                var fingerRig = fingerRigs[i];
                if (fingerRig != null && fingerRig.root != null) {
                    var fingerTransforms = GetFingerTransforms(fingerRig);
                    var fingerPoseData = GetFingerPoseData(CurrentPose, fingerRig.root.name);
                    _fingerAnimateCoroutines[i] = StartCoroutine(
                        AnimateFingerPoseCoroutine(fingerTransforms, fingerPoseData, duration)
                    );
                }
            }
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

    private Dictionary<string, Transform> GetFingerTransforms(FingerRig fingerRig) {
        Dictionary<string, Transform> transforms = new Dictionary<string, Transform>();
        transforms[fingerRig.root.name] = fingerRig.root;
        foreach (var joint in fingerRig.joints) {
            if (joint != null) {
                transforms[joint.name] = joint;
            }
        }
        return transforms;
    }

    private List<HandRigData> GetFingerPoseData(HandPoseScriptable pose, string rootName) {
        return pose.Rigs.FindAll(rig => rig.ObjectName.StartsWith(rootName));
    }

    IEnumerator AnimateFingerPoseCoroutine(Dictionary<string, Transform> fingerTransforms, List<HandRigData> poseData, float duration) {
        var time = 0.0f;
        var startRotations = new Dictionary<string, Quaternion>();
        var startPositions = new Dictionary<string, Vector3>();

        // 開始時の各ボーンの位置と回転を保存
        foreach (var rig in poseData) {
            if (fingerTransforms.TryGetValue(rig.ObjectName, out Transform transform)) {
                startRotations[rig.ObjectName] = transform.localRotation;
                startPositions[rig.ObjectName] = transform.localPosition;
            }
        }

        while (time < duration) {
            float normalizedTime = time / duration;
            float t = EaseInOutQuad(normalizedTime);
            
            foreach (var rig in poseData) {
                if (fingerTransforms.TryGetValue(rig.ObjectName, out Transform transform)) {
                    // 位置の補間
                    transform.localPosition = Vector3.Lerp(
                        startPositions[rig.ObjectName],
                        rig.Position,
                        t
                    );

                    // 回転の補間（最短経路を使用）
                    transform.localRotation = Quaternion.Slerp(
                        startRotations[rig.ObjectName],
                        Quaternion.Euler(rig.Rotation),
                        t
                    );
                }
            }

            time += Time.deltaTime;
            yield return null;
        }

        // 最終位置と回転を確実に適用
        foreach (var rig in poseData) {
            if (fingerTransforms.TryGetValue(rig.ObjectName, out Transform transform)) {
                transform.localPosition = rig.Position;
                transform.localRotation = Quaternion.Euler(rig.Rotation);
            }
        }
    }

    private float EaseInOutQuad(float t) {
        return t < 0.5f ? 2f * t * t : -1f + (4f - 2f * t) * t;
    }
}

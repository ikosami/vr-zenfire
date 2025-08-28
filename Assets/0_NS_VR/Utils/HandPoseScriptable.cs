
using System.Collections.Generic;
using UnityEngine;
using System;
using Oculus.Haptics;

[CreateAssetMenu(fileName = "HandPoseScriptable", menuName = "HandPoseScriptable")]
public class HandPoseScriptable : ScriptableObject
{
    [SerializeField] List<HandPoseData> _poses;
}

[System.Serializable]
public class HandPoseData {
    public string Name;
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;

    public List<HandPoseData> Children;
}
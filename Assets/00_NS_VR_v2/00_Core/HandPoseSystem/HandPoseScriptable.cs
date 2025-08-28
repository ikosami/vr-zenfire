
using System.Collections.Generic;
using UnityEngine;
using System;
using Oculus.Haptics;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(fileName = "HandPoseScriptable", menuName = "HandPoseScriptable")]
public class HandPoseScriptable : ScriptableObject
{
    public ControllerSide ControllerSide;
    public string Prefix = "L_";
    public string MirroredPrefix = "R_";
    public string PoseName;
    public List<HandRigData> Rigs;

    public List<HandRigData> CloneRigs() {
        List<HandRigData> clone = new List<HandRigData>();
        foreach(HandRigData rig in Rigs) {
            clone.Add(rig.Clone());
        }
        return clone;
    }

    
    public HandPoseScriptable CreateMirroredPose()
    {
        HandPoseScriptable mirroredPose = CreateInstance<HandPoseScriptable>();
        mirroredPose.PoseName = this.PoseName;
        mirroredPose.Rigs = new List<HandRigData>();

        foreach (HandRigData rig in Rigs)
        {
            HandRigData mirroredRig = rig.Clone();
            mirroredRig.Position.x *= -1;
            mirroredRig.Rotation.y *= -1;
            mirroredRig.Rotation.z *= -1;
            
            mirroredRig.ObjectName = mirroredRig.ObjectName
                .Replace("Left", "Temp")
                .Replace("Right", "Left")
                .Replace("Temp", "Right")
                .Replace(Prefix, MirroredPrefix);



            mirroredPose.Rigs.Add(mirroredRig);
        }

        mirroredPose.Prefix = MirroredPrefix;
        mirroredPose.MirroredPrefix = Prefix;
        mirroredPose.ControllerSide = ControllerSide == ControllerSide.Left ? ControllerSide.Right : ControllerSide.Left;

        return mirroredPose;
    }

#if UNITY_EDITOR
    [MenuItem("Assets/Create Mirrored Hand Pose")]
    private static void CreateMirroredPoseAsset()
    {
        HandPoseScriptable selectedPose = Selection.activeObject as HandPoseScriptable;
        if (selectedPose == null)
        {
            Debug.LogError("選択されたアセットがHandPoseScriptableではありません。");
            return;
        }

        HandPoseScriptable mirroredPose = selectedPose.CreateMirroredPose();
        
        string path = AssetDatabase.GetAssetPath(selectedPose);
        string directory = System.IO.Path.GetDirectoryName(path);
        string newPath = directory + "/" + selectedPose.name + "_Mirrored.asset";
        
        AssetDatabase.CreateAsset(mirroredPose, newPath);
        AssetDatabase.SaveAssets();
        
        Debug.Log($"反転したポーズを作成しました: {newPath}");
    }
#endif
}

[System.Serializable]
public class HandRigData {
    public string ObjectName;
    public Vector3 Position;
    public Vector3 Rotation;
    public Vector3 Scale;

    public void Lerp(HandRigData to, float rate) {
        Position = Vector3.Lerp(Position, to.Position, rate);
        // クォータニオンを使用して最短経路での補間を行う
        Quaternion fromRot = Quaternion.Euler(Rotation);
        Quaternion toRot = Quaternion.Euler(to.Rotation);
        Rotation = Quaternion.Slerp(fromRot, toRot, rate).eulerAngles;
        Scale = Vector3.Lerp(Scale, to.Scale, rate);
    }

    public HandRigData Clone() {
        HandRigData clone = new HandRigData();
        clone.ObjectName = ObjectName;
        clone.Position = Position;
        clone.Rotation = Rotation;
        clone.Scale = Scale;
        return clone;
    }
}

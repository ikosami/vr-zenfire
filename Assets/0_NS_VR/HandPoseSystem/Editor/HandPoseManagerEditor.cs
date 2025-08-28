using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(HandPoseManager))]
public class HandPoseManagerEditor : Editor
{
    HandPoseManager handPoseManager;

    private void OnEnable()
    {
        handPoseManager = (HandPoseManager)target;
    }

    public override void OnInspectorGUI()
    {
        // デフォルトのインスペクタを表示
        DrawDefaultInspector();

        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("ポーズ編集", EditorStyles.boldLabel);

        if (handPoseManager.HandPoseScriptables == null || handPoseManager.HandPoseScriptables.Count == 0)
        {
            EditorGUILayout.HelpBox("HandPoseScriptablesリストが空です", MessageType.Warning);
            return;
        }

        // ポーズ選択用のポップアップ
        string[] poseNames = handPoseManager.HandPoseScriptables.ConvertAll(pose => pose.PoseName).ToArray();
        handPoseManager.CurrentPoseIndex = EditorGUILayout.Popup("編集するポーズ", handPoseManager.CurrentPoseIndex, poseNames);

        if (GUILayout.Button("現在の姿勢を保存"))
        {
            if (handPoseManager.HandPoseRoot == null)
            {
                EditorUtility.DisplayDialog("エラー", "HandPoseRootが設定されていません", "OK");
                return;
            }

            SaveCurrentPose(handPoseManager.CurrentPoseIndex);
        }

        if (GUILayout.Button("選択したポーズを適用"))
        {
            handPoseManager.GetTransforms(); // TransformsのDictionaryを更新
            handPoseManager.SelectPose(poseNames[handPoseManager.CurrentPoseIndex], HandPartType.All, true);
        }
    }

    private void SaveCurrentPose(int index)
    {
        var targetPose = handPoseManager.HandPoseScriptables[index];
        Undo.RecordObject(targetPose, "Save Hand Pose");

        // 現在のTransformsを取得して保存
        handPoseManager.GetTransforms();
        handPoseManager.ApplyTransformsToPoseDatas(handPoseManager.HandRigs, targetPose);

        EditorUtility.SetDirty(targetPose);
        AssetDatabase.SaveAssets();
        EditorUtility.DisplayDialog("成功", $"ポーズ「{targetPose.PoseName}」を保存しました", "OK");
    }
}

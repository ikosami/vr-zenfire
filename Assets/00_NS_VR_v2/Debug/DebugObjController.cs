using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugObjController : MonoBehaviour
{
    public List<GameObject> debugObjects;

    private float holdTime = 5f; // 5秒長押し時間
    private float buttonHoldTimer = 0f; // ボタン押下時間カウント

    void Start()
    {
        UpdateDebugObjects(DebugSettings.IsDebugVisible);
    }

    void Update()
    {
        if (!DebugSettings.IsDebugModeEnabled) return; // デバッグモードがOFFなら処理しない

        bool isPrimary = VRInputManager.Instance.ButtonPrimaryPressed(ControllerSide.Right);
        bool isSecondary = VRInputManager.Instance.ButtonSecondaryPressed(ControllerSide.Right);

        if (isPrimary && isSecondary)
        {
            buttonHoldTimer += Time.deltaTime;

            if (buttonHoldTimer >= holdTime)
            {
                ToggleDebugObjects();
                buttonHoldTimer = 0f; // タイマーリセット
            }
        }
        else
        {
            buttonHoldTimer = 0f; // ボタンが離されたらタイマーリセット
        }
    }

    public void UpdateDebugObjects(bool isVisible)
    {
        if (!DebugSettings.IsDebugModeEnabled)
        {
            isVisible = false;
        }
        DebugSettings.IsDebugVisible = isVisible;

        foreach (var obj in debugObjects)
        {
            obj.SetActive(isVisible);
        }
    }

    private void ToggleDebugObjects()
    {
        bool newState = !DebugSettings.IsDebugVisible;
        UpdateDebugObjects(newState);
    }
}

public static class DebugSettings
{
    public static bool IsDebugModeEnabled = false; // デバッグ機能全体の有効/無効
    public static bool IsDebugVisible = false; // デバッグ用オブジェクトの表示状態
}

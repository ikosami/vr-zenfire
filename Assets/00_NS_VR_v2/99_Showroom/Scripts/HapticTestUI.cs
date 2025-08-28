using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HapticTestUI : MonoBehaviour
{
    public TMP_Dropdown hapticDropdown;
    public HapticReferencesScriptable hapticReferences;

    public string hapticName = "";

    float _leftTimer = 0f;
    float _rightTimer = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // ドロップダウンの初期化
        if (hapticDropdown != null && hapticReferences != null)
        {
            List<string> options = new List<string>();
            foreach (var haptic in hapticReferences.Haptics)
            {
                options.Add(haptic.Key); // Hapticの名前を表示
            }
            hapticDropdown.ClearOptions();
            hapticDropdown.AddOptions(options);
            hapticDropdown.onValueChanged.AddListener(OnHapticDropdownChanged);

            // 初期設定として最初のHapticを設定
            if (hapticReferences.Haptics.Count > 0)
            {
                hapticName = hapticReferences.Haptics[0].Key;
            }
        }
    }

    // ドロップダウンが変更されたときに呼ばれるイベントハンドラー
    void OnHapticDropdownChanged(int index)
    {
        if (hapticReferences != null && index < hapticReferences.Haptics.Count)
        {
            hapticName = hapticReferences.Haptics[index].Key;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(_leftTimer > 0f) {
            _leftTimer -= Time.deltaTime;
        }
        if(_rightTimer > 0f) {
            _rightTimer -= Time.deltaTime;
        }

        if(VRInputManager.Instance.IsTriggerPressed(ControllerSide.Left, true)) {
            if(_leftTimer <= 0f) {
                VibrationController.Instance.Play(hapticName, ControllerSide.Left);
                _leftTimer = 0.5f;
            }
        }

        if(VRInputManager.Instance.IsTriggerPressed(ControllerSide.Right, true)) {
            if(_rightTimer <= 0f) {
                VibrationController.Instance.Play(hapticName, ControllerSide.Right);
                _rightTimer = 0.5f;
            }
        }
    }
}

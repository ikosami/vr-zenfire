using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugModeActivator : MonoBehaviour
{
    public Transform vrController; // VRコントローラーのTransform

    private Vector3 relativePosition; // コントローラーの相対位置
    private ControllerPosition currentControllerPos; // コントローラーの位置分類
    [SerializeField] GameObject debugOnView;


    [System.Serializable]
    public class DebugSequenceStep
    {
        public ControllerPosition position;
        public ButtonCombination buttonCombo;
    }

    public List<DebugSequenceStep> debugModeSequence; // 正しい入力のシーケンス
    private int currentStep = 0; // 現在の入力進行状況
    bool isWait = false;

    private void Awake()
    {
        debugOnView.SetActive(false);
    }

    void Update()
    {
        UpdateControllerPosition();

        bool isPrimaryDown = VRInputManager.Instance.ButtonPrimaryPressed(ControllerSide.Right);
        bool isSecondaryDown = VRInputManager.Instance.ButtonSecondaryPressed(ControllerSide.Right);

        ButtonCombination currentCombo = GetButtonCombination(isPrimaryDown, isSecondaryDown);

        if (isWait)
        {
            if (currentCombo == ButtonCombination.None)
            {
                isWait = false;
            }
            else
            {
                return;
            }
        }
        if (debugModeSequence.Count > 0 && currentStep < debugModeSequence.Count)
        {
            var expectedStep = debugModeSequence[currentStep];

            if (currentCombo != ButtonCombination.None) // A か B を押した瞬間に判定
            {
                isWait = true;
                if (expectedStep.position == currentControllerPos && expectedStep.buttonCombo == currentCombo)
                {
                    currentStep++;
                    Debug.Log($"Step {currentStep}/{debugModeSequence.Count} 完了");

                    if (currentStep >= debugModeSequence.Count)
                    {
                        ActivateDebugMode();
                        currentStep = 0;
                    }
                }
                else // 間違えたらリセット
                {
                    Debug.Log($"入力ミス！最初からやり直し currentControllerPos:{currentControllerPos}  currentCombo:{currentCombo}");
                    currentStep = 0;
                }
            }
        }
    }

    private void UpdateControllerPosition()
    {
        relativePosition = vrController.position - Camera.main.transform.position;

        bool isRight = relativePosition.x >= 0;
        bool isFront = relativePosition.z >= 0;
        bool isUp = relativePosition.y >= 0;

        currentControllerPos = (ControllerPosition)(
            (isRight ? 1 : 0) +
            (isFront ? 2 : 0) +
            (isUp ? 4 : 0)
        );
    }

    private ButtonCombination GetButtonCombination(bool isPrimaryDown, bool isSecondaryDown)
    {
        if (isPrimaryDown) return ButtonCombination.A;
        if (isSecondaryDown) return ButtonCombination.B;
        return ButtonCombination.None; // ここに来ることはないが、念のため
    }

    private void ActivateDebugMode()
    {
        DebugSettings.IsDebugModeEnabled = true;
        DebugSettings.IsDebugVisible = true;
        Debug.Log("デバッグモードON!");
        debugOnView.SetActive(true);
    }
}

// コントローラーの位置（8通り）
public enum ControllerPosition
{
    BackLeftDown = 0,
    BackRightDown = 1,
    FrontLeftDown = 2,
    FrontRightDown = 3,
    BackLeftUp = 4,
    BackRightUp = 5,
    FrontLeftUp = 6,
    FrontRightUp = 7
}

// ボタンの組み合わせ（2通り）
public enum ButtonCombination
{
    A,    // Aボタン
    B,     // Bボタン
    None
}

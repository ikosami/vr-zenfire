using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// 集中化されたVR入力管理システム
/// すべてのVRコントローラー入力（スティック、ボタン、トリガー）を管理
/// </summary>
public class VRInputManager : MonoBehaviour
{
    private static VRInputManager _instance;
    public static VRInputManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<VRInputManager>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("VRInputManager");
                    _instance = go.AddComponent<VRInputManager>();
                }
            }
            return _instance;
        }
    }

    // スティック入力
    [Header("スティック")]
    [SerializeField] private InputActionReference leftStick;
    [SerializeField] private InputActionReference rightStick;

    // ボタン入力
    [Header("ボタン")]
    [SerializeField] private InputActionReference buttonA;
    [SerializeField] private InputActionReference buttonB;

    // トリガー入力
    [Header("トリガー")]
    [SerializeField] private InputActionReference leftIndexTrigger;
    [SerializeField] private InputActionReference leftMiddleTrigger;
    [SerializeField] private InputActionReference rightIndexTrigger;
    [SerializeField] private InputActionReference rightMiddleTrigger;

    // コントローラーの位置と回転
    [Header("コントローラートラッキング")]
    [SerializeField] private InputActionReference leftControllerPosition;
    [SerializeField] private InputActionReference leftControllerRotation;
    [SerializeField] private InputActionReference rightControllerPosition;
    [SerializeField] private InputActionReference rightControllerRotation;

    // 公開プロパティ
    public Vector2 LeftStickValue => leftStick?.action.ReadValue<Vector2>() ?? Vector2.zero;
    public Vector2 RightStickValue => rightStick?.action.ReadValue<Vector2>() ?? Vector2.zero;
    
    public bool ButtonAPressed => buttonA?.action.IsPressed() ?? false;
    public bool ButtonBPressed => buttonB?.action.IsPressed() ?? false;
    
    public float LeftIndexTriggerValue => leftIndexTrigger?.action.ReadValue<float>() ?? 0f;
    public float LeftMiddleTriggerValue => leftMiddleTrigger?.action.ReadValue<float>() ?? 0f;
    public float RightIndexTriggerValue => rightIndexTrigger?.action.ReadValue<float>() ?? 0f;
    public float RightMiddleTriggerValue => rightMiddleTrigger?.action.ReadValue<float>() ?? 0f;
    
    public Vector3 LeftControllerPos => leftControllerPosition?.action.ReadValue<Vector3>() ?? Vector3.zero;
    public Quaternion LeftControllerRot => Quaternion.Euler(leftControllerRotation?.action.ReadValue<Vector3>() ?? Vector3.zero);
    public Vector3 RightControllerPos => rightControllerPosition?.action.ReadValue<Vector3>() ?? Vector3.zero;
    public Quaternion RightControllerRot => Quaternion.Euler(rightControllerRotation?.action.ReadValue<Vector3>() ?? Vector3.zero);

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        EnableAllActions();
    }

    private void OnDisable()
    {
        DisableAllActions();
    }

    /// <summary>
    /// すべての入力アクションを有効化
    /// </summary>
    private void EnableAllActions()
    {
        leftStick?.action.Enable();
        rightStick?.action.Enable();
        buttonA?.action.Enable();
        buttonB?.action.Enable();
        leftIndexTrigger?.action.Enable();
        leftMiddleTrigger?.action.Enable();
        rightIndexTrigger?.action.Enable();
        rightMiddleTrigger?.action.Enable();
        leftControllerPosition?.action.Enable();
        leftControllerRotation?.action.Enable();
        rightControllerPosition?.action.Enable();
        rightControllerRotation?.action.Enable();
    }

    /// <summary>
    /// すべての入力アクションを無効化
    /// </summary>
    private void DisableAllActions()
    {
        leftStick?.action.Disable();
        rightStick?.action.Disable();
        buttonA?.action.Disable();
        buttonB?.action.Disable();
        leftIndexTrigger?.action.Disable();
        leftMiddleTrigger?.action.Disable();
        rightIndexTrigger?.action.Disable();
        rightMiddleTrigger?.action.Disable();
        leftControllerPosition?.action.Disable();
        leftControllerRotation?.action.Disable();
        rightControllerPosition?.action.Disable();
        rightControllerRotation?.action.Disable();
    }

    /// <summary>
    /// 指定された閾値以上のトリガー入力があるかチェック
    /// </summary>
    public bool IsTriggerPressed(ControllerSide side, bool isIndex, float threshold = 0.5f)
    {
        if (side == ControllerSide.Left)
        {
            return isIndex ? LeftIndexTriggerValue > threshold : LeftMiddleTriggerValue > threshold;
        }
        return isIndex ? RightIndexTriggerValue > threshold : RightMiddleTriggerValue > threshold;
    }

    /// <summary>
    /// コントローラーの速度を計算（アームスイング移動用）
    /// </summary>
    public Vector3 GetControllerVelocity(ControllerSide side)
    {
        // 実際の実装では、前フレームの位置との差分から速度を計算する必要があります
        // この実装は後でMovementControllerで行います
        return Vector3.zero;
    }
}

using Unity.VisualScripting;
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

    public System.Action<ControllerSide, VRInputType> OnInput;

    // スティック入力
    [Header("スティック")]
    [SerializeField] private InputActionReference leftStick;
    [SerializeField] private InputActionReference rightStick;
    [SerializeField] private InputActionReference leftStickTouch;
    [SerializeField] private InputActionReference rightStickTouch;

    // ボタン入力
    [Header("ボタン")]
    [SerializeField] private InputActionReference leftButtonPrimary;
    [SerializeField] private InputActionReference leftButtonSecondary;
    [SerializeField] private InputActionReference rightButtonPrimary;
    [SerializeField] private InputActionReference rightButtonSecondary;

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
    public float LeftStickTouch => leftStickTouch?.action.ReadValue<float>() ?? 0f;
    public float RightStickTouch => rightStickTouch?.action.ReadValue<float>() ?? 0f;

    public bool LeftButtonPrimaryPressed => leftButtonPrimary?.action.IsPressed() ?? false;
    public bool LeftButtonSecondaryPressed => leftButtonSecondary?.action.IsPressed() ?? false;
    public float LeftButtonPrimaryValue => leftButtonPrimary?.action.ReadValue<float>() ?? 0f;
    public float LeftButtonSecondaryValue => leftButtonSecondary?.action.ReadValue<float>() ?? 0f;

    public bool RightButtonPrimaryPressed => rightButtonPrimary?.action.IsPressed() ?? false;
    public bool RightButtonSecondaryPressed => rightButtonSecondary?.action.IsPressed() ?? false;
    public float RightButtonPrimaryValue => rightButtonPrimary?.action.ReadValue<float>() ?? 0f;
    public float RightButtonSecondaryValue => rightButtonSecondary?.action.ReadValue<float>() ?? 0f;
    
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
        leftStickTouch?.action.Enable();
        rightStickTouch?.action.Enable();
        leftButtonPrimary?.action.Enable();
        leftButtonSecondary?.action.Enable();
        rightButtonPrimary?.action.Enable();
        rightButtonSecondary?.action.Enable();
        leftIndexTrigger?.action.Enable();
        leftMiddleTrigger?.action.Enable();
        rightIndexTrigger?.action.Enable();
        rightMiddleTrigger?.action.Enable();
        leftControllerPosition?.action.Enable();
        leftControllerRotation?.action.Enable();
        rightControllerPosition?.action.Enable();
        rightControllerRotation?.action.Enable();

        leftStick.action.performed += OnInputLeftStick;
        rightStick.action.performed += OnInputRightStick;
        leftStickTouch.action.performed += OnInputLeftStickTouch;
        rightStickTouch.action.performed += OnInputRightStickTouch;
        leftButtonPrimary.action.performed += OnInputLeftButtonPrimary;
        leftButtonSecondary.action.performed += OnInputLeftButtonSecondary;
        rightButtonPrimary.action.performed += OnInputRightButtonPrimary;
        rightButtonSecondary.action.performed += OnInputRightButtonSecondary;
        leftIndexTrigger.action.performed += OnInputLeftTrigger;
        leftMiddleTrigger.action.performed += OnInputLeftGrip;
        rightIndexTrigger.action.performed += OnInputRightTrigger;
        rightMiddleTrigger.action.performed += OnInputRightGrip;
    }

    private void OnInputLeftStick(InputAction.CallbackContext context) {
        OnInput?.Invoke(ControllerSide.Left, VRInputType.Stick);
    }

    private void OnInputRightStick(InputAction.CallbackContext context) {
        OnInput?.Invoke(ControllerSide.Right, VRInputType.Stick);
    }

    private void OnInputLeftStickTouch(InputAction.CallbackContext context) {
        OnInput?.Invoke(ControllerSide.Left, VRInputType.StickTouch);
    }

    private void OnInputRightStickTouch(InputAction.CallbackContext context) {
        OnInput?.Invoke(ControllerSide.Right, VRInputType.StickTouch);
    }

    private void OnInputLeftButtonPrimary(InputAction.CallbackContext context) {
        OnInput?.Invoke(ControllerSide.Left, VRInputType.ButtonPrimary);
    }

    private void OnInputRightButtonPrimary(InputAction.CallbackContext context) {
        OnInput?.Invoke(ControllerSide.Right, VRInputType.ButtonPrimary);
    }

    private void OnInputLeftButtonSecondary(InputAction.CallbackContext context) {
        OnInput?.Invoke(ControllerSide.Left, VRInputType.ButtonSecondary);
    }

    private void OnInputRightButtonSecondary(InputAction.CallbackContext context) {
        OnInput?.Invoke(ControllerSide.Right, VRInputType.ButtonSecondary);
    }

    private void OnInputLeftTrigger(InputAction.CallbackContext context) {
        OnInput?.Invoke(ControllerSide.Left, VRInputType.Trigger);
    }

    private void OnInputRightTrigger(InputAction.CallbackContext context) {
        OnInput?.Invoke(ControllerSide.Right, VRInputType.Trigger);
    }

    private void OnInputLeftGrip(InputAction.CallbackContext context) {
        OnInput?.Invoke(ControllerSide.Left, VRInputType.Grip);
    }

    private void OnInputRightGrip(InputAction.CallbackContext context) {
        OnInput?.Invoke(ControllerSide.Right, VRInputType.Grip);
    }

    /// <summary>
    /// すべての入力アクションを無効化
    /// </summary>
    private void DisableAllActions()
    {
        leftStick?.action.Disable();
        rightStick?.action.Disable();
        leftStickTouch?.action.Disable();
        rightStickTouch?.action.Disable();
        leftButtonPrimary?.action.Disable();
        leftButtonSecondary?.action.Disable();
        rightButtonPrimary?.action.Disable();
        rightButtonSecondary?.action.Disable();
        leftIndexTrigger?.action.Disable();
        leftMiddleTrigger?.action.Disable();
        rightIndexTrigger?.action.Disable();
        rightMiddleTrigger?.action.Disable();
        leftControllerPosition?.action.Disable();
        leftControllerRotation?.action.Disable();
        rightControllerPosition?.action.Disable();
        rightControllerRotation?.action.Disable();

        leftStick.action.performed -= OnInputLeftStick;
        rightStick.action.performed -= OnInputRightStick;
        leftStickTouch.action.performed -= OnInputLeftStickTouch;
        rightStickTouch.action.performed -= OnInputRightStickTouch;
        leftButtonPrimary.action.performed -= OnInputLeftButtonPrimary;
        leftButtonSecondary.action.performed -= OnInputLeftButtonSecondary;
        rightButtonPrimary.action.performed -= OnInputRightButtonPrimary;
        rightButtonSecondary.action.performed -= OnInputRightButtonSecondary;
        leftIndexTrigger.action.performed -= OnInputLeftTrigger;
        leftMiddleTrigger.action.performed -= OnInputLeftGrip;
        rightIndexTrigger.action.performed -= OnInputRightTrigger;
        rightMiddleTrigger.action.performed -= OnInputRightGrip;
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

    public Vector2 StickValue(ControllerSide side) {
        return side == ControllerSide.Left ? LeftStickValue : RightStickValue;
    }

    public bool StickTouchPressed(ControllerSide side) {
        return side == ControllerSide.Left ? LeftStickTouch > 0f : RightStickTouch > 0f;
    }
    
    public bool ButtonPrimaryPressed(ControllerSide side) {
        return side == ControllerSide.Left ? LeftButtonPrimaryPressed : RightButtonPrimaryPressed;
    }

    public bool ButtonSecondaryPressed(ControllerSide side) {
        return side == ControllerSide.Left ? LeftButtonSecondaryPressed : RightButtonSecondaryPressed;
    }

    public float ButtonPrimaryValue(ControllerSide side) {
        return side == ControllerSide.Left ? LeftButtonPrimaryValue : RightButtonPrimaryValue;
    }

    public float ButtonSecondaryValue(ControllerSide side) {
        return side == ControllerSide.Left ? LeftButtonSecondaryValue : RightButtonSecondaryValue;
    }

    public float TriggerValue(ControllerSide side) {
        return side == ControllerSide.Left ? LeftIndexTriggerValue : RightIndexTriggerValue;
    }

    public float GripValue(ControllerSide side) {
        return side == ControllerSide.Left ? LeftMiddleTriggerValue : RightMiddleTriggerValue;
    }
}

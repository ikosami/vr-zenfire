using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// VRコントローラーを使用したプレイヤー移動システム
/// スティック入力とアームスイング（ゴリラタグ方式）の両方をサポート
/// </summary>
public class MovementController : MonoBehaviour
{

    [SerializeField] bool _canLeftStickMove = true;
    [SerializeField] bool _canArmSwingMove = true;
    [SerializeField] bool _canRightStickRotate = true;

    [SerializeField] GameObject _leftStickMoveObject;
    [SerializeField] GameObject _rightStickRotateObject;

    [Header("移動設定")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private Transform headTransform;
    [SerializeField] private Transform leftControllerTransform;
    [SerializeField] private Transform rightControllerTransform;
    [SerializeField] private Transform leftControllerBottomTransform;
    [SerializeField] private Transform rightControllerBottomTransform;
    private float handMoveAddForce => ParamData.Instance.HandMoveAddForce;
    private float maxSpeed => ParamData.Instance.MaxSpeed;
    private float friction => ParamData.Instance.Friction;
    private float jumpAddForce => ParamData.Instance.JumpAddForce;
    private float groundFriction => ParamData.Instance.GroundFriction;

    [Header("アームスイング設定")]
    [SerializeField] private float armSwingForce = 2.0f;
    private float armSwingThreshold => ParamData.Instance.ArmSwingThreshold;
    private float jumpThreshold => ParamData.Instance.JumpThreshold;
    private float groundCheckDistance => ParamData.Instance.GroundCheckDistance; // 地面判定の距離
    [SerializeField] private LayerMask groundLayer = -1; // 地面として判定するレイヤー

    private Vector3 moveVelocity;
    private float verticalVelocity;
    private Vector3 previousLeftPosition;
    private Vector3 previousRightPosition;
    private bool isGrounded;

    private void Start()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
        }

        // 初期位置を記録
        previousLeftPosition = leftControllerTransform.localPosition;
        previousRightPosition = rightControllerTransform.localPosition;

        // _leftStickMoveObject.SetActive(_canLeftStickMove);
        _rightStickRotateObject.SetActive(_canRightStickRotate);
        ParamData.OnParamChange += OnParamChange;
        OnParamChange("All");
    }

    private void OnParamChange(string key) {
        var allUpdate = key == "All";
        if(allUpdate || key == "CanLeftStickMove") {
            _canLeftStickMove = ParamData.Instance.CanLeftStickMove;
            _leftStickMoveObject.SetActive(_canLeftStickMove);
        }
        if(allUpdate || key == "CanArmSwingMove") {
            _canArmSwingMove = ParamData.Instance.CanArmSwingMove;
        }
        if(allUpdate || key == "CanRightStickRotate") {
            _canRightStickRotate = ParamData.Instance.CanRightStickRotate;
            _rightStickRotateObject.SetActive(_canRightStickRotate);
        }

        if(allUpdate || key == "Gravity") {
            Physics.gravity = new Vector3(0, -ParamData.Instance.Gravity, 0);
        }
    }

    private void Update()
    {
        UpdateGroundedState();
        if(_canLeftStickMove) UpdateStickMovement();
        if(_canArmSwingMove) UpdateArmSwingMovement();
        UpdateGravity();
        ApplyMovement();
    }

    public void SetLeftStickMoveEnabled(bool enabled)
    {
        _canLeftStickMove = enabled;
        _leftStickMoveObject.SetActive(enabled);
    }

    public void SetRightStickRotateEnabled(bool enabled)
    {
        _canRightStickRotate = enabled;
        _rightStickRotateObject.SetActive(enabled);
    }
    

    private void UpdateGroundedState()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -0.5f; // 接地判定を安定させる
        }
        if(isGrounded) moveVelocity -= groundFriction * moveVelocity * Time.deltaTime;
    }

    private void UpdateStickMovement()
    {
        // スティック入力による移動
        Vector2 stickInput = VRInputManager.Instance.LeftStickValue;
        if (stickInput.magnitude > 0.1f)
        {
            // カメラの向きを基準に移動方向を決定
            Vector3 cameraForward = headTransform.forward;
            Vector3 cameraRight = headTransform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            moveVelocity = (cameraForward * stickInput.y + cameraRight * stickInput.x) * handMoveAddForce;
        }
    }

    private bool IsControllerGrounded(Vector3 controllerPosition, Transform controllerBottomTransform)
    {
        // コントローラーの位置を中心のカプセルレイキャスト
        return Physics.CapsuleCast(controllerBottomTransform.position - controllerBottomTransform.up * controllerBottomTransform.lossyScale.y / 2f, controllerBottomTransform.position + controllerBottomTransform.up * controllerBottomTransform.lossyScale.y / 2f, controllerBottomTransform.lossyScale.y / 2f, controllerBottomTransform.forward, (controllerBottomTransform.position - controllerPosition).magnitude, groundLayer);
        // return Physics.Raycast(controllerPosition - Vector3.up * 0.1f, Vector3.down, groundCheckDistance, groundLayer);
    }

    private void UpdateArmSwingMovement()
    {
        // 現在のコントローラー位置を取得
        Vector3 currentLeftPos = leftControllerTransform.localPosition;
        Vector3 currentRightPos = rightControllerTransform.localPosition;

        // 位置の変化量を計算
        Vector3 leftDelta = currentLeftPos - previousLeftPosition;
        Vector3 rightDelta = currentRightPos - previousRightPosition;

        // コントローラーが地面に接している場合のみアームスイングを適用
        bool leftGrounded = IsControllerGrounded(leftControllerTransform.position, leftControllerBottomTransform);
        bool rightGrounded = IsControllerGrounded(rightControllerTransform.position, rightControllerBottomTransform);

        if (leftGrounded || rightGrounded)
        {
            // 腕の振りの速さを計算
            float leftSwingSpeed = leftDelta.magnitude / Time.deltaTime;
            float rightSwingSpeed = rightDelta.magnitude / Time.deltaTime;
            

            // 閾値以上の速さで振った場合に移動力を加える
            if (leftGrounded) {
                if (leftSwingSpeed > armSwingThreshold) {
                    Vector3 moveDirection = -leftDelta.normalized;

                    // カメラの向き分回転
                    moveDirection = Quaternion.Euler(0, headTransform.rotation.eulerAngles.y, 0) * moveDirection;

                    moveVelocity += moveDirection * armSwingForce;

                    // 一定以上の速さで振った場合にジャンプ力を加える
                    if (leftSwingSpeed > jumpThreshold) {
                        moveVelocity += moveDirection * armSwingForce * jumpAddForce;
                    }
                }
            }
            if (rightGrounded) {
                if (rightSwingSpeed > armSwingThreshold) {
                    Vector3 moveDirection = -rightDelta.normalized;

                    // カメラの向き分回転
                    moveDirection = Quaternion.Euler(0, headTransform.rotation.eulerAngles.y, 0) * moveDirection;

                    moveVelocity += moveDirection * armSwingForce;

                    // 一定以上の速さで振った場合に力を二倍にする
                    if (rightSwingSpeed > jumpThreshold) {
                        moveVelocity += moveDirection * armSwingForce * jumpAddForce;
                    }
                }
            }

        }

        // 速度を制限
        moveVelocity = Vector3.ClampMagnitude(moveVelocity, maxSpeed);

        // 位置を更新
        previousLeftPosition = currentLeftPos;
        previousRightPosition = currentRightPos;

        // 摩擦を適用
        moveVelocity -= friction * moveVelocity * Time.deltaTime;
    }

    private void UpdateGravity()
    {
        // 重力の適用
        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        // ジャンプ処理
        // if (isGrounded && VRInputManager.Instance.LeftButtonPrimaryPressed)
        // {
        //     verticalVelocity = jumpPower;
        // }
    }

    private void ApplyMovement()
    {
        // 最終的な移動を適用
        Vector3 movement = moveVelocity;
        movement.y += verticalVelocity;
        characterController.Move(movement * Time.deltaTime);
    }

    /// <summary>
    /// 外部から速度を追加
    /// クライミングやジャンプなど、他のコンポーネントからの速度変更に使用
    /// </summary>
    /// <param name="velocity">追加する速度ベクトル</param>
    /// <remarks>
    /// 追加された速度は最大速度（maxSpeed）を超えないように制限されます。
    /// これはVR酔い防止のための重要な制限です。
    /// </remarks>
    public void AddVelocity(Vector3 velocity)
    {
        // 現在の速度に新しい速度を追加
        moveVelocity += velocity;
        // VR酔い防止のため、最大速度を制限
        moveVelocity = Vector3.ClampMagnitude(moveVelocity, maxSpeed);
    }
}

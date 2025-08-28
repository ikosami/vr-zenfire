using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// VRコントローラーを使用したプレイヤー移動システム
/// スティック入力とアームスイング（ゴリラタグ方式）の両方をサポート
/// </summary>
public class MovementController : MonoBehaviour
{
    [Header("移動設定")]
    [SerializeField] private CharacterController characterController;
    [SerializeField] private float moveSpeed = 5.0f;
    [SerializeField] private float maxSpeed = 10.0f;
    [SerializeField] private float friction = 0.9f;
    [SerializeField] private float jumpPower = 5.0f;

    [Header("アームスイング設定")]
    [SerializeField] private float armSwingForce = 2.0f;
    [SerializeField] private float armSwingThreshold = 1.0f;

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
        previousLeftPosition = VRInputManager.Instance.LeftControllerPos;
        previousRightPosition = VRInputManager.Instance.RightControllerPos;
    }

    private void Update()
    {
        UpdateGroundedState();
        UpdateStickMovement();
        UpdateArmSwingMovement();
        UpdateGravity();
        ApplyMovement();
    }

    private void UpdateGroundedState()
    {
        isGrounded = characterController.isGrounded;
        if (isGrounded && verticalVelocity < 0)
        {
            verticalVelocity = -0.5f; // 接地判定を安定させる
        }
    }

    private void UpdateStickMovement()
    {
        // スティック入力による移動
        Vector2 stickInput = VRInputManager.Instance.LeftStickValue;
        if (stickInput.magnitude > 0.1f)
        {
            // カメラの向きを基準に移動方向を決定
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 cameraRight = Camera.main.transform.right;
            cameraForward.y = 0;
            cameraRight.y = 0;
            cameraForward.Normalize();
            cameraRight.Normalize();

            moveVelocity = (cameraForward * stickInput.y + cameraRight * stickInput.x) * moveSpeed;
        }
    }

    private void UpdateArmSwingMovement()
    {
        // 現在のコントローラー位置を取得
        Vector3 currentLeftPos = VRInputManager.Instance.LeftControllerPos;
        Vector3 currentRightPos = VRInputManager.Instance.RightControllerPos;

        // 位置の変化量を計算
        Vector3 leftDelta = currentLeftPos - previousLeftPosition;
        Vector3 rightDelta = currentRightPos - previousRightPosition;

        // トリガーが押されている場合のみアームスイングを適用
        if (VRInputManager.Instance.IsTriggerPressed(ControllerSide.Left, true) ||
            VRInputManager.Instance.IsTriggerPressed(ControllerSide.Right, true))
        {
            // 腕の振りの速さを計算
            float leftSwingSpeed = leftDelta.magnitude / Time.deltaTime;
            float rightSwingSpeed = rightDelta.magnitude / Time.deltaTime;

            // 閾値以上の速さで振った場合に移動力を加える
            if (leftSwingSpeed > armSwingThreshold || rightSwingSpeed > armSwingThreshold)
            {
                Vector3 moveDirection = transform.forward;
                moveVelocity += moveDirection * armSwingForce;
            }
        }

        // 速度を制限
        moveVelocity = Vector3.ClampMagnitude(moveVelocity, maxSpeed);

        // 位置を更新
        previousLeftPosition = currentLeftPos;
        previousRightPosition = currentRightPos;

        // 摩擦を適用
        moveVelocity *= friction;
    }

    private void UpdateGravity()
    {
        // 重力の適用
        verticalVelocity += Physics.gravity.y * Time.deltaTime;

        // ジャンプ処理
        if (isGrounded && VRInputManager.Instance.LeftButtonPrimaryPressed)
        {
            verticalVelocity = jumpPower;
        }
    }

    private void ApplyMovement()
    {
        // 最終的な移動を適用
        Vector3 movement = moveVelocity;
        movement.y = verticalVelocity;
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

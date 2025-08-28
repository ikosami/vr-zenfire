using UnityEngine;

public class PlayerCar : MonoBehaviour
{
    public Rigidbody rb;
    [SerializeField] float acceleration = 10f;
    [SerializeField] float turnSpeed = 100f;
    [SerializeField] float maxSpeed = 20f;
    public bool IsLock;

    // 入力値保持
    float steerInput;
    float motorInput;
    public bool isPlayer;
    public Transform playerPos;
    public Transform downPos;
    [SerializeField] HandAttack handAttack;
    public float lastRemoveTime = 0;

    bool isTriggerPressRight = false;

    public void SetPlayer(bool isPlayer)
    {
        this.isPlayer = isPlayer;
        handAttack.enabled = isPlayer;

        isTriggerPressRight = true;

        rb.linearVelocity = Vector3.zero;
    }

    void Update()
    {
        if (!isPlayer)
        {
            steerInput = 0;
            motorInput = 0f;
            return;
        }

        if (VRInputManager.Instance.IsTriggerPressed(ControllerSide.Right, false))
        {
            if (!isTriggerPressRight)
            {
                isTriggerPressRight = true;
                PlayerController.Instance.RemoveCar(this);
                lastRemoveTime = Time.time;
            }
        }
        else
        {
            isTriggerPressRight = false;
        }

        // スティック入力でステアリング
        Vector2 stick = VRInputManager.Instance.LeftStickValue;
        steerInput = stick.x;

        // 前進・後退ボタン入力
        if (VRInputManager.Instance.RightButtonPrimaryPressed)
            motorInput = 1f;
        else if (VRInputManager.Instance.RightButtonSecondaryPressed)
            motorInput = -1f;
        else
            motorInput = 0f;
    }

    void FixedUpdate()
    {
        if (!isPlayer)
        {
            return;
        }

        // 加速・ブレーキ
        if (motorInput != 0f)
        {
            rb.AddForce(transform.forward * motorInput * acceleration, ForceMode.Acceleration);
        }

        Vector3 localVelocity = transform.InverseTransformDirection(rb.linearVelocity);
        localVelocity.x *= 0.96f; // 横方向を減衰（0に近づけるほど摩擦強くなる）
        rb.linearVelocity = transform.TransformDirection(localVelocity);

        var velocity = rb.linearVelocity;
        if (motorInput != 0f)
        {
            velocity = velocity.normalized * velocity.magnitude * 0.999f;
        }

        //上は飛ばない
        if (velocity.y > 0)
            velocity.y *= 0.1f;
        rb.linearVelocity = velocity;

        // 最高速度の制限
        if (rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }

        // 車速に応じたステアリング
        Vector3 horizVel = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);
        float speed = horizVel.magnitude;
        float speedFactor = Mathf.Clamp01(speed / maxSpeed);
        if (Mathf.Abs(steerInput) > 0f && speed > 0.1f)
        {
            float turn = steerInput * turnSpeed * Time.fixedDeltaTime * speedFactor;
            rb.MoveRotation(rb.rotation * Quaternion.Euler(0f, turn, 0f));
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
        }
    }
}

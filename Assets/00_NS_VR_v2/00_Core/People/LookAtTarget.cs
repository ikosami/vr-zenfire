using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtTarget : MonoBehaviour
{
     public Transform target;
    public Transform head;
    public Vector3 rotate;
    public float maxAngle = 90f; // 正面からの最大回転角度
    float maxDistance = 5f; // ターゲットとの最大距離

    Quaternion _headRotation;

    void Awake() {
        rotate = head.rotation.eulerAngles;
        _headRotation = head.rotation;
    }

    void LateUpdate()
    {
        if (!enabled) return;

        // ターゲットとの距離を計算
        float distance = Vector3.Distance(target.position, head.position);

        // 距離が一定以内でない場合は何もしない
        if (distance > maxDistance) {
            head.rotation = Quaternion.Slerp(_headRotation, head.rotation, Time.deltaTime * 10f);
            _headRotation = head.rotation;
            return;
        }

        // プレイヤーの方向ベクトル
        Vector3 direction = (target.position - head.position).normalized;

        // 現在の前方ベクトル
        Vector3 forward = transform.forward;

        // 角度を計算
        float angle = Vector3.Angle(forward, direction);

        // 一定角度以内なら向く
        if (angle < maxAngle)
        {
            var q = Quaternion.LookRotation(target.position - head.position);

            q *= Quaternion.Euler(rotate.x, rotate.y, rotate.z);

            // 徐々に回転
            head.rotation = Quaternion.Slerp(_headRotation, q, Time.deltaTime * 10f);
            _headRotation = head.rotation;
        }else {
            head.rotation = Quaternion.Slerp(_headRotation, head.rotation, Time.deltaTime * 10f);
            _headRotation = head.rotation;
        }
    }

    public void SetTarget(Transform target)
    {
        this.target = target;
    }

    public void SetActive(bool isActive)
    {
        enabled = isActive;
        if (!isActive)
        {
            head.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
}

using UnityEngine;
using System.Collections.Generic;
using System;

public class VRButton : MonoBehaviour
{
    [SerializeField] Collider mainBodyCollider;
    [SerializeField] CollisionChecker collisionChecker;
    [SerializeField] float pushActionRate = 0.8f;
    [SerializeField] float maxPressDistance = 0.05f;
    [SerializeField] float returnSpeed = 5f;

    public bool isPressed = false;
    public Action OnPressed;
    public Action OnReleased;
    [SerializeField] string handTag = "Hand";


    // 「親オブジェクト基準」の初期ローカル位置
    Vector3 initialLocalPos;

    // ボタン上端(親基準)のY座標
    float buttonTopY;

    // 近くにいる手
    List<Collider> nearHands = new List<Collider>();

    void Start()
    {
        if (mainBodyCollider == null)
        {
            Debug.LogError("mainBodyColliderが設定されていません。");
            enabled = false;
            return;
        }

        // 親基準の初期ローカル位置
        initialLocalPos = mainBodyCollider.transform.localPosition;

        // ─────────────────────────────────────────
        // ボタン上端(ワールド座標)を「Collider.bounds.max」から求める
        // それを親Transform基準のローカル座標に変換し、Yだけ覚える
        // ─────────────────────────────────────────
        Bounds btnBounds = mainBodyCollider.bounds;
        Vector3 topWorld = new Vector3(btnBounds.center.x, btnBounds.max.y, btnBounds.center.z);
        Transform parentT = mainBodyCollider.transform.parent;
        Vector3 topLocal = parentT.InverseTransformPoint(topWorld);
        buttonTopY = topLocal.y;

        if (collisionChecker != null)
        {
            collisionChecker.OnTriggerEnterAction += other =>
            {
                //Debug.LogError("other " + other.gameObject + " " + (other.CompareTag(handTag) && !nearHands.Contains(other)));
                if (other.CompareTag(handTag) && !nearHands.Contains(other))
                    nearHands.Add(other);
            }
                ;
            collisionChecker.OnTriggerExitAction += other =>
            {
                if (nearHands.Contains(other))
                    nearHands.Remove(other);
            };
        }
    }

    void Update()
    {
        if (mainBodyCollider == null) return;

        float targetY = initialLocalPos.y;

        if (nearHands.Count > 0)
        {
            float mostPushedOffset = 0f;
            bool first = true;

            foreach (var hand in nearHands)
            {
                // 手のバウンディングボックス下端(ワールド座標)
                Bounds handBounds = hand.bounds;
                Vector3 handBottomWorld = handBounds.min;

                // 親基準ローカルに変換
                Transform parentT = mainBodyCollider.transform.parent;
                Vector3 handBottomLocal = parentT.InverseTransformPoint(handBottomWorld);

                // ボタン上端(=buttonTopY)からどれだけ下がっているか
                float offset = handBottomLocal.y - buttonTopY; // 例: -0.01f など

                if (first || offset < mostPushedOffset)
                {
                    mostPushedOffset = offset;
                    first = false;
                }
            }

            // 上端からの押し込み量を -maxPressDistance～0 にClamp
            float clamped = Mathf.Clamp(mostPushedOffset, -maxPressDistance, 0f);

            // 初期ローカル位置.y + 押し込み量
            targetY = initialLocalPos.y + clamped;
        }
        else
        {
            // 手がいなければ一定速度で戻る
            float currentY = mainBodyCollider.transform.localPosition.y;
            targetY = Mathf.Lerp(currentY, initialLocalPos.y, returnSpeed * Time.deltaTime);
        }

        // 実際にボタンを動かす
        Vector3 pos = mainBodyCollider.transform.localPosition;
        pos.y = targetY;
        mainBodyCollider.transform.localPosition = pos;

        // 押下/解放判定
        float offsetFromInit = pos.y - initialLocalPos.y;

        // 押下閾値
        float pressThreshold = -maxPressDistance * pushActionRate;

        if (!isPressed && offsetFromInit <= pressThreshold)
        {
            isPressed = true;
            OnPressed?.Invoke();
        }
        else if (isPressed && offsetFromInit > pressThreshold)
        {
            isPressed = false;
            OnReleased?.Invoke();
        }
    }
}

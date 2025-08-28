using UnityEngine;

/// <summary>
/// 手の物理的な衝突を処理し、壁を貫通しないようにするシステム
/// FixedJointを使用して手の動きを制限
/// </summary>
public class HandCollisionHandler : MonoBehaviour
{
    [Header("コリジョン設定")]
    [SerializeField] private LayerMask wallLayers;
    [SerializeField] private float jointBreakForce = 2000f;
    [SerializeField] private float jointBreakTorque = 2000f;

    private FixedJoint fixedJoint;
    private Rigidbody handRigidbody;
    private bool isColliding = false;

    private void Start()
    {
        // 必要なコンポーネントを取得または追加
        handRigidbody = GetComponent<Rigidbody>();
        if (handRigidbody == null)
        {
            handRigidbody = gameObject.AddComponent<Rigidbody>();
            handRigidbody.isKinematic = true;
            handRigidbody.useGravity = false;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        // 壁のレイヤーとの衝突をチェック
        if (((1 << collision.gameObject.layer) & wallLayers) != 0)
        {
            if (!isColliding)
            {
                isColliding = true;
                CreateFixedJoint(collision);
            }
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        // 壁との接触が終わった場合
        if (((1 << collision.gameObject.layer) & wallLayers) != 0)
        {
            if (isColliding)
            {
                isColliding = false;
                DestroyFixedJoint();
            }
        }
    }

    private void CreateFixedJoint(Collision collision)
    {
        // 既存のジョイントを破棄
        DestroyFixedJoint();

        // 新しいFixedJointを作成
        fixedJoint = gameObject.AddComponent<FixedJoint>();
        fixedJoint.connectedBody = collision.rigidbody;
        fixedJoint.breakForce = jointBreakForce;
        fixedJoint.breakTorque = jointBreakTorque;

        // 衝突点の位置にジョイントを設定
        if (collision.contactCount > 0)
        {
            Vector3 contactPoint = collision.GetContact(0).point;
            fixedJoint.anchor = transform.InverseTransformPoint(contactPoint);
        }
    }

    private void DestroyFixedJoint()
    {
        if (fixedJoint != null)
        {
            Destroy(fixedJoint);
            fixedJoint = null;
        }
    }

    private void OnJointBreak(float breakForce)
    {
        // ジョイントが破壊された場合の処理
        isColliding = false;
        fixedJoint = null;
    }

    private void OnDisable()
    {
        // コンポーネントが無効化された時にジョイントを破棄
        DestroyFixedJoint();
    }
}

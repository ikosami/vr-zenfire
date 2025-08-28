using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// 手の物理的な衝突を処理し、壁を貫通しないようにするシステム
/// FixedJointを使用して手の動きを制限し、持っているオブジェクトの衝突も処理
/// </summary>
public class HandCollisionHandler : MonoBehaviour
{
    [Header("コリジョン設定")]
    [SerializeField] private LayerMask wallLayers;
    [SerializeField] private float jointBreakForce = 2000f;
    [SerializeField] private float jointBreakTorque = 2000f;
    
    [Header("Grab設定")]
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactors.XRDirectInteractor interactor;
    
    private FixedJoint fixedJoint;
    private Rigidbody handRigidbody;
    private bool isColliding = false;
    private UnityEngine.XR.Interaction.Toolkit.Interactables.IXRSelectInteractable heldObject;
    private Rigidbody heldObjectRigidbody;

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

        if (interactor != null)
        {
            interactor.selectEntered.AddListener(OnGrab);
            interactor.selectExited.AddListener(OnRelease);
        }
    }

    private void OnGrab(SelectEnterEventArgs args)
    {
        heldObject = args.interactableObject;
        if (heldObject != null)
        {
            heldObjectRigidbody = heldObject.transform.GetComponent<Rigidbody>();
        }
    }

    private void OnRelease(SelectExitEventArgs args)
    {
        heldObject = null;
        heldObjectRigidbody = null;
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

                // 持っているオブジェクトがある場合、そのオブジェクトにも衝突処理を適用
                if (heldObjectRigidbody != null)
                {
                    PhysicsInteractionManager.Instance.CreateJoint(
                        heldObjectRigidbody,
                        collision.rigidbody,
                        collision.GetContact(0).point
                    );
                }
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

                // 持っているオブジェクトの衝突ジョイントを解除
                if (heldObjectRigidbody != null)
                {
                    var joints = heldObjectRigidbody.GetComponents<ConfigurableJoint>();
                    foreach (var joint in joints)
                    {
                        if (joint.connectedBody == collision.rigidbody)
                        {
                            PhysicsInteractionManager.Instance.RemoveJoint(joint);
                        }
                    }
                }
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
        
        // Cleanup event listeners
        if (interactor != null)
        {
            interactor.selectEntered.RemoveListener(OnGrab);
            interactor.selectExited.RemoveListener(OnRelease);
        }
    }
}

using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Handles climbing mechanics for VR interactions
/// Extends movement system with climbing capabilities
/// </summary>
public class ClimbingController : MonoBehaviour
{
    [Header("Climbing Settings")]
    [SerializeField] private MovementController movementController;
    [SerializeField] private float climbSpeed = 1f;
    [SerializeField] private float grabThreshold = 0.3f;
    [SerializeField] private LayerMask climbableLayers;
    [SerializeField] private float maxReachDistance = 1.0f;
    [SerializeField] private float releaseVelocityMultiplier = 1.5f;

    [Header("Hand References")]
    [SerializeField] private Transform leftHand;
    [SerializeField] private Transform rightHand;

    private bool isLeftHandGrabbing = false;
    private bool isRightHandGrabbing = false;
    private Vector3 leftGrabPoint;
    private Vector3 rightGrabPoint;
    private Vector3 previousLeftPosition;
    private Vector3 previousRightPosition;
    private CharacterController characterController;

    private void Start()
    {
        if (movementController == null)
        {
            movementController = GetComponent<MovementController>();
        }
        characterController = GetComponent<CharacterController>();
        
        previousLeftPosition = leftHand.position;
        previousRightPosition = rightHand.position;
    }

    private void Update()
    {
        UpdateHandPositions();
        CheckGrabbing();
        HandleClimbingMovement();
    }

    /// <summary>
    /// Update hand position tracking
    /// </summary>
    private void UpdateHandPositions()
    {
        previousLeftPosition = leftHand.position;
        previousRightPosition = rightHand.position;
    }

    /// <summary>
    /// Check for grabbing interactions with climbable surfaces
    /// </summary>
    /// <summary>
    /// 両手のグラブ状態をチェックし、クライミング可能なオブジェクトとの相互作用を処理
    /// </summary>
    private void CheckGrabbing()
    {
        // 左手のグラブチェック
        // Raycastを使用して、手の前方にクライミング可能なオブジェクトがあるかを確認
        if (Physics.Raycast(leftHand.position, leftHand.forward, out RaycastHit leftHit, maxReachDistance, climbableLayers))
        {
            // 中指トリガーの値がしきい値を超えた場合、グラブ開始
            if (VRInputManager.Instance.LeftMiddleTriggerValue > grabThreshold)
            {
                if (!isLeftHandGrabbing)
                {
                    // グラブ位置を記録し、グラブ状態を有効化
                    leftGrabPoint = leftHit.point;
                    isLeftHandGrabbing = true;
                }
            }
            else
            {
                isLeftHandGrabbing = false;
            }
        }
        else
        {
            isLeftHandGrabbing = false;
        }

        // Right hand grab check
        if (Physics.Raycast(rightHand.position, rightHand.forward, out RaycastHit rightHit, maxReachDistance, climbableLayers))
        {
            if (VRInputManager.Instance.RightMiddleTriggerValue > grabThreshold)
            {
                if (!isRightHandGrabbing)
                {
                    rightGrabPoint = rightHit.point;
                    isRightHandGrabbing = true;
                }
            }
            else
            {
                isRightHandGrabbing = false;
            }
        }
        else
        {
            isRightHandGrabbing = false;
        }
    }

    /// <summary>
    /// Handle climbing movement based on hand positions
    /// </summary>
    /// <summary>
    /// クライミング中の移動を処理
    /// 手の動きに基づいて物理的な移動を計算し適用
    /// </summary>
    private void HandleClimbingMovement()
    {
        // クライミング状態でない場合は通常の移動を有効化
        if (!isLeftHandGrabbing && !isRightHandGrabbing)
        {
            movementController.enabled = true;
            return;
        }

        // クライミング中は通常の移動を無効化
        movementController.enabled = false;
        Vector3 movement = Vector3.zero;

        if (isLeftHandGrabbing)
        {
            movement += CalculateClimbingMovement(leftHand.position - previousLeftPosition);
        }

        if (isRightHandGrabbing)
        {
            movement += CalculateClimbingMovement(rightHand.position - previousRightPosition);
        }

        // Apply climbing movement
        if (movement.magnitude > 0)
        {
            movement *= climbSpeed;
            characterController.Move(-movement);
        }
    }

    /// <summary>
    /// Calculate climbing movement based on hand movement
    /// </summary>
    /// <summary>
    /// 手の移動量から実際のクライミング移動量を計算
    /// </summary>
    /// <param name="handDelta">手の位置の変化量</param>
    /// <returns>計算された移動量</returns>
    /// <remarks>
    /// VR酔い防止のため、クライミング面に対する移動を適切に制限します。
    /// 将来的な拡張として、クライミング面の法線に基づく移動制限を実装予定。
    /// </remarks>
    private Vector3 CalculateClimbingMovement(Vector3 handDelta)
    {
        // クライミング面の向きに基づいて移動を調整
        // 現在は単純な実装だが、将来的にはより複雑な移動制限を追加予定
        return handDelta;
    }

    /// <summary>
    /// Release from climbing with velocity
    /// </summary>
    public void ReleaseWithVelocity(Vector3 velocity)
    {
        if (isLeftHandGrabbing || isRightHandGrabbing)
        {
            movementController.enabled = true;
            movementController.AddVelocity(velocity * releaseVelocityMultiplier);
            isLeftHandGrabbing = false;
            isRightHandGrabbing = false;
        }
    }

    /// <summary>
    /// Check if currently climbing
    /// </summary>
    public bool IsClimbing()
    {
        return isLeftHandGrabbing || isRightHandGrabbing;
    }

    private void OnDrawGizmos()
    {
        if (leftHand != null && rightHand != null)
        {
            // Draw grab points
            Gizmos.color = Color.yellow;
            if (isLeftHandGrabbing)
            {
                Gizmos.DrawWireSphere(leftGrabPoint, 0.05f);
                Gizmos.DrawLine(leftHand.position, leftGrabPoint);
            }
            if (isRightHandGrabbing)
            {
                Gizmos.DrawWireSphere(rightGrabPoint, 0.05f);
                Gizmos.DrawLine(rightHand.position, rightGrabPoint);
            }

            // Draw reach distance
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(leftHand.position, maxReachDistance);
            Gizmos.DrawWireSphere(rightHand.position, maxReachDistance);
        }
    }
}

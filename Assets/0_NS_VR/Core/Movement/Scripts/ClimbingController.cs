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
    private void CheckGrabbing()
    {
        // Left hand grab check
        if (Physics.Raycast(leftHand.position, leftHand.forward, out RaycastHit leftHit, maxReachDistance, climbableLayers))
        {
            if (VRInputManager.Instance.LeftMiddleTriggerValue > grabThreshold)
            {
                if (!isLeftHandGrabbing)
                {
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
    private void HandleClimbingMovement()
    {
        if (!isLeftHandGrabbing && !isRightHandGrabbing)
        {
            movementController.enabled = true;
            return;
        }

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
    private Vector3 CalculateClimbingMovement(Vector3 handDelta)
    {
        // Remove any movement along the forward axis of the climbing surface
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

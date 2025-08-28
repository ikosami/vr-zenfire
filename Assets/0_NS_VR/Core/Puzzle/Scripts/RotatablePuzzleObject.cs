using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

/// <summary>
/// Represents a puzzle object that can be rotated in 3D space
/// Handles rotation constraints and snap angles
/// </summary>
public class RotatablePuzzleObject : PuzzleState
{
    [Header("Rotation Settings")]
    [SerializeField] private Vector3 rotationAxis = Vector3.forward;
    [SerializeField] private float snapAngle = 90f;
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float snapThreshold = 5f;
    [SerializeField] private bool useLocalRotation = true;

    [Header("Interaction Settings")]
    [SerializeField] private XRGrabInteractable grabInteractable;
    [SerializeField] private float minRotationDelta = 1f;

    private Quaternion targetRotation;
    private Vector3 previousRotation;
    private float currentAngle;
    private bool isGrabbed;

    protected override void Start()
    {
        base.Start();
        
        if (grabInteractable == null)
        {
            grabInteractable = GetComponent<XRGrabInteractable>();
        }

        // Setup interaction events
        grabInteractable.selectEntered.AddListener(OnGrabStart);
        grabInteractable.selectExited.AddListener(OnGrabEnd);

        // Initialize rotation
        previousRotation = useLocalRotation ? transform.localEulerAngles : transform.eulerAngles;
        targetRotation = useLocalRotation ? transform.localRotation : transform.rotation;
    }

    private void Update()
    {
        if (!isGrabbed)
        {
            // Smooth rotation to target when not grabbed
            if (useLocalRotation)
            {
                transform.localRotation = Quaternion.Lerp(transform.localRotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
            else
            {
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }
        }
        else
        {
            // Track rotation changes while grabbed
            Vector3 currentRotation = useLocalRotation ? transform.localEulerAngles : transform.eulerAngles;
            Vector3 rotationDelta = currentRotation - previousRotation;

            // Calculate rotation around axis
            float angleDelta = Vector3.Dot(rotationDelta, rotationAxis);
            if (Mathf.Abs(angleDelta) > minRotationDelta)
            {
                currentAngle += angleDelta;
                UpdateState("rotation", currentAngle);
            }

            previousRotation = currentRotation;
        }
    }

    private void OnGrabStart(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        previousRotation = useLocalRotation ? transform.localEulerAngles : transform.eulerAngles;
    }

    private void OnGrabEnd(SelectExitEventArgs args)
    {
        isGrabbed = false;
        
        // Snap to nearest angle
        float snappedAngle = Mathf.Round(currentAngle / snapAngle) * snapAngle;
        if (Mathf.Abs(currentAngle - snappedAngle) < snapThreshold)
        {
            currentAngle = snappedAngle;
            Quaternion snapRotation = Quaternion.Euler(rotationAxis * snappedAngle);
            targetRotation = useLocalRotation ? snapRotation : transform.parent ? transform.parent.rotation * snapRotation : snapRotation;
            
            UpdateState("rotation", currentAngle);
        }
    }

    protected override bool CheckCompletionConditions()
    {
        // Override in derived classes to define specific completion conditions
        return false;
    }

    private void OnDestroy()
    {
        if (grabInteractable != null)
        {
            grabInteractable.selectEntered.RemoveListener(OnGrabStart);
            grabInteractable.selectExited.RemoveListener(OnGrabEnd);
        }
    }
}

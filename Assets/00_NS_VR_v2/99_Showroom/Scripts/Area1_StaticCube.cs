using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Area1_StaticCube : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
{
    protected override void Awake()
    {
        base.Awake();
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = true;
        }
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        transform.SetParent(null);
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }
}

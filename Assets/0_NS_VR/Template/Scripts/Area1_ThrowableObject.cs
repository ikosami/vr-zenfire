using UnityEngine;


public class Area1_ThrowableObject : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
{
    protected override void Awake()
    {
        base.Awake();
        var rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
        }
    }
}

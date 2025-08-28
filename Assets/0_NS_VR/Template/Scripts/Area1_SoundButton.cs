using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class Area1_SoundButton : UnityEngine.XR.Interaction.Toolkit.Interactables.XRBaseInteractable
{
    [SerializeField] private AudioSource buttonSound;
    [SerializeField] private float pressThreshold = 0.1f;
    private bool isPressed = false;
    private float initialHeight;
    private Transform buttonTransform;

    protected override void Awake()
    {
        base.Awake();
        buttonTransform = transform.Find("Button");
        if (buttonTransform != null)
        {
            initialHeight = buttonTransform.localPosition.y;
        }
    }

    public override void ProcessInteractable(XRInteractionUpdateOrder.UpdatePhase updatePhase)
    {
        base.ProcessInteractable(updatePhase);

        if (updatePhase == XRInteractionUpdateOrder.UpdatePhase.Dynamic)
        {
            if (buttonTransform != null)
            {
                float currentHeight = buttonTransform.localPosition.y;
                bool shouldBePressed = currentHeight <= (initialHeight - pressThreshold);

                if (shouldBePressed && !isPressed)
                {
                    isPressed = true;
                    if (buttonSound != null)
                    {
                        buttonSound.Play();
                    }
                }
                else if (!shouldBePressed && isPressed)
                {
                    isPressed = false;
                }
            }
        }
    }
}

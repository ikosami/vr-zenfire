using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ev;
using Ev.Events;

public class Area1_InteractionManager : MonoBehaviour
{
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable throwableSphere;
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable staticCube;
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable mugWithPose;
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable gun;
    [SerializeField] private UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable ketchupBottle;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject ketchupDecalPrefab;
    [SerializeField] private AudioSource buttonSound;
    private void Start()
    {
        if (throwableSphere != null)
        {
            var rb = throwableSphere.GetComponent<Rigidbody>();
            if (rb != null) rb.useGravity = true;
        }

        if (mugWithPose != null)
        {
            mugWithPose.selectEntered.AddListener(OnMugGrabbed);
            mugWithPose.selectExited.AddListener(OnMugReleased);
        }

        if (gun != null)
        {
            gun.activated.AddListener(OnGunTriggerPressed);
        }

        if (ketchupBottle != null)
        {
            ketchupBottle.activated.AddListener(OnKetchupTriggerPressed);
            ketchupBottle.selectExited.AddListener(OnKetchupTriggerReleased);
        }
    }

    private void OnMugGrabbed(SelectEnterEventArgs args)
    {
        var controllerSide = args.interactorObject.transform.CompareTag("RightHand") ? ControllerSide.Right : ControllerSide.Left;
        EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, 
            new Ev.Events.HandPoseChange() { 
                PoseName = "GrabMug",
                ControllerSide = controllerSide,
                HandPartType = HandPartType.All
            });
    }

    private void OnMugReleased(SelectExitEventArgs args)
    {
        var controllerSide = args.interactorObject.transform.CompareTag("RightHand") ? ControllerSide.Right : ControllerSide.Left;
        EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_RESET,
            new Ev.Events.HandPoseReset() {
                ControllerSide = controllerSide,
                HandPartType = HandPartType.All
            });
    }

    private void OnGunTriggerPressed(ActivateEventArgs args)
    {
        if (bulletPrefab != null)
        {
            var bullet = Instantiate(bulletPrefab, gun.transform.position, gun.transform.rotation);
            var rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddForce(gun.transform.forward * 20f, ForceMode.Impulse);
                Destroy(bullet, 2f);
            }
        }
    }

    private void OnKetchupTriggerPressed(ActivateEventArgs args)
    {
        var waterFlow = ketchupBottle.GetComponent<WaterFlowCustom>();
        if (waterFlow != null)
        {
            waterFlow.StartGenerating();
        }
    }

    private void OnKetchupTriggerReleased(SelectExitEventArgs args)
    {
        var waterFlow = ketchupBottle.GetComponent<WaterFlowCustom>();
        if (waterFlow != null)
        {
            waterFlow.StopGenerating();
        }
    }

    public void PlayButtonSound()
    {
        if (buttonSound != null)
        {
            buttonSound.Play();
        }
    }
}

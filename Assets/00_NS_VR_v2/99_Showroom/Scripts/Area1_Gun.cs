using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using Ev;
using Ev.Events;

public class Area1_Gun : UnityEngine.XR.Interaction.Toolkit.Interactables.XRGrabInteractable
{
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform bulletSpawnPoint;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private string grabPoseName = "GrabGun";
    [SerializeField] private string defaultPoseName = "Default";
    protected override void OnSelectEntered(SelectEnterEventArgs args)
    {
        base.OnSelectEntered(args);
        var controllerSide = args.interactorObject.transform.CompareTag("RightHand") ? ControllerSide.Right : ControllerSide.Left;
        EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, 
            new Ev.Events.HandPoseChange() { 
                PoseName = grabPoseName,
                ControllerSide = controllerSide,
                HandPartType = HandPartType.All
            });
    }

    protected override void OnSelectExited(SelectExitEventArgs args)
    {
        base.OnSelectExited(args);
        var controllerSide = args.interactorObject.transform.CompareTag("RightHand") ? ControllerSide.Right : ControllerSide.Left;
        EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_RESET,
            new Ev.Events.HandPoseReset() {
                ControllerSide = controllerSide,
                HandPartType = HandPartType.All
            });
    }

    protected override void OnActivated(ActivateEventArgs args)
    {
        base.OnActivated(args);
        Shoot();
    }

    private void Shoot()
    {
        if (bulletPrefab != null && bulletSpawnPoint != null)
        {
            GameObject bullet = Instantiate(bulletPrefab, bulletSpawnPoint.position, bulletSpawnPoint.rotation);
            Rigidbody rb = bullet.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.linearVelocity = bulletSpawnPoint.forward * bulletSpeed;
                Destroy(bullet, 2f);
            }
        }
    }
}

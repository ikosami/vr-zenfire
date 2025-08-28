using Ev;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.OpenXR.Input;

public class Gun : GrabbableAction
{
    string _se = "gun_fire";

    string _poseName = "Gun";
    string _poseName2 = "Gun2";

    [SerializeField] Transform _spawnPoint;
    //[SerializeField] GameObject _bulletPrefab;
    [SerializeField] Transform _fireEffectSpawnPoint;
    [SerializeField] GameObject _fireEffect;
    
    // 連射速度
    [SerializeField] float _fireRate = 0.5f;
    float timer = 0;
    float poseBackTimer = 0;

    public System.Action<RaycastHit> OnHit;

    void Update()
    {
        if (!isGrab) return;

        if (VRInputManager.Instance.IsTriggerPressed(side, true))
        {
            if (timer + _fireRate < Time.time)
            {
                timer = Time.time;
                Shoot();
            }
        }

        if (poseBackTimer > 0)
        {
            poseBackTimer -= Time.deltaTime;
            if (poseBackTimer <= 0)
            {
                EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, new Ev.Events.HandPoseChange() { PoseName = _poseName, ControllerSide = side, HandPartType = HandPartType.All });
            }
        }
    }

    public void Shoot()
    {
        poseBackTimer = 0.3f;
        EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, new Ev.Events.HandPoseChange() { PoseName = _poseName2, ControllerSide = side, HandPartType = HandPartType.All });

        SoundManager.Instance.Play3D(_se, transform.position, index: 0, time: 0.1f);

        //GameObject bullet = Instantiate(_bulletPrefab, _spawnPoint.position, _spawnPoint.rotation);
        //bullet.GetComponent<Rigidbody>().AddForce(_spawnPoint.forward * 800);
        Instantiate(_fireEffect, _fireEffectSpawnPoint.position, Quaternion.identity).transform.localScale *= 0.3f;
        VibrationController.Instance.Play("click", side);

        RaycastHit hit;
        if (Physics.Raycast(_spawnPoint.position, _spawnPoint.forward, out hit))
        {
            Instantiate(_fireEffect, hit.point, Quaternion.identity);
            OnHit?.Invoke(hit);
        }

    }

    public override void OnGrab(SelectEnterEventArgs args, ControllerSide controllerSide)
    {
        side = controllerSide;
        isGrab = true;

        EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, new Ev.Events.HandPoseChange() { PoseName = _poseName, ControllerSide = side, HandPartType = HandPartType.All });
        EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_LOCK, new Ev.Events.HandPoseLock() { ControllerSide = side, HandPartType = HandPartType.All });
    }
    public override void OnRelease(SelectExitEventArgs args, ControllerSide controllerSide)
    {
        side = controllerSide;
        isGrab = false;
        EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_RESET, new Ev.Events.HandPoseReset() { ControllerSide = side, HandPartType = HandPartType.All });
        EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_UNLOCK, new Ev.Events.HandPoseUnlock() { ControllerSide = side, HandPartType = HandPartType.All });
    }
}

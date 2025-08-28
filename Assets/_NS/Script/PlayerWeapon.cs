using Ev;
using System;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    WeaponData[] weapons => References.Instance.WeaponDatas.Weapons;
    int currentWeaponIndex = -1;

    GameObject haveObj;
    GunDataComponent currentWeapon;
    GameObject heldItemPrefab;// 持ち物用Prefab

    ControllerSide controllerSide = ControllerSide.Right;

    void Start()
    {
        ChangeWeapon();
    }

    bool PrimaryPressed = false;
    bool SecondaryPressed = false;

    void ChangeWeapon()
    {
        if (currentWeapon != null)
        {
            Destroy(currentWeapon.gameObject);
            currentWeapon = null;
        }
        if (haveObj != null)
        {
            Destroy(haveObj);
            haveObj = null;
        }

        if (currentWeaponIndex == -1)
        {
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_RESET, new Ev.Events.HandPoseReset() { ControllerSide = controllerSide, HandPartType = HandPartType.All });
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_UNLOCK, new Ev.Events.HandPoseLock() { ControllerSide = controllerSide, HandPartType = HandPartType.All });
            return; // 装備なし
        }

        if (currentWeaponIndex == -2)
        {
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_RESET, new Ev.Events.HandPoseReset() { ControllerSide = controllerSide, HandPartType = HandPartType.All });
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_UNLOCK, new Ev.Events.HandPoseLock() { ControllerSide = controllerSide, HandPartType = HandPartType.All });
            if (heldItemPrefab != null)
            {
                haveObj = Instantiate(heldItemPrefab, transform);
            }
            return;
        }



        var data = weapons[currentWeaponIndex];
        haveObj = Instantiate(data.WeaponObj, transform);
        currentWeapon = haveObj.GetComponentInChildren<GunDataComponent>();
        currentWeapon.SetSetting(data);
    }

    void Update()
    {


        int change = 0;
        if (VRInputManager.Instance.RightButtonPrimaryPressed)
        {
            if (PrimaryPressed) return;
            PrimaryPressed = true;
            change = 1;
        }
        else
        {
            PrimaryPressed = false;
        }
        if (VRInputManager.Instance.RightButtonSecondaryPressed)
        {
            if (SecondaryPressed) return;
            SecondaryPressed = true;
            change = -1;
        }
        else
        {
            SecondaryPressed = false;
        }

        if (change == 0) return;

        do
        {
            currentWeaponIndex += change;

            if (currentWeaponIndex > weapons.Length - 1)
                currentWeaponIndex = -1;
            else if (currentWeaponIndex < -1)
                currentWeaponIndex = weapons.Length - 1;
        }
        while (currentWeaponIndex >= 0 && !weapons[currentWeaponIndex].IsUnLocked());

        ChangeWeapon();
    }

    internal void ChangeWeapon(string weaponName)
    {
        for (int i = 0; i < weapons.Length; i++)
        {
            if (weapons[i].WeaponName == weaponName)
            {
                currentWeaponIndex = i;
                break;
            }
        }
        ChangeWeapon();
    }

    // 任意のアイテムを手に持つ
    public void HoldItem(GameObject itemPrefab)
    {
        if (currentWeaponIndex == -2) return; // すでに持っている場合は何もしない
        heldItemPrefab = itemPrefab;
        currentWeaponIndex = -2;
        ChangeWeapon();
    }

    // 手に持っているアイテムを解除
    public void DropItem()
    {
        if (currentWeaponIndex != -2) return;
        heldItemPrefab = null;
        currentWeaponIndex = -1;
        ChangeWeapon();
    }

    public void SetCanShoot(bool CanShoot)
    {
        if (currentWeapon == null) return;
        currentWeapon.CanShoot = CanShoot;
    }
}

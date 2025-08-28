using System;
using UnityEngine;

public class WeaponDatas : ScriptableObject
{
    public WeaponData[] Weapons;

    public WeaponData FindWeapon(string itemName)
    {
        foreach (var weapon in Weapons)
        {
            if (weapon.WeaponName == itemName)
            {
                return weapon;
            }
        }
        return null;
    }
}

[Serializable]
public class WeaponData
{
    public string WeaponName;
    public GameObject WeaponObj;
    public int Cost = 1000;
    public int Speed = 50;
    public int Damage = 200;
    public float SpanTime = 10f;
    public string audioName;
    public int BulletNum = 1;

    public void UnLock()
    {
        PlayerPrefs.SetInt(WeaponName, 1);
    }
    public bool IsUnLocked()
    {
        return PlayerPrefs.GetInt(WeaponName, 0) == 1;
    }
}
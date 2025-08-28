using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSelector : MonoBehaviour
{
    public static WeaponSelector Instance { get; private set; }
    [SerializeField] WeaponData[] _weaponDatas;
    // Start is called before the first frame update
    void Start()
    {
        if(Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectWeapon(int index) {
        Debug.Log("SelectWeapon: " + index);
        foreach(var weaponData in _weaponDatas) {
            foreach(var weapon in weaponData._weapons) {
                weapon.SetActive(false);
            }
        }
        foreach(var weapon in _weaponDatas[index]._weapons) {
            weapon.SetActive(true);
        }
    }
}

[System.Serializable]
public class WeaponData {
    public GameObject[] _weapons;
}

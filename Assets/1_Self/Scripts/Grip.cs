using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grip : MonoBehaviour
{

    float _size = 1.0f;
    Collider _weapon;
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.Instance.OnLvChanged += OnLvChanged;
        OnLvChanged(PlayerData.Instance.GetLv(ParamType.WeaponSize));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnLvChanged(PlayerData.Lv lv) {
        if(lv.Key == ParamType.WeaponSize) {
            UpdateSize(lv);
        }
    }

    public void SetWeapon(Collider weapon) {
        if(_weapon != null) {
            Destroy(_weapon.gameObject);
        }
        _weapon = weapon;
        _weapon.transform.parent = transform;
        _weapon.transform.localPosition = Vector3.zero;
        _weapon.transform.localRotation = Quaternion.identity;
    }

    public void UpdateSize(PlayerData.Lv lv) {
        _size = lv.Value;
        transform.localScale = new Vector3(_size, _size, _size);
    }
}

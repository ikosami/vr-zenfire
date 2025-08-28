using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages weapon selection and switching in VR
/// </summary>
public class WeaponSelector : MonoBehaviour
{
    private static WeaponSelector _instance;
    public static WeaponSelector Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<WeaponSelector>();
                if (_instance == null)
                {
                    GameObject go = new GameObject("WeaponSelector");
                    _instance = go.AddComponent<WeaponSelector>();
                }
            }
            return _instance;
        }
    }

    [System.Serializable]
    public class WeaponData
    {
        public string Key;
        public WeaponBase Weapon;
    }

    [SerializeField] private WeaponData[] weaponDatas;
    private Dictionary<string, WeaponBase> weaponMap = new Dictionary<string, WeaponBase>();
    private string currentWeaponKey;

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);

        // Initialize weapon map
        foreach (var data in weaponDatas)
        {
            weaponMap[data.Key] = data.Weapon;
            data.Weapon.gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Select and activate a weapon by its key
    /// </summary>
    public void SelectWeapon(string key)
    {
        if (!weaponMap.ContainsKey(key))
        {
            Debug.LogWarning($"Weapon key not found: {key}");
            return;
        }

        if (currentWeaponKey == key) return;

        // Deactivate current weapon
        if (!string.IsNullOrEmpty(currentWeaponKey) && weaponMap.ContainsKey(currentWeaponKey))
        {
            weaponMap[currentWeaponKey].gameObject.SetActive(false);
        }

        // Activate new weapon
        weaponMap[key].gameObject.SetActive(true);
        currentWeaponKey = key;
    }

    /// <summary>
    /// Get the currently active weapon
    /// </summary>
    public WeaponBase GetCurrentWeapon()
    {
        return !string.IsNullOrEmpty(currentWeaponKey) ? weaponMap[currentWeaponKey] : null;
    }
}

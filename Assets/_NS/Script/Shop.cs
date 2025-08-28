using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] ShopKind shopKind;
    public enum ShopKind
    {
        Weapon,
        Car,
        Object
    }
    [SerializeField] string itemName;
    [SerializeField] GameObject item;

    [SerializeField] GameObject parent;
    [SerializeField] TextMeshProUGUI text;
    [SerializeField] Button buyButton;
    float multi = 1;
    public static Action<ShopKind, string> OnBuy;

    string SAVE_CAR_KEY = "Car_";
    string SAVE_OBJECT_KEY = "OBJECT_";

    WeaponDatas weaponDatas => References.Instance.WeaponDatas;

    public int Cost;

    void Start()
    {
        switch (shopKind)
        {
            case ShopKind.Weapon:
                {
                    var weapon = weaponDatas.FindWeapon(itemName);
                    Cost = (int)(weapon.Cost * multi);
                    if (weapon == null)
                    {
                        Destroy(gameObject);
                    }
                    if (weapon.IsUnLocked())
                    {
                        Destroy(gameObject);
                    }

                    var obj = Instantiate(weapon.WeaponObj, parent.transform);
                    obj.transform.localPosition = Vector3.zero;
                    obj.transform.localRotation = Quaternion.identity;
                    obj.transform.localScale = Vector3.one * 5;
                    obj.GetComponent<GunDataComponent>().enabled = false;
                }
                break;
            case ShopKind.Car:
                {
                    Cost = References.Instance.CarDatas.Find(itemName).Cost;
                    if (PlayerPrefs.GetInt(SAVE_OBJECT_KEY + itemName, 0) == 1)
                    {
                        var car = item.GetComponent<PlayerCar>();
                        car.IsLock = false;
                        car.rb.isKinematic = false;
                        Destroy(gameObject);
                    }
                    else
                    {
                        var car = item.GetComponent<PlayerCar>();
                        car.IsLock = true;
                        car.rb.isKinematic = true;
                    }
                }
                break;
            case ShopKind.Object:
                {
                    Cost = References.Instance.ObjectPriceDatas.Find(itemName);
                    if (PlayerPrefs.GetInt(SAVE_OBJECT_KEY + itemName, 0) == 1)
                    {
                        var car = item.GetComponent<UnlockObject>();
                        car.SetLock(false);
                        Destroy(gameObject);
                    }
                    else
                    {
                        var car = item.GetComponent<UnlockObject>();
                        car.SetLock(true);
                    }
                }
                break;
        }

        text.text = Cost.ToString();

        OnBuy += OnBuyItem;

        buyButton.onClick.AddListener(() =>
        {
            if (CashController.Instance.Cash < Cost) return;

            CashController.Instance.Cash -= Cost;
            OnBuy.Invoke(shopKind, itemName);

            switch (shopKind)
            {
                case ShopKind.Weapon:
                    {
                        var weapon = weaponDatas.FindWeapon(itemName);
                        weapon.UnLock();
                        PlayerController.Instance.ChangeWeapon(weapon.WeaponName);
                    }
                    break;
                case ShopKind.Car:
                    {
                        PlayerPrefs.SetInt(SAVE_CAR_KEY + itemName, 1);
                        var car = item.GetComponent<PlayerCar>();
                        car.IsLock = false;
                        car.rb.isKinematic = false;
                    }
                    break;
                case ShopKind.Object:
                    {
                        PlayerPrefs.SetInt(SAVE_OBJECT_KEY + itemName, 1);
                        var car = item.GetComponent<UnlockObject>();
                        car.SetLock(false);
                    }
                    break;
            }

            Destroy(gameObject);
        });
    }

    private void OnDestroy()
    {
        OnBuy -= OnBuyItem;
    }

    private void OnBuyItem(ShopKind kind, string itemName)
    {
        if (kind != shopKind) return;
        if (name != itemName) return;
        Destroy(gameObject);
    }

}

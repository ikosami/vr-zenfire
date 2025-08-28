using System;
using System.Text;
using UnityEditor;
using UnityEngine;

public class SaveController : MonoBehaviour
{

    [MenuItem("NS/お金増加　+1000")]
    static void AddCash()
    {
        CashController.Instance.Cash += 1000;
    }
    [MenuItem("NS/お金増加　*10")]
    static void AddCashMulti10()
    {
        CashController.Instance.Cash *= 10;
    }

    [MenuItem("NS/AddressableAsset ClearCache")]
    static void CleanPlayerContent()
    {
        Caching.ClearCache();
    }
}

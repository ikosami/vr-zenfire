using System;
using TMPro;
using UnityEngine;

public class CashController : MonoBehaviour
{
    public static CashController Instance;
    public Action<int> OnCashChanged;

    private void Awake()
    {
        Instance = this;
    }

    public int Cash
    {
        get => PlayerPrefs.GetInt("Cash", 0);
        set
        {
            PlayerPrefs.SetInt("Cash", value);
            OnCashChanged?.Invoke(value);
        }
    }
}

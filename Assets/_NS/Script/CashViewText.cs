using System;
using TMPro;
using UnityEngine;

public class CashViewText : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI text;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        CashController.Instance.OnCashChanged += UpdateText;
        UpdateText(CashController.Instance.Cash);
    }
    private void OnDestroy()
    {
        CashController.Instance.OnCashChanged -= UpdateText;
    }

    private void UpdateText(int obj)
    {
        text.text = obj.ToString();
    }

}

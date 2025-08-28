using System;
using UnityEngine;
using UnityEngine.UI;

public class DebugButton : MonoBehaviour
{
    [SerializeField] Button[] buttons;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        buttons[0].onClick.AddListener(zeroMoney);
        buttons[1].onClick.AddListener(AddCash100);
        buttons[2].onClick.AddListener(MultiCash10);
    }

    private void zeroMoney()
    {
        CashController.Instance.Cash = 0;
    }
    private void AddCash100()
    {
        CashController.Instance.Cash += 100;
    }
    private void MultiCash10()
    {
        CashController.Instance.Cash *= 10;
    }
}

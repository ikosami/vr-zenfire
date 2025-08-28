using System;
using System.Collections;
using System.Diagnostics;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Buyer : PlayerNearActiveBase
{
    [SerializeField] GameObject buyCanvas;
    [SerializeField] TextMeshProUGUI sellPriceText;
    [SerializeField] Button sellButton;
    [SerializeField] float multi = 1;
    [SerializeField] NPC npc;

    [SerializeField] Renderer[] eyes;
    [SerializeField] Material eyeMaterialWhite;
    [SerializeField] Material eyeMaterialRed;




    void Start()
    {
        buyCanvas.SetActive(false);
        OnChange = (isActive) =>
        {
            if (isActive)
            {
                var price = (int)(PlayerController.Instance.itemInventory.GetSellPrice() * multi);
                buyCanvas.SetActive(price != 0);
                sellPriceText.text = price.ToString();
            }
            else
            {
                buyCanvas.SetActive(false);
            }
        };
        sellButton.onClick.AddListener(OnSellButton);
    }

    private void OnSellButton()
    {
        SoundManager.Instance.Play("coin");
        CashController.Instance.Cash += (int)(PlayerController.Instance.itemInventory.GetSellPrice() * multi * ParamData.Instance.moneyMulti);
        PlayerController.Instance.itemInventory.AllDelete();
        buyCanvas.SetActive(false);

        npc.animator.Play("Catch");


        //NS.Util.CoroutineRunner.Instance.WaitRun(() =>
        //{
        //    foreach (var item in eyes)
        //    {
        //        item.material = eyeMaterialRed;
        //    }
        //    normalBackTime = 20;
        //}, 2);

    }

    float normalBackTime = 0;

    protected override void Update()
    {
        base.Update();

        if (normalBackTime > 0)
        {
            normalBackTime -= Time.deltaTime;
            if (normalBackTime <= 0)
            {
                foreach (var item in eyes)
                {
                    item.material = eyeMaterialWhite;
                }
            }
        }
    }
}
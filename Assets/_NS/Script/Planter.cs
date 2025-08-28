using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Planter : MonoBehaviour
{
    [SerializeField] GameObject parent;
    [SerializeField] TextMeshProUGUI lvText;
    [SerializeField] TextMeshProUGUI lvupCostText;

    [SerializeField] TextMeshProUGUI seedPriceText;
    [SerializeField] TextMeshProUGUI plantPriceText;

    [SerializeField] TextMeshProUGUI timeText;
    [SerializeField] TextMeshProUGUI plantTimeText;
    [SerializeField] TextMeshProUGUI plantetPriceText;
    [SerializeField] Image plantImage;
    [SerializeField] GameObject purchaseUI;
    [SerializeField] Image progressBar;

    [SerializeField] GameObject StandObj;
    [SerializeField] GameObject emptyViewObj;
    [SerializeField] Button planterBuyButton;
    int planterPrice => References.Instance.PlanterDatas.Find(planterID);

    PlantDatas plantDatas => References.Instance.PlantDatas;

    [SerializeField] Button seedPurchaseButton;
    [SerializeField] Button leftButton;
    [SerializeField] Button rightButton;
    [SerializeField] Button levelUpButton;
    [SerializeField] Button harvestButton;

    [SerializeField] LevelBasedSwitcher levelBasedSwitcher;

    [SerializeField] GameObject lightOnObj;
    [SerializeField] GameObject lightOffObj;

    [SerializeField] int planterID = 0;

    int currentIndex = 0;
    // 現在の表示インデックス保持用
    int currentViewIndex = -1;

    PlantData currentPlant;
    GameObject plantedObject;
    bool isGrowing = false;
    bool isWaitHarvest = false;
    float growthTimer = 0f;

    int level = 0;
    int levelUpCostBase = 100;
    int levelUpCost = 100;
    float effectivePlantTime = 10;


    public int PlayerSaveLvel
    {
        get => PlayerPrefs.GetInt($"Planter_{planterID}", 0);
        set
        {
            PlayerPrefs.SetInt($"Planter_{planterID}", value);
            PlayerPrefs.Save();
        }
    }

    void Start()
    {
        seedPurchaseButton.onClick.AddListener(OnBuyButton);
        leftButton.onClick.AddListener(OnLeftButton);
        rightButton.onClick.AddListener(OnRightButton);
        levelUpButton.onClick.AddListener(OnLevelUpButton);
        harvestButton.onClick.AddListener(Harvest);
        planterBuyButton.onClick.AddListener(PlanterBuy);

        level = PlayerSaveLvel;

        UpdateUI();
    }


    void Update()
    {
        if (isGrowing && !isWaitHarvest)
        {
            growthTimer += Time.deltaTime * ParamData.Instance.growthSpeedMulti;
            float progress = Mathf.Clamp01(growthTimer / effectivePlantTime);
            parent.transform.localScale = Vector3.one * progress;
            progressBar.fillAmount = progress;


            // 追加: 成長段階に応じてオブジェクト切り替え
            if (currentPlant != null && currentPlant.PlantObjs != null)
            {
                int newIndex = 0;
                for (int i = 0; i < currentPlant.PlantObjs.Length; i++)
                {
                    if (progress >= currentPlant.PlantObjs[i].par)
                        newIndex = i;
                    else
                        break;
                }
                if (newIndex != currentViewIndex)
                {
                    if (plantedObject != null)
                        Destroy(plantedObject);
                    plantedObject = Instantiate(
                        currentPlant.PlantObjs[newIndex].PlantObj,
                        parent.transform
                    );
                    currentViewIndex = newIndex;
                }
            }

            if (progress >= 1f)
            {
                isWaitHarvest = true;
                UpdateUI();
            }
        }
    }

    public void OnLeftButton()
    {
        currentIndex = (currentIndex - 1 + plantDatas.plantDatas.Length) % plantDatas.plantDatas.Length;
        UpdateUI();
    }

    public void OnRightButton()
    {
        currentIndex = (currentIndex + 1) % plantDatas.plantDatas.Length;
        UpdateUI();
    }
    void UpdateUI()
    {
        levelBasedSwitcher.SetLevel(level);
        if (level == 0)
        {
            emptyViewObj.SetActive(true);
            purchaseUI.gameObject.SetActive(false);
            progressBar.transform.parent.gameObject.SetActive(false);
            harvestButton.gameObject.SetActive(false);
            plantetPriceText.text = planterPrice.ToString();
            StandObj.SetActive(false);
            return;
        }
        emptyViewObj.SetActive(false);
        StandObj.SetActive(true);

        bool isLightOn = isGrowing && !isWaitHarvest;
        lightOnObj.SetActive(isLightOn);
        lightOffObj.SetActive(!isLightOn);

        purchaseUI.gameObject.SetActive(!isGrowing);
        progressBar.transform.parent.gameObject.SetActive(isGrowing && !isWaitHarvest);

        harvestButton.gameObject.SetActive(isWaitHarvest);

        lvText.text = $"Lv.{level}";

        levelUpCost = Mathf.CeilToInt(levelUpCostBase * Mathf.Pow(ParamData.Instance.levelUpCostLv, level - 1));
        lvupCostText.text = $"¥{levelUpCost}";

        currentPlant = plantDatas.plantDatas[currentIndex];
        seedPriceText.text = currentPlant.SeedPrice.ToString();
        plantPriceText.text = currentPlant.PlantPrice.ToString();

        effectivePlantTime = currentPlant.PlantTime * Mathf.Pow(ParamData.Instance.growthSpeedLv, level - 1);
        timeText.text = $"{effectivePlantTime:F1}s";
        plantImage.sprite = currentPlant.Sprite;

    }
    public void OnBuyButton()
    {
        if (CashController.Instance.Cash < currentPlant.SeedPrice) return;

        CashController.Instance.Cash -= currentPlant.SeedPrice;

        plantedObject = Instantiate(
            currentPlant.PlantObjs[0].PlantObj,
            parent.transform
        );
        parent.transform.localScale = Vector3.zero;
        progressBar.fillAmount = 0f;
        progressBar.transform.parent.gameObject.SetActive(true);
        growthTimer = 0f;
        isGrowing = true;
        purchaseUI.SetActive(false);

        UpdateUI();
    }

    public void Harvest()
    {
        if (!isWaitHarvest) return;

        SoundManager.Instance.Play("harvest");

        PlayerController.Instance.itemInventory.AddItem(currentPlant.PlantName, 1);


        PlayerController.Instance.weapon.HoldItem(currentPlant.HaveItem);

        if (plantedObject != null)
        {
            Destroy(plantedObject);
        }

        isWaitHarvest = false;
        isGrowing = false;
        UpdateUI();
    }
    public void OnLevelUpButton()
    {
        if (CashController.Instance.Cash < levelUpCost) return;

        CashController.Instance.Cash -= levelUpCost;
        level++;

        PlayerSaveLvel = level;
        UpdateUI();
    }
    private void PlanterBuy()
    {
        if (CashController.Instance.Cash < planterPrice) return;

        CashController.Instance.Cash -= planterPrice;
        level++;
        PlayerSaveLvel = level;
        UpdateUI();
    }
}

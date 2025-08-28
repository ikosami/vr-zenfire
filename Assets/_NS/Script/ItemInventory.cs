using System.Collections.Generic;
using UnityEngine;

public class ItemInventory
{

    public Dictionary<string, int> items = new Dictionary<string, int>();

    public void AddItem(string itemName, int amount)
    {
        if (items.ContainsKey(itemName))
        {
            items[itemName] += amount;
        }
        else
        {
            items[itemName] = amount;
        }
        SaveItems();
        PlantHaveViewManager.Instance.ViewUpdate();
    }

    public void AllDelete()
    {
        items.Clear();
        SaveItems();
        PlantHaveViewManager.Instance.ViewUpdate();
    }

    public int GetSellPrice()
    {
        int totalPrice = 0;

        foreach (var item in items)
        {
            totalPrice += References.Instance.PlantDatas.Find(item.Key).PlantPrice * item.Value;
        }
        return totalPrice;
    }

    public Dictionary<string, int> GetItems()
    {
        return items;
    }
    public void SaveItems()
    {
        string json = JsonUtility.ToJson(new Serialization<string, int>(items));
        PlayerPrefs.SetString("ItemInventory", json);
        PlayerPrefs.Save();
    }

    public void LoadItems()
    {
        if (PlayerPrefs.HasKey("ItemInventory"))
        {
            string json = PlayerPrefs.GetString("ItemInventory");
            items = JsonUtility.FromJson<Serialization<string, int>>(json).ToDictionary();
        }
    }
}

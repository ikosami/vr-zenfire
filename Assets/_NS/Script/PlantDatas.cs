using System;
using UnityEngine;

public class PlantDatas : ScriptableObject
{
    public PlantData[] plantDatas;

    public PlantData Find(string itemName)
    {
        foreach (var data in plantDatas)
        {
            if (data.PlantName == itemName)
            {
                return data;
            }
        }
        return null;
    }
}

[Serializable]
public class PlantData
{
    public string PlantName;
    public Sprite Sprite;
    public PlantView[] PlantObjs;
    public int SeedPrice;
    public int PlantPrice;
    public int PlantTime;

    public GameObject HaveItem;
}

[Serializable]
public class PlantView
{
    public GameObject PlantObj;
    public float par;
}

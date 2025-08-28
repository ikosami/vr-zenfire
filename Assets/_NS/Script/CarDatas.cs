using System;
using System.Collections.Generic;
using UnityEngine;

public class CarDatas : ScriptableObject
{
    public List<CarData> Data;

    public CarData Find(string itemName)
    {
        if (int.TryParse(itemName, out int itemIndex))
        {
            var data = Data.Find(x => x.ID == itemIndex);

            if (data != null)
            {
                return data;
            }
            return null;
        }
        return null;
    }

}
[Serializable]
public class CarData
{
    public int ID;
    public int Cost;
    public Sprite Sprite;
    public GameObject CarPrefab;
}

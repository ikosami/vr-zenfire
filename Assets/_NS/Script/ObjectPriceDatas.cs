using System;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPriceDatas : ScriptableObject
{
    public List<ObjectPriceData> priceData;

    public int Find(string itemName)
    {
        if (int.TryParse(itemName, out int itemIndex))
        {
            var data = priceData.Find(x => x.ID == itemIndex);

            if (data != null)
            {
                return data.Price;
            }
            return 999999;
        }
        return 999999;
    }

}
[Serializable]
public class ObjectPriceData
{
    public int ID;
    public int Price;
}

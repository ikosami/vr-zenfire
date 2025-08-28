using System;
using UnityEngine;

public class PlanterDatas : ScriptableObject
{
    public int[] planterPrices;

    public int Find(int id)
    {
        return planterPrices[id - 1];
    }
}

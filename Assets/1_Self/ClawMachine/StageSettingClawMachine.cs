using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSettingClawMachine : ScriptableObject
{
    public List<StageDataClawMachine> stageParamDatas;
}

[Serializable]
public class StageDataClawMachine
{
    public List<ClawMachineItem> itemList;
    public List<int> popCnt;
    public GameObject prefab;
    public int completeCount = 10;
}
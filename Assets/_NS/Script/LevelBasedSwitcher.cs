using UnityEngine;
using System.Collections.Generic;

[System.Serializable]
public class LevelObject
{
    // このレベル以上なら有効化の閾値
    public int levelThreshold;
    // 対応する子オブジェクト
    public GameObject obj;
}

public class LevelBasedSwitcher : MonoBehaviour
{
    [SerializeField]
    private List<LevelObject> levelObjects = new List<LevelObject>();

    private void Awake()
    {
        // 閾値昇順にソート
        levelObjects.Sort((a, b) => a.levelThreshold.CompareTo(b.levelThreshold));
    }

    public void SetLevel(int level)
    {
        LevelObject target = null;
        // 最大の閾値を満たすものを取得
        foreach (var lo in levelObjects)
        {
            if (level >= lo.levelThreshold)
                target = lo;
            else
                break;
        }
        // それ以外は全て非アクティブ
        foreach (var lo in levelObjects)
            lo.obj.SetActive(lo == target);
    }
}

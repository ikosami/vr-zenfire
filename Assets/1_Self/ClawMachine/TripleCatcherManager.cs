using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

// UFOキャッチャーのマネージャークラス
public class TripleCatcherManager : MonoBehaviour
{
    [SerializeField] GameObject popRange; // 景品を生成する範囲
    [SerializeField] StageSettingClawMachine stageSetting;
    [SerializeField] VRButton _nextStageButton;
    [SerializeField] VRButton _resetButton;
    [SerializeField] VRButton _fouceNextButton;
    //[SerializeField] VRButton _deleteNextButton;
    //[SerializeField] TextMeshProUGUI _fourceNextStageText;
    [SerializeField] TextMeshProUGUI _nextStageText;

    //[SerializeField] MeshHighlighter _nextButtonHighlighter;

    [SerializeField] Transform _centerT;
    [SerializeField] GameObject lockObj;
    int currentStage = 0; // 現在のステージ番号

    int getNum = 0;
    int targetNum = 10;
    GameObject stageObj = null;

    [SerializeField] GameObject itemDestoryEffect;

    List<ClawMachineItem> itemList = new List<ClawMachineItem>();

    private void Start()
    {
        currentStage = PlayerPrefs.GetInt("currentStage", 0);
        _nextStageButton.OnPressed += () =>
        {
            SoundManager.Instance.Play("button");
            if (getNum < targetNum) return;
            currentStage++;
            StageInit();
        };

        _resetButton.OnPressed += () =>
        {
            SoundManager.Instance.Play("button");
            StageInit();
        };
        _fouceNextButton.OnPressed += () =>
        {
            SoundManager.Instance.Play("button");
            currentStage++;
            StageInit();
        };

        //_deleteNextButton.OnPressed += () =>
        //{
        //    SoundManager.Instance.Play("button");
        //    DeleteObj();
        //};

        StageInit();
    }

    private void UpdateNumButtonText()
    {
        //if (getNum < targetNum)
        //{
        //    _nextStageText.text = $"{getNum}/{targetNum}";
        //}
        //else
        //{
        //    _nextStageText.text = $"NEXT\nGAME";
        //}
        _nextStageText.text = $"NEXT\nGAME";
    }

    /// <summary>
    /// 開始時に景品を生成
    /// </summary>
    private void StageInit()
    {
        if (currentStage >= stageSetting.stageParamDatas.Count)
        {
            currentStage = 0;
        }
        PlayerPrefs.SetInt("currentStage", currentStage);
        StageDataClawMachine stageData = stageSetting.stageParamDatas[currentStage];
        //_fourceNextStageText.text = $"STAGE {currentStage + 1}";

        DeleteObj();


        NS.Util.CoroutineRunner.Instance.WaitRun(() =>
        {
            if (stageData.prefab != null)
                stageObj = Instantiate(stageData.prefab, _centerT);
            // popRangeの範囲内でランダムに景品を配置
            Bounds rangeBounds = popRange.GetComponent<Collider>().bounds;

            for (int i = 0; i < stageData.itemList.Count; i++)
            {
                ClawMachineItem item = stageData.itemList[i];
                for (int num = 0; num < stageData.popCnt[i]; num++)
                {
                    itemList.Add(SpawnItem(item, rangeBounds));
                }
            }

            NS.Util.CoroutineRunner.Instance.WaitRun(() =>
            {
                lockObj.SetActive(false);
            }, 2.0f);
        }, 1.0f);

        targetNum = stageData.completeCount;
        //_nextButtonHighlighter.SetHighlighting(false);

        if (popRange == null)
        {
            Debug.LogWarning("popRange が設定されていません。");
            return;
        }

        lockObj.SetActive(true);


        getNum = 0;
        UpdateNumButtonText();
    }

    private void DeleteObj()
    {
        if (stageObj != null)
            Destroy(stageObj.gameObject);
        foreach (var item in itemList)
        {
            item.DestroyItem(itemDestoryEffect);
        }
        itemList.Clear();
    }

    /// <summary>
    /// 指定された範囲内にアイテムを生成する
    /// </summary>
    private ClawMachineItem SpawnItem(ClawMachineItem item, Bounds rangeBounds)
    {
        Vector3 randomPosition = new Vector3(
            Random.Range(rangeBounds.min.x, rangeBounds.max.x),
            Random.Range(rangeBounds.min.y, rangeBounds.max.y),
            Random.Range(rangeBounds.min.z, rangeBounds.max.z)
        );

        Quaternion randomRotation = Quaternion.Euler(
            Random.Range(0f, 360f), // X軸の回転
            Random.Range(0f, 360f), // Y軸の回転
            Random.Range(0f, 360f)  // Z軸の回転
        );



        ClawMachineItem itemObj = Instantiate(item, randomPosition, randomRotation);
        itemObj.transform.localScale *= ParamData.Instance.ItemScaleClawMachine;
        return itemObj;
    }

    public void GetItem(ClawMachineItem itemComponent)
    {
        getNum++;
        if (getNum >= targetNum)
        {
            //_nextButtonHighlighter.SetHighlighting(true);
        }
        UpdateNumButtonText();
    }
}

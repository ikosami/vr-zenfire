using Cysharp.Threading.Tasks.Triggers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlantHaveViewManager : MonoBehaviour
{
    public static PlantHaveViewManager Instance;
    [SerializeField] List<PlantHaveView> itemDatas = new List<PlantHaveView>();
    [SerializeField] PlantHaveView haveViewPrefab;

    private void Awake()
    {
        Instance = this;
    }
    private void Start()
    {
        ViewUpdate();
    }

    // Update is called once per frame
    public void ViewUpdate()
    {
        var items = PlayerController.Instance.itemInventory.GetItems();
        for (int i = 0; i < items.Count; i++)
        {
            PlantHaveView view = null;
            var plantData = References.Instance.PlantDatas.Find(items.Keys.ToArray()[i]);
            if (itemDatas.Count <= i)
            {
                view = Instantiate(haveViewPrefab, transform);
                itemDatas.Add(view);
            }
            else
            {
                view = itemDatas[i];
            }

            view.gameObject.SetActive(true);
            view.PlantData = plantData;
            view.Image.sprite = plantData.Sprite;
            view.Text.text = items.Values.ToArray()[i].ToString();
        }
        // 余ったViewを非表示にする
        for (int i = items.Count; i < itemDatas.Count; i++)
        {
            itemDatas[i].gameObject.SetActive(false);
        }

    }
}

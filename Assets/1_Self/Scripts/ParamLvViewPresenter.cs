using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class ParamLvViewPresenter : MonoBehaviour
{
    [System.Serializable]
    public class KeyAndView {
        public ParamType ParamType;
        public ParamLvView View;
    }

    [SerializeField] KeyAndView[] _keyAndViews;
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.Instance.OnLvChanged += OnLvChanged;
        GameEvents.Instance.OnCoinChanged += OnCoinChanged;
        foreach(var keyAndView in _keyAndViews) {
            keyAndView.View.SetLv(PlayerData.Instance.GetLv(keyAndView.ParamType).Level);
            keyAndView.View.SetNeedExp(PlayerData.Instance.GetLv(keyAndView.ParamType).NeedExp);
            keyAndView.View.SetLvUpActive(PlayerData.Instance.Coin >= PlayerData.Instance.GetLv(keyAndView.ParamType).NeedExp);
            keyAndView.View._button.onClick.AddListener(() => {
                PlayerData.Instance.LvUp(keyAndView.ParamType);
            });
        }
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnLvChanged(PlayerData.Lv lv) {
        var view = _keyAndViews.FirstOrDefault(x => x.ParamType == lv.Key);
        if(view != null) {
            view.View.SetLv(lv.Level);
            view.View.SetNeedExp(lv.NeedExp);
            view.View.SetLvUpActive(PlayerData.Instance.Coin >= lv.NeedExp);
        }
    }

    void OnCoinChanged(float coin) {
        foreach(var keyAndView in _keyAndViews) {
            keyAndView.View.SetLvUpActive(PlayerData.Instance.Coin >= PlayerData.Instance.GetLv(keyAndView.ParamType).NeedExp);
        }
    }
}

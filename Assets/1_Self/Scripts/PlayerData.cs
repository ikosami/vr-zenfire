using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerData : MonoBehaviour
{
    public static PlayerData Instance;
    [System.Serializable]
    public class Lv {
        public ParamType Key;
        public int Level => PlayerPrefs.GetInt("ParamLv_" + Key.ToString(), 1);
        public int NeedExpBase;
        public int NeedExpAdd;
        public int NeedExp => NeedExpBase + NeedExpAdd * (Level - 1);
        public float ValueBase;
        public float ValueAdd;
        public float Value => ValueBase + ValueAdd * (Level - 1);
    }

    [SerializeField] List<Lv> Lvs;

    public float Coin {
        get {
            var coin = PlayerPrefs.GetFloat("Coin", 0);
            if(coin < 0) {
                coin = 0;
                PlayerPrefs.SetFloat("Coin", 0);
            }
            return coin;
        }
        set {
            PlayerPrefs.SetFloat("Coin", value);
        }
    }
    // Start is called before the first frame update
    void Awake()
    {
        if(Instance == null) {
            Instance = this;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Lv GetLv(ParamType key) {
        return Lvs.Find(lv => lv.Key == key);
    }

    public void ClearCoin() {
        Coin = 0;
        GameEvents.Instance.OnCoinChanged?.Invoke(Coin);
    }

    public void AddCoin(float coin) {
        Coin += coin;
        GameEvents.Instance.OnCoinChanged?.Invoke(Coin);
    }

    public void ResetLv(ParamType key) {
        PlayerPrefs.SetInt("ParamLv_" + key.ToString(), 1);
        GameEvents.Instance.OnLvChanged?.Invoke(GetLv(key));
    }

    public void ResetAllLv() {
        foreach(var lv in Lvs) {
            ResetLv(lv.Key);
            GameEvents.Instance.OnLvChanged?.Invoke(lv);
        }
    }

    public bool LvUp(ParamType key) {
        var lv = GetLv(key);
        if(Coin < lv.NeedExp) {
            return false;
        }
        Coin -= lv.NeedExp;
        PlayerPrefs.SetInt("ParamLv_" + key.ToString(), lv.Level + 1);
        GameEvents.Instance.OnLvChanged?.Invoke(lv);
        GameEvents.Instance.OnCoinChanged?.Invoke(Coin);
        return true;
    }
}

public enum ParamType {
    Power,
    CoinRate,
    WeaponSize,
}
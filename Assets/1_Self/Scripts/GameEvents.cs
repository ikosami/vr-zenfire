using System;
using UnityEngine;

public class GameEvents : MonoBehaviour {
    public Action<float> OnCoinChanged;
    public Action<PlayerData.Lv> OnLvChanged;

    public static GameEvents Instance;

    void Awake() {
        if(Instance == null) {
            Instance = this;
        }
    }
}
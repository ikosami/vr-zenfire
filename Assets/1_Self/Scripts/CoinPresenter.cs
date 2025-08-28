using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CoinPresenter : MonoBehaviour
{
    [SerializeField] TMP_Text _coinText;
    // Start is called before the first frame update
    void Start()
    {
        GameEvents.Instance.OnCoinChanged += OnCoinChanged;
        _coinText.text = PlayerData.Instance.Coin.ToString();
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnCoinChanged(float coin) {
        _coinText.text = coin.ToString();
    }
}

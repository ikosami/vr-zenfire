using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class RewardCoinManager : MonoBehaviour
{
    [SerializeField] TMP_Text _rewardText;

    int _reward = 0;

    public int Reward => _reward;

    float _Rate => PlayerData.Instance.GetLv(ParamType.CoinRate).Value;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void InitReward() 
    {
        _reward = 0;
        _rewardText.text = "0";
    }

    public void CalcReward(float measure)
    {
        _reward = (int)(measure * _Rate);
        if(_reward <= 0) {
            _reward = 0;
        }
        _rewardText.text = _reward.ToString();
    }


}

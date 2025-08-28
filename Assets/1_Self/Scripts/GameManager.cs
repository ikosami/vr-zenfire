using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] GameObject _cautionPanel;
    [SerializeField] GameObject _startPanel;
    [SerializeField] SandbagManager _sandbagManager;
    [SerializeField] WallManager _wallManager;
    [SerializeField] ScoreManager _scoreManager;
    [SerializeField] RewardCoinManager _rewardCoinManager;
    [SerializeField] FollowTarget _followTarget;
    [SerializeField] CountDownTimer _countDownTimer;
    [SerializeField] GameObject _countDownPanel;
    [SerializeField] GameObject _countEndPanel;
    [SerializeField] PowerComboProgress _powerComboProgress;
    [SerializeField] CountDownIntTimer _countDownIntTimer;
    [SerializeField] GameObject _countDownIntPanel;
    [SerializeField] GameObject _monitor;

    // Start is called before the first frame update
    void Start()
    {
        _cautionPanel.SetActive(true);
        _startPanel.SetActive(false);
        _countDownPanel.SetActive(false);
        _countEndPanel.SetActive(false);
        _countDownIntPanel.SetActive(false);
        _scoreManager.OnFixed += (score) =>
        {
            EndGame(score);
        };

        _countDownTimer.OnCountDownEnd += () => {
            _countDownPanel.SetActive(false);
            _sandbagManager.Current.VelocityRate = Mathf.Clamp01(_powerComboProgress.Progress) * 0.5f + 0.5f;
            _sandbagManager.Current.CanRagdoll = RagdollOnCollision.RagdollType.Cannot;
            _countDownIntPanel.SetActive(true);
            _countDownIntTimer.Init();
            _countDownIntTimer.StartCountDown();
            References.Instance.GetSceneParticle("effect_light").Stop();
        };

        _countDownIntTimer.OnCountDownEnd += () => {
            _sandbagManager.Current.CanRagdoll = RagdollOnCollision.RagdollType.Ragdoll;
            _countDownIntPanel.SetActive(false);
            _countEndPanel.SetActive(true);
        };
    }

    IEnumerator DelayCoroutine(float delay, System.Action action) {
        yield return new WaitForSeconds(delay);
        action?.Invoke();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void ShowStartPanel()
    {
        _cautionPanel.SetActive(false);
        _startPanel.SetActive(true);
    }

    public void PlayGame()
    {
        _followTarget.StopSpin();
        _followTarget.transform.rotation = Quaternion.identity;
        _sandbagManager.Spawn((target) => {
            _scoreManager.StartMeasuring(target.transform);
            _followTarget.SetTarget(target.transform);
            _countEndPanel.SetActive(false);
            _monitor.SetActive(true);
            // StartCoroutine(HitStopCoroutine());
        });
        _sandbagManager.Current.CanRagdoll = RagdollOnCollision.RagdollType.Combo;
        _wallManager.SpawnWalls();
        _scoreManager.InitScore();
        _rewardCoinManager.InitReward();
        _countDownTimer.Init();
        System.Action<Collision> func = null;
        func = (collision) => {
            _countDownTimer.StartCountDown();
            References.Instance.GetSceneParticle("effect_light").Play();
            _sandbagManager.Current.OnHit -= func;
        };
        _sandbagManager.Current.OnHit += func;
        _startPanel.SetActive(false);
        _countDownPanel.SetActive(true);
        _countEndPanel.SetActive(false);
        _countDownIntPanel.SetActive(false);
        _monitor.SetActive(false);
        _powerComboProgress.Init(_sandbagManager.Current);

        SoundManager.Instance.Play("start");
    }

    public void EndGame(float score)
    {
        _startPanel.SetActive(true);
        _rewardCoinManager.CalcReward(score);
        PlayerData.Instance.AddCoin(_rewardCoinManager.Reward);
        var particle = References.Instance.GetSceneParticle("confetti");
        particle.Play();
        _followTarget.StartSpin();
        // _windEffectParticle.Stop();
    }

    private IEnumerator HitStopCoroutine()
    {
        yield return new WaitForSecondsRealtime(0.05f);
        Time.timeScale = 0.1f;
        yield return new WaitForSecondsRealtime(1f);
        Time.timeScale = 1f;
    }
}

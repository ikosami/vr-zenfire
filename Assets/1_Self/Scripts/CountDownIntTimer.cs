using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountDownIntTimer : MonoBehaviour
{
    [SerializeField] TMP_Text _text;
    [SerializeField] int _countDownTime = 3;
    public System.Action OnCountDownEnd;

    float _time = 0f;

    bool _isCounting = false;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(!_isCounting) return;
        var prevTime = _time;
        _time -= Time.deltaTime;
        if(prevTime > 2 && _time <= 2) {
            SoundManager.Instance.Play("two");
        }
        if(prevTime > 1 && _time <= 1) {
            SoundManager.Instance.Play("one");
        }
        SetText(_time);
        if(_time <= 0) {
            _time = 0;
            _text.text = "GO!";
            _isCounting = false;
            OnCountDownEnd?.Invoke();
        }
    }

    public void Init() {
        _isCounting = false;
        _time = _countDownTime;
        SetText(_time);
    }

    public void StartCountDown() {
        _isCounting = true;
        _time = _countDownTime;
        SetText(_time);
        SoundManager.Instance.Play("three");
    }

    void SetText(float time) {
        _text.text = Mathf.CeilToInt(_time).ToString();
    }
}

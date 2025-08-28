using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CountDownTimer : MonoBehaviour
{
    [SerializeField] TMP_Text _text;
    [SerializeField] float _countDownTime = 5f;
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
        _time -= Time.deltaTime;
        _text.text = _time.ToString("F2");
        if(_time <= 0) {
            _time = 0;
            _text.text = _countDownTime.ToString("F2");
            _isCounting = false;
            OnCountDownEnd?.Invoke();
        }
    }

    public void Init() {
        _isCounting = false;
        _time = _countDownTime;
        _text.text = _time.ToString("F2");
    }

    public void StartCountDown() {
        _isCounting = true;
        _time = _countDownTime;
        _text.text = _time.ToString("F2");
    }
}

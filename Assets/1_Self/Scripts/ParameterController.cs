using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ParameterController : MonoBehaviour
{
    public static ParameterController Instance { get; private set; }

    [SerializeField] Slider _slider;
    [SerializeField] TMP_Text _text;
    [SerializeField] Slider _slider2;
    [SerializeField] TMP_Text _text2;
    [SerializeField] Button _coinResetBtn;
    [SerializeField] Button _coinAdd10000Btn;    
    [SerializeField] Button _lvResetBtn;

    float _rate = 50f;

    float _rate2 = -9.8f * 2f;

    public float PunchFactor => _slider.value * _rate;
    public float Gravity => _slider2.value * _rate2;
    // Start is called before the first frame update
    void Start()
    {
        if(Instance != null) {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _slider.value = 1f /_rate;
        _slider.onValueChanged.AddListener(OnValueChanged);
        _slider2.value = -4.9f /_rate2;
        _slider2.onValueChanged.AddListener(OnValueChanged2);
        SetText();
        SetText2();

        _coinResetBtn.onClick.AddListener(() => {
            PlayerData.Instance.ClearCoin();
        });
        _coinAdd10000Btn.onClick.AddListener(() => {
            PlayerData.Instance.AddCoin(10000);
        });
        _lvResetBtn.onClick.AddListener(() => {
            PlayerData.Instance.ResetAllLv();
        });
    }

    void OnValueChanged(float value) {
        SetText();
    }

    void OnValueChanged2(float value) {
        SetText2();
        // 重力設定を変更
        Physics.gravity = new Vector3(0, Gravity, 0);
    }

    void SetText() {
        _text.text = PunchFactor.ToString("F2");
    }

    void SetText2() {
        _text2.text = Gravity.ToString("F2");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

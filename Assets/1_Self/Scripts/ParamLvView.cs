using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ParamLvView : MonoBehaviour
{
    public  Button _button;
    [SerializeField] TMP_Text _lvText;
    [SerializeField] TMP_Text _needExpText;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void SetLv(int lv) {
        _lvText.text = lv.ToString();
    }

    public void SetNeedExp(int needExp) {
        _needExpText.text = needExp.ToString();
    }

    public void SetLvUpActive(bool active) {
        _button.interactable = active;
    }
}

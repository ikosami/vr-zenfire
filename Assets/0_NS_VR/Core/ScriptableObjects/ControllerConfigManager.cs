using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerConfigManager : MonoBehaviour
{
    [SerializeField] ControllerConfig[] _models;
    // Start is called before the first frame update
    void Start()
    {
        ParamData.OnParamChange += OnParamChange;
        SwitchModel(ParamData.Instance.HandModelKind);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnParamChange(string paramName)
    {
        if(paramName == "HandModelKind")
        {
            SwitchModel(ParamData.Instance.HandModelKind);
        }
    }

    public void SwitchModel(HandModelKind handModelKind)
    {
        foreach(var model in _models)
        {
            if(model.handModelKind == handModelKind)
            {
                model.model.SetActive(true);
            }
            else
            {
                model.model.SetActive(false);
            }
        }
    }

    [System.Serializable]
    public class ControllerConfig
    {
        public HandModelKind handModelKind;
        public GameObject model;
        
    }
}

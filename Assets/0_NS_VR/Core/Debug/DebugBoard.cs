using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DebugBoard : MonoBehaviour
{
    [SerializeField] DebugValueSlider _valueSliderPrefab;
    [SerializeField] DebugValueToggle _valueTogglePrefab;
    [SerializeField] DebugValueDropdown _valueDropdownPrefab;
    [SerializeField] Transform _listParent;
    [SerializeField] Button _resetButton;
    [SerializeField] Button _saveResetButton;
    
    List<DebugValueSlider> SliderList = new List<DebugValueSlider>();
    List<DebugValueToggle> ToggleList = new List<DebugValueToggle>();
    List<DebugValueDropdown> DropdownList = new List<DebugValueDropdown>();

    void Start()
    {
        Type type = ParamData.Instance.GetType();
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            if (field.FieldType == typeof(bool))
            {
                var obj = Instantiate(_valueTogglePrefab, _listParent);
                obj.SetField(field);
                ToggleList.Add(obj);
            // enumかどうか判定
            } else if(field.FieldType.IsEnum) {
                var obj = Instantiate(_valueDropdownPrefab, _listParent);
                obj.SetField(field);
                DropdownList.Add(obj);
            } else {
                var obj = Instantiate(_valueSliderPrefab, _listParent);
                obj.SetField(field);
                SliderList.Add(obj);
            }

            
        }

        _resetButton.onClick.AddListener(OnResetButtonClicked);


        _saveResetButton.onClick.AddListener(() =>
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    public static void ChangeParam(string paramName)
    {
        ParamData.OnParamChange?.Invoke(paramName);
    }

    private void OnResetButtonClicked()
    {
        foreach (var slider in SliderList)
        {
            slider.Slider.value = 1;
        }
        foreach (var toggle in ToggleList)
        {
            toggle.Toggle.isOn = false;
        }
        foreach (var dropdown in DropdownList)
        {
            dropdown.Dropdown.value = 0;
        }
    }
}

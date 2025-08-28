using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Linq;

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
        // ParamDataからの取得
        // AddFieldsFromInstance(ParamData.Instance);

        // または、現在のシーンの特定のコンポーネントを持つオブジェクトから取得
        var debuggableObjects = FindObjectsOfType<MonoBehaviour>().Where(mb => 
            mb.GetType().GetCustomAttribute<DebuggableClassAttribute>() != null);
        
        foreach (var obj in debuggableObjects)
        {
            AddFieldsFromInstance(obj);
        }

        // 自分のクラスからの取得
        // Type type = ParamData.Instance.GetType();
        // FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        // foreach (var field in fields)
        // {
        //     // Only process fields with the DebugUIAttribute
        //     if (!Attribute.IsDefined(field, typeof(DebugUIAttribute)))
        //         continue;
                
        //     if (field.FieldType == typeof(bool))
        //     {
        //         var obj = Instantiate(_valueTogglePrefab, _listParent);
        //         obj.SetField(field);
        //         ToggleList.Add(obj);
        //     // enumかどうか判定
        //     } else if(field.FieldType.IsEnum) {
        //         var obj = Instantiate(_valueDropdownPrefab, _listParent);
        //         obj.SetField(field);
        //         DropdownList.Add(obj);
        //     } else {
        //         var obj = Instantiate(_valueSliderPrefab, _listParent);
        //         obj.SetField(field);
        //         SliderList.Add(obj);
        //     }

            
        // }

        _resetButton.onClick.AddListener(OnResetButtonClicked);


        _saveResetButton.onClick.AddListener(() =>
        {
            PlayerPrefs.DeleteAll();
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    private void AddFieldsFromInstance(MonoBehaviour instance)
    {
        if (instance == null) return;
        
        Type type = instance.GetType();
        FieldInfo[] fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);

        foreach (var field in fields)
        {
            // Only process fields with the DebugUIAttribute
            if (!Attribute.IsDefined(field, typeof(DebugUIAttribute)))
                continue;

            var debugUIAttribute = field.GetCustomAttribute<DebugUIAttribute>();

            if (field.FieldType == typeof(bool))
            {
                var obj = Instantiate(_valueTogglePrefab, _listParent);
                obj.SetField(field, instance, instance.name + "." + debugUIAttribute.Label);  // インスタンスも渡す
                ToggleList.Add(obj);
            // enumかどうか判定
            } else if(field.FieldType.IsEnum) {
                var obj = Instantiate(_valueDropdownPrefab, _listParent);
                obj.SetField(field, instance, instance.name + "." + debugUIAttribute.Label);  // インスタンスも渡す
                DropdownList.Add(obj);
            } else {
                var obj = Instantiate(_valueSliderPrefab, _listParent);
                obj.SetField(field, instance, instance.name + "." + debugUIAttribute.Label);  // インスタンスも渡す
                SliderList.Add(obj);
            }
        }
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
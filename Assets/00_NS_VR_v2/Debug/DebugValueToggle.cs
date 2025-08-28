using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugValueToggle : MonoBehaviour
{
    public Toggle Toggle;
    public TextMeshProUGUI NameText;

    FieldInfo field;
    object baseValue;
    object instance;
    private void Awake()
    {
        Toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    public void SetField(FieldInfo field, object instance, string label = "")
    {
        this.field = field;
        NameText.text = string.IsNullOrEmpty(label) ? field.Name : label;
        baseValue = field.GetValue(instance);
        this.instance = instance;
        // bool型の場合のみ処理
        if (field.FieldType == typeof(bool))
        {
            Toggle.isOn = (bool)baseValue;
        }
        else
        {
            Debug.LogWarning("Unsupported field type: " + field.FieldType);
        }
    }

    public void OnToggleValueChanged(bool toggleValue)
    {
        var targetObject = instance;

        if (field != null && targetObject != null && baseValue != null)
        {
            if (field.FieldType == typeof(bool))
            {
                field.SetValue(targetObject, toggleValue);
            }
            else
            {
                Debug.LogWarning("Unsupported field type: " + field.FieldType);
            }
            DebugBoard.ChangeParam(field.Name);
        }
        else
        {
            Debug.LogError("Field, targetObject, or baseValue is not set!");
        }
    }

    internal void SetParent(DebugBoard debugBoard)
    {
        throw new NotImplementedException();
    }
} 
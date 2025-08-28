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

    private void Awake()
    {
        Toggle.onValueChanged.AddListener(OnToggleValueChanged);
    }

    public void SetField(FieldInfo field)
    {
        this.field = field;
        NameText.text = field.Name;
        baseValue = field.GetValue(ParamData.Instance);

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
        var targetObject = ParamData.Instance;

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
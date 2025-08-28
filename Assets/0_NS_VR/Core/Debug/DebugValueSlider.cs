using System;
using System.Reflection;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DebugValueSlider : MonoBehaviour
{
    public Slider Slider;
    public TextMeshProUGUI NameText;
    public TextMeshProUGUI ValueText;
    public TextMeshProUGUI ParText;

    FieldInfo field;
    object baseValue;

    private void Awake()
    {
        Slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    public void SetField(FieldInfo field)
    {
        this.field = field;
        NameText.text = field.Name;
        baseValue = field.GetValue(ParamData.Instance);
        ValueText.text = baseValue.ToString();
    }

    public void OnSliderValueChanged(float sliderValue)
    {
        ParText.text = (sliderValue * 100) + "%";
        var targetObject = ParamData.Instance;

        // 型ごとに計算を分ける
        if (field != null && targetObject != null && baseValue != null)
        {
            Type fieldType = field.FieldType;

            if (fieldType == typeof(int))
            {
                // int型の場合
                int calculatedValue = Mathf.RoundToInt((int)baseValue * sliderValue);
                field.SetValue(targetObject, calculatedValue);
                ValueText.text = calculatedValue.ToString();
            }
            else if (fieldType == typeof(float))
            {
                // float型の場合
                float calculatedValue = (float)baseValue * sliderValue;
                field.SetValue(targetObject, calculatedValue);
                ValueText.text = calculatedValue.ToString();
            }
            else if (fieldType == typeof(double))
            {
                // double型の場合
                double calculatedValue = (double)baseValue * sliderValue;
                field.SetValue(targetObject, calculatedValue);
                ValueText.text = calculatedValue.ToString();
            }
            else
            {
                Debug.LogWarning("Unsupported field type: " + fieldType);
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

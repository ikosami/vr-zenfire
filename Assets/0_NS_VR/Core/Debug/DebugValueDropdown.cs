using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DebugValueDropdown : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI _nameText;
    // InspectorからアサインできるDropdownコンポーネント
    [SerializeField] private TMP_Dropdown _dropdown;

    public TMP_Dropdown Dropdown => _dropdown;

    // 対象のフィールド情報を保持
    private FieldInfo _targetField;

    private void Awake()
    {
        // Inspectorでアサインされていなければ、同じGameObjectから取得
        if (_dropdown == null)
        {
            _dropdown = GetComponent<TMP_Dropdown>();
        }
    }

    // フィールド情報を設定し、ドロップダウンのオプションを初期化する
    public void SetField(FieldInfo field)
    {
        _targetField = field;
        _nameText.text = field.Name;
        if (_dropdown == null)
        {
            Debug.LogError("Dropdownコンポーネントが見つかりません。");
            return;
        }

        // 現在の値を取得
        object currentValue = field.GetValue(ParamData.Instance);

        // ドロップダウンのオプションをクリア
        _dropdown.ClearOptions();

        List<string> options = new List<string>();
        Array enumValues = Enum.GetValues(field.FieldType);
        int selectedIndex = 0;
        int index = 0;
        foreach (object value in enumValues)
        {
            string optionText = value.ToString();
            options.Add(optionText);
            if (value.Equals(currentValue))
            {
                selectedIndex = index;
            }
            index++;
        }

        // オプションの追加
        _dropdown.AddOptions(options);
        // 現在の値に合わせて選択
        _dropdown.value = selectedIndex;

        // 値が変化した時のイベントを登録
        _dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    // ドロップダウンの値が変更された時の処理
    private void OnDropdownValueChanged(int index)
    {
        if (_targetField == null)
        {
            Debug.LogError("対象フィールドが設定されていません。");
            return;
        }
        Array enumValues = Enum.GetValues(_targetField.FieldType);
        if (index < 0 || index >= enumValues.Length)
        {
            Debug.LogError("無効なインデックスが選択されました。");
            return;
        }
        object selectedValue = enumValues.GetValue(index);
        _targetField.SetValue(ParamData.Instance, selectedValue);
        DebugBoard.ChangeParam(_targetField.Name);
    }
} 
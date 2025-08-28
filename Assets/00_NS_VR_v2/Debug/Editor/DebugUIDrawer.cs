using UnityEngine;
using UnityEditor;

[CustomPropertyDrawer(typeof(DebugUIAttribute))]
public class DebugUIDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 元のGUIの色を保存
        Color originalColor = GUI.color;
        
        // オレンジ色に設定 (少し薄めのオレンジを使用)
        GUI.color = new Color(1f, 0.8f, 0.4f, 1f);
        
        // デフォルトのプロパティフィールドを描画
        EditorGUI.PropertyField(position, property, label, true);
        
        // 色を元に戻す
        GUI.color = originalColor;
    }
}

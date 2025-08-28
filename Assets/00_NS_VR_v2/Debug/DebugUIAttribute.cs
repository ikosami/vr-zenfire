using System;

/// <summary>
/// Attribute to mark fields that should be exposed to the debug UI.
/// </summary>
[AttributeUsage(AttributeTargets.Field)]
public class DebugUIAttribute : Attribute
{
    /// <summary>
    /// デバッグUI上で表示されるラベル
    /// 指定されていない場合は、フィールド名が使用されます
    /// </summary>
    public string Label { get; private set; }

    public DebugUIAttribute()
    {
        Label = string.Empty;
    }

    public DebugUIAttribute(string label)
    {
        Label = label;
    }
}

[AttributeUsage(AttributeTargets.Class)]
public class DebuggableClassAttribute : Attribute
{
    public string Category { get; private set; }

    // カテゴリなしでも使用可能
    public DebuggableClassAttribute()
    {
        Category = string.Empty;
    }

    // カテゴリを指定して使用可能
    public DebuggableClassAttribute(string category)
    {
        Category = category;
    }
}

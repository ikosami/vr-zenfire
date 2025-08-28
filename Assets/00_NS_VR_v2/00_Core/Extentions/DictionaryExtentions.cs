using System.Collections;
using System.Collections.Generic;

public static class DictionaryExtentions
{

    public static bool GetBool(Dictionary<string, object> dict, string key, bool defaultValue = false)
    {
        if (!dict.ContainsKey(key)) return defaultValue;
        return bool.Parse(dict[key].ToString());
    }

    public static float GetFloat(Dictionary<string, object> dict, string key, float defaultValue = 0)
    {
        if (!dict.ContainsKey(key)) return defaultValue;
        return float.Parse(dict[key].ToString());
    }

    public static int GetInt(Dictionary<string, object> dict, string key, int defaultValue = 0)
    {
        if (!dict.ContainsKey(key)) return defaultValue;
        return int.Parse(dict[key].ToString());
    }
}
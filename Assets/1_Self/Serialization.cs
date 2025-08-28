using System.Collections.Generic;

[System.Serializable]
public class Serialization<TKey, TValue>
{
    [System.Serializable]
    public struct KeyValue
    {
        public TKey Key;
        public TValue Value;
    }

    public List<KeyValue> target = new List<KeyValue>();

    public Serialization(Dictionary<TKey, TValue> dict)
    {
        foreach (var pair in dict)
        {
            target.Add(new KeyValue() { Key = pair.Key, Value = pair.Value });
        }
    }

    public Dictionary<TKey, TValue> ToDictionary()
    {
        var dict = new Dictionary<TKey, TValue>();
        foreach (var pair in target)
        {
            dict[pair.Key] = pair.Value;
        }
        return dict;
    }
}

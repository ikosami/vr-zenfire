using System.Collections.Generic;
using UnityEngine;
using System;

[CreateAssetMenu(fileName = "ReferencesScriptable", menuName = "ReferencesScriptable")]
public class ReferencesScriptable : ScriptableObject
{
    [SerializeField] List<ReferenceData> _prefabs;

    // シリアライズしない
    [NonSerialized] bool _isLoaded = false;
    [NonSerialized] Dictionary<string, GameObject> _references = new Dictionary<string, GameObject>();

    public GameObject GetPrefab(string key) {
        if(!_isLoaded) {
            Load();
        }
        if(_references.TryGetValue(key, out var prefab)) {
            return prefab;
        }
        return null;
    }

    void Load() {
        _references.Clear();
        foreach(var prefab in _prefabs) {
            _references.Add(prefab.Key, prefab.Value);
        }
        _isLoaded = true;
    }
}

[System.Serializable]
public class ReferenceData : IKeyValue<string, GameObject> {
}
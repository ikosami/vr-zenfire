using System.Collections.Generic;
using UnityEngine;
using System;
using Oculus.Haptics;

[CreateAssetMenu(fileName = "HapticReferencesScriptable", menuName = "HapticReferencesScriptable")]
public class HapticReferencesScriptable : ScriptableObject
{
    [SerializeField] List<HapticReferenceData> _haptics;

    // シリアライズしない
    [NonSerialized] bool _isLoaded = false;
    [NonSerialized] Dictionary<string, HapticClip> _references = new Dictionary<string, HapticClip>();

    public HapticClip GetClip(string key) {
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
        foreach(var haptic in _haptics) {
            _references.Add(haptic.Key, haptic.Value);
        }
        _isLoaded = true;
    }
}

[System.Serializable]
public class HapticReferenceData : IKeyValue<string, HapticClip> {
}
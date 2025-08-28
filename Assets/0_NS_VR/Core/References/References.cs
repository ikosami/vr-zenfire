using System.Collections.Generic;
using UnityEngine;
using PaintIn3D;
using NS_VR.StoreIntentSystem;

public class References : MonoBehaviour
{
    public Transform Player;
    public static References Instance;

    [SerializeField] ReferencesScriptable _references;
    [SerializeField] List<ObjectReference> _sceneObjects;
    [SerializeField] DecalReferences _sceneDecalReferences;
    [SerializeField] List<ParticleReference> _sceneParticles;
    [SerializeField] OculusStoreAppData _oculusStoreAppData;

    void Awake() {
        Instance = this;
    }

    public GameObject GetPrefab(string key) {
        return _references.GetPrefab(key);
    }

    public CwPaintDecal GetSceneDecal(string key) {
        return _sceneDecalReferences.Decals.Find(x => x.Key == key).Value;
    }

    public GameObject GetSceneObject(string key) {
        return _sceneObjects.Find(x => x.Key == key).Value;
    }

    public ParticleSystem GetSceneParticle(string key) {
        return _sceneParticles.Find(x => x.Key == key).Value;
    }

    public OculusStoreAppData OculusStoreAppData => _oculusStoreAppData;

    // デバッグ用のワールド座標マーカー
    public void MarkPosition(Vector3 pos) {
        var marker = GetPrefab("debug_marker");
        var instance = Instantiate(marker, pos, Quaternion.identity);
        marker.SetActive(true);
    }
}

[System.Serializable]
public class ObjectReference : IKeyValue<string, GameObject> {
}

[System.Serializable]
public class ParticleReference : IKeyValue<string, ParticleSystem> {
}


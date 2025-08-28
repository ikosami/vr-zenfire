using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using PaintIn3D;

public class DecalOnHit : MonoBehaviour
{
    [SerializeField] RagdollOnCollision _ragdollOnCollision;
    [SerializeField] string _decalName = "decal_aza";
    [SerializeField] Transform[] _decalPoints;

    string[] _hitAnimKeys = new string[] { "punch type1", "punch type2", "punch type3" };

    List<Transform> _decalPointsShuffled = new List<Transform>();

    int _decalIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        _decalPointsShuffled.AddRange(_decalPoints);
        _decalPointsShuffled.Shuffle();

        _ragdollOnCollision.OnHit += (hit) => {
            if(PowerComboProgress.Instance.Progress < (_decalIndex + 1) * 0.2f) {
                return;
            }
            if(_decalIndex >= _decalPointsShuffled.Count) {
                return;
            }
            var decal = References.Instance.GetSceneDecal(_decalName);
            var euler = _decalPointsShuffled[_decalIndex].rotation.eulerAngles;
            euler.z = Random.Range(0, 360);
            decal.HandleHitPoint(false, 0, 100, 0, _decalPointsShuffled[_decalIndex].position, Quaternion.Euler(euler));
            _decalIndex++;
        };
    }

    // Update is called once per frame
    void Update()
    {
    }
}

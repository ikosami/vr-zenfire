using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class AnimationOnHit : MonoBehaviour
{
    [SerializeField] Animator _animator;
    [SerializeField] RagdollOnCollision _ragdollOnCollision;

    int[] _hitAnimKeys = new int[] { 13, 14, 15 };
    // Start is called before the first frame update
    void Start()
    {
        _ragdollOnCollision.OnHit += (hit) => {
            var hitIndex = _hitAnimKeys[Random.Range(0, _hitAnimKeys.Length)];
            for (int i = 0; i < _animator.layerCount; i++) {
                _animator.SetLayerWeight(i, i == hitIndex ? 1 : 0);
            }
        };
    }

    // Update is called once per frame
    void Update()
    {
    }
}

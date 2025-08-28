using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class RagdollState : MonoBehaviour
{

    [SerializeField] private bool _isRagdoll = false;
    [SerializeField] Animator _animator;

    public bool IsRagdoll => _isRagdoll;

    public void SetRagdoll(bool isRagdoll)
    {
        if(_isRagdoll == isRagdoll) return;
        _isRagdoll = isRagdoll;
        _animator.SetLayerWeight(6, _isRagdoll ? 1f : 0f);
        _animator.enabled = !_isRagdoll;
    }
    // Start is called before the first frame update
    void Start()
    {
        SetRagdoll(_isRagdoll);
    }

    // Update is called once per frame
    void Update()
    {
    }
}

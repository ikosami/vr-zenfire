using UnityEngine;

/// <summary>
/// Manages ragdoll state transitions for characters
/// Handles animator and physics-based movement switching
/// </summary>
[RequireComponent(typeof(Animator))]
public class RagdollState : MonoBehaviour
{
    [Header("Ragdoll Settings")]
    [SerializeField] private bool _isRagdoll = false;
    [SerializeField] private Animator _animator;
    [SerializeField] private int _ragdollLayer = 6;
    [SerializeField] private float _recoveryTime = 2f;
    [SerializeField] private float _blendDuration = 0.5f;

    private bool _isRecovering = false;
    private float _recoveryTimer = 0f;
    private float _blendWeight = 0f;

    public bool IsRagdoll => _isRagdoll;

    private void Start()
    {
        if (_animator == null)
        {
            _animator = GetComponent<Animator>();
        }
        SetRagdoll(_isRagdoll);
    }

    private void Update()
    {
        if (_isRecovering)
        {
            UpdateRecovery();
        }
    }

    /// <summary>
    /// Toggle ragdoll state
    /// </summary>
    public void SetRagdoll(bool isRagdoll)
    {
        if (_isRagdoll == isRagdoll) return;
        
        _isRagdoll = isRagdoll;
        _animator.enabled = !_isRagdoll;

        // Handle rigidbodies and colliders
        var rigidbodies = GetComponentsInChildren<Rigidbody>();
        var colliders = GetComponentsInChildren<Collider>();

        foreach (var rb in rigidbodies)
        {
            rb.isKinematic = !_isRagdoll;
            rb.useGravity = _isRagdoll;
        }

        foreach (var collider in colliders)
        {
            collider.enabled = _isRagdoll;
        }

        // Set animator layer weight
        _animator.SetLayerWeight(_ragdollLayer, _isRagdoll ? 1f : 0f);

        if (!_isRagdoll)
        {
            StartRecovery();
        }
    }

    /// <summary>
    /// Start recovery animation when transitioning from ragdoll to animated
    /// </summary>
    private void StartRecovery()
    {
        _isRecovering = true;
        _recoveryTimer = _recoveryTime;
        _blendWeight = 1f;
    }

    /// <summary>
    /// Update recovery animation blend
    /// </summary>
    private void UpdateRecovery()
    {
        if (_recoveryTimer > 0)
        {
            _recoveryTimer -= Time.deltaTime;
            _blendWeight = Mathf.Lerp(0f, 1f, _recoveryTimer / _recoveryTime);
            _animator.SetLayerWeight(_ragdollLayer, _blendWeight);
        }
        else
        {
            _isRecovering = false;
            _animator.SetLayerWeight(_ragdollLayer, 0f);
        }
    }

    /// <summary>
    /// Apply an impulse force to the ragdoll
    /// </summary>
    public void ApplyForce(Vector3 force, ForceMode mode = ForceMode.Impulse)
    {
        if (!_isRagdoll) return;

        var rigidbodies = GetComponentsInChildren<Rigidbody>();
        foreach (var rb in rigidbodies)
        {
            rb.AddForce(force, mode);
        }
    }
}

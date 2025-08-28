using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RagdollOnCollision : MonoBehaviour
{
    [SerializeField] LayerMask _collisionLayer;
    [SerializeField] string[] _collisionTags;
    [SerializeField] ColliderEventListener[] _colliderEventListeners;
    [SerializeField] RagdollState _ragdollState;
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] TrailRenderer[] _trailRenderer;
    [SerializeField] AudioSource _audioSource;
    // [SerializeField] AudioSource _audioSource;

    float _hitMagnitude = 0f;


    public bool AllTag => _collisionTags.Length == 0;

    public RagdollType CanRagdoll;

    public enum RagdollType {
        Cannot,
        Combo,
        Ragdoll,
    }

    public System.Action<Transform> OnRagdoll;

    public System.Action<Collision> OnHit;

    public float VelocityRate;

    string[] _voiceKeys = new string[] { "voice_knocked_1", "voice_knocked_2", "voice_knocked_3" };

    string[] _hitEffKeys = new string[] { "effect_hit_punch_1", "effect_hit_punch_2", "effect_hit_punch_3" };
    // Start is called before the first frame update
    void Start()
    {
        foreach (var listener in _colliderEventListeners)
        {
            listener._OnCollisionEnter += (collision) => _OnCollisionEnter(collision, listener);
        }

        foreach (var trail in _trailRenderer)
        {
            trail.emitting = false;
        }
    }

    void _OnCollisionEnter(Collision collision, ColliderEventListener target)
    {
        if(CanRagdoll == RagdollType.Cannot) return;
        if (_collisionLayer == (_collisionLayer | (1 << collision.gameObject.layer)))
        {
            bool hit = false;
            if (AllTag)
            {
                
                hit = true;
            }
            else
            {
                foreach (var tag in _collisionTags)
                {
                    if (collision.gameObject.CompareTag(tag))
                    {
                        hit = true;
                        break;
                    }
                }
            }
            if (hit)
            {
                if(CanRagdoll == RagdollType.Combo) {
                    if(target.Rigidbody != null) {
                        target.Rigidbody.AddForce(collision.impulse * 5f, ForceMode.Impulse);
                    }

                    var prefab = References.Instance.GetSceneParticle(_hitEffKeys[Random.Range(0, _hitEffKeys.Length)]);
                    prefab.transform.position = collision.contacts[0].point;
                    prefab.Play();
                    VibrationController.Instance.Play("click", ControllerSide.Both);
                    SoundManager.Instance.Play("hit");
                    OnHit?.Invoke(collision);
                    return;
                }   
                if(_ragdollState.IsRagdoll) {
                    // if(target.Rigidbody != null) {
                    //     target.Rigidbody.AddForce(collision.relativeVelocity * 100f, ForceMode.Impulse);
                    // }
                }
                else if (collision.relativeVelocity.magnitude >= _hitMagnitude)
                {
                    foreach(var listener in _colliderEventListeners) {
                        if(listener.Rigidbody != null) {
                            listener.Rigidbody.isKinematic = false;
                        }
                    };
                    _ragdollState.SetRagdoll(true);
                    _rigidbody.velocity = new Vector3(0,2.8f, VelocityRate * 7f + Mathf.Clamp(collision.impulse.z, 0, 10) * 10f * PlayerData.Instance.GetLv(ParamType.Power).Value * ParameterController.Instance.PunchFactor);
                    // _rigidbody.AddForceAtPosition(collision.relativeVelocity * 10f, collision.contacts[0].point, ForceMode.Impulse);
                    // if(target.Rigidbody != null) {
                    //     target.Rigidbody.mass *= 10;
                    //     target.Rigidbody.AddForceAtPosition(collision.relativeVelocity * 100f * PlayerData.Instance.GetLv(ParamType.Power).Value * ParameterController.Instance.PunchFactor, collision.contacts[0].point, ForceMode.Impulse);
                    // }
                    _rigidbody.angularVelocity = new Vector3(0, 0, 10);
                    var prefab = References.Instance.GetPrefab("effect_hit_punch");
                    Instantiate(prefab, collision.contacts[0].point, Quaternion.identity);
                    SoundManager.Instance.Play("smash");
                    _audioSource.PlayOneShot(SoundManager.Instance.GetSound(_voiceKeys[Random.Range(0, _voiceKeys.Length)]));
                    VibrationController.Instance.Play("hit_human", ControllerSide.Both);
                    foreach (var trail in _trailRenderer)
                    {
                        trail.emitting = true;
                    }
                    OnRagdoll?.Invoke(_rigidbody.transform);
                }
            }
        }
    }
}

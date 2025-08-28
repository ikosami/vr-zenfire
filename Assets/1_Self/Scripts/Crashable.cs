using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PaintIn3D;

[RequireComponent(typeof(Collider))]
public class Crashable : MonoBehaviour
{
    [SerializeField] ColliderEventListener _colliderEventListener;
    [SerializeField] Rigidbody[] _rigidbodies;
    [SerializeField] Collider[] _colliders;
    [SerializeField] Collider _self;
    [SerializeField] GameObject _prevObject;
    [SerializeField] LayerMask _excludeLayerMask;
    [SerializeField] AudioSource _audioSource;

    public float CrashBorderVelocity = 5f; 
    // Start is called before the first frame update
    void Start()
    {
        _colliderEventListener._OnTriggerEnter += _OnTriggerEnter;
        _audioSource.clip = SoundManager.Instance.GetSound("wall_break");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void _OnTriggerEnter(Collider other)
    {
        if(_excludeLayerMask == (_excludeLayerMask | (1 << other.gameObject.layer))) {
            return;
        }

        bool isSelf = false;

        foreach(var rb in _rigidbodies) {
            if(rb.gameObject == other.gameObject) {
                isSelf = true;
                break;
            }
        }
        
        if(isSelf) {
            return;
        }

        
        // _prevObject.SetActive(false);

        var blocked = other.GetComponent<Rigidbody>() == null || other.GetComponent<Rigidbody>().linearVelocity.z < CrashBorderVelocity;
        
        var point = _self.ClosestPointOnBounds(other.transform.position);

        if(blocked) {
            _self.isTrigger = false;
            References.Instance.GetSceneDecal("decal_demolish").HandleHitPoint(false, 0, 100, 0, point, Quaternion.identity);
        }else {
            _self.enabled = false;
            _prevObject.SetActive(false);
            foreach (var rb in _rigidbodies)
            {
                rb.isKinematic = blocked;
            }
            foreach (var col in _colliders)
            {
                col.enabled = true;
                col.gameObject.SetActive(true);
            }
            // SoundManager.Instance.Play("wall_break");
            _audioSource.PlayOneShot(_audioSource.clip);
        }

        var prefab = References.Instance.GetPrefab("effect_hit_wall");
        Instantiate(prefab, point, Quaternion.identity);
        VibrationController.Instance.Play("hit_wall", ControllerSide.Both);
        // References.Instance.MarkPosition(point); // デバッグ用
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTarget : MonoBehaviour
{
    [SerializeField] Transform _target;

    [SerializeField] bool _X = true;
    [SerializeField] bool _Y = true;
    [SerializeField] bool _Z = true;

    [SerializeField] bool _Rot = false;

    [SerializeField] bool _lateUpdate = true;

    bool _spin = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void LateUpdate() {
        if(_lateUpdate) {
            _Update();
        }
    }

    void Update() {
        if(!_lateUpdate) {
            _Update();
        }
    }

    // Update is called once per frame
    void _Update()
    {
        if(_target == null) return;
        var pos = _target.position;
        if(!_X) pos.x = transform.position.x;
        if(!_Y) pos.y = transform.position.y;
        if(!_Z) pos.z = transform.position.z;
        transform.position = pos;

        if(_Rot) {
            transform.rotation = _target.rotation;
        }

        if(_spin) {
            transform.Rotate(0, 60 * Time.deltaTime, 0);
        }
    }

    public void SetTarget(Transform target) {
        _target = target;
    }

    public void StartSpin() {
        _spin = true;
    }

    public void StopSpin() {
        _spin = false;
    }
}

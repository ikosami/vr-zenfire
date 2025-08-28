using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LookAtObjectFromAreaEventHandler : MonoBehaviour
{
    [SerializeField] ColliderEventListener _colliderEventListener;
    [SerializeField] Transform _targetTransform;
    [SerializeField] float _lookAtAngle = 30f;
    public UnityEvent OnEnterArea;
    public UnityEvent OnExitArea;
    public UnityEvent OnStartLookAt;
    public UnityEvent OnEndLookAt;

    bool _isLookAt = false;

    public bool IsLookAt => IsEnterArea && _isLookAt;
    public bool IsEnterArea { get; private set; } = false;
    // Start is called before the first frame update
    void Start()
    {
        _colliderEventListener._OnTriggerEnter += _OnEnterArea;
        _colliderEventListener._OnTriggerExit += _OnExitArea;
    }

    // Update is called once per frame
    void Update()
    {
        if(IsEnterArea) {
            Vector3 cameraForward = Camera.main.transform.forward;
            Vector3 directionToTarget = (_targetTransform.position - Camera.main.transform.position).normalized;

            float angle = Vector3.Angle(cameraForward, directionToTarget);
            if(angle < _lookAtAngle) {
                LookAt();
            } else {
                DontLookAt();
            }
        }
    }

    void _OnEnterArea(Collider collider)
    {
        IsEnterArea = true;
        OnEnterArea?.Invoke();
    }

    void _OnExitArea(Collider collider)
    {
        IsEnterArea = false;
        DontLookAt();
        OnExitArea?.Invoke();
    }
    
    void LookAt()
    {
        var prev = _isLookAt;
        _isLookAt = true;
        if(prev != _isLookAt) OnStartLookAt?.Invoke();
    }

    void DontLookAt()
    {
        var prev = _isLookAt;
        _isLookAt = false;
        if(prev != _isLookAt) OnEndLookAt?.Invoke();
    }
    
    
}

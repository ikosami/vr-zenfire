using UnityEngine;
public class HandPhysicsFollowInit : MonoBehaviour {

    [SerializeField] private HandColliderBase _handCollider;
    [SerializeField] private FollowTargetPhysics _followTargetPhysics;
    void Start() {
        if (_handCollider != null && _followTargetPhysics != null) {
            if(_handCollider.IsInited) {
                OnInit();
            } else {
                _handCollider.OnInited += OnInit;
            }
        }
    }

    void OnInit() {
        _followTargetPhysics.InitializeColliderEventListeners(_handCollider.ColliderEventListeners.ToArray());
        _handCollider.OnInited -= OnInit;
    }
}
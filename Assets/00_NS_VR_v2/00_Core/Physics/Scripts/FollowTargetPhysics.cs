using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowTargetPhysics : MonoBehaviour
{
    [SerializeField] Transform _target;
    [SerializeField] Rigidbody _rigidbody;
    [SerializeField] ColliderEventListener[] _colliderEventListeners;
    [SerializeField] Transform _collisionPointTransform;
    [SerializeField] GameObject _floorHitEffect;
    [SerializeField] ControllerSide controllerSide;

    float _springForce = 1000f;    // ばね力
    float _damping = 10f;          // 減衰力
    float _rotationForce = 100f;   // 回転の力

    bool _isCollision = false;

    Vector3 _collisionRelativePosition;

    public Rigidbody HandRigidbody => _rigidbody;
    
    Rigidbody _playerRigidbody;
    
    // 衝突イベント用のデリゲート
    public delegate void CollisionEventHandler(FollowTargetPhysics sender, Vector3 collisionPoint, Vector3 relativeVelocity);
    public event CollisionEventHandler OnCollisionEnterEvent;
    public event CollisionEventHandler OnCollisionExitEvent;
    public event CollisionEventHandler OnCollisionForceEvent;

    // public bool IsCollision => _isCollision;

    //     /// <summary>
    // /// 衝突点の相対位置を取得
    // /// </summary>
    // public Vector3 CollisionRelativePosition => _collisionRelativePosition;

    // /// <summary>
    // /// 衝突点のトランスフォームを取得
    // /// </summary>
    // public Transform CollisionPointTransform => _collisionPointTransform;



    // Start is called before the first frame update
    void Start()
    {
        foreach(var colliderEventListener in _colliderEventListeners) {
            colliderEventListener._OnTriggerEnter += _OnTriggerEnter;
            colliderEventListener._OnTriggerExit += _OnTriggerExit;
        }
    }

    public void InitializeColliderEventListeners(ColliderEventListener[] colliderEventListeners) {
        foreach(var colliderEventListener in _colliderEventListeners) {
            colliderEventListener._OnTriggerEnter -= _OnTriggerEnter;
            colliderEventListener._OnTriggerExit -= _OnTriggerExit;
        }

        _colliderEventListeners = colliderEventListeners;
        foreach(var colliderEventListener in _colliderEventListeners) {
            colliderEventListener._OnTriggerEnter += _OnTriggerEnter;
            colliderEventListener._OnTriggerExit += _OnTriggerExit;
        }
    }

    void LateUpdate() {
        if(_isCollision) {
            // var relativePosition = _collisionPointTransform.position - _playerRigidbody.transform.position;
            //  Vector3 moveDirection = relativePosition - _collisionRelativePosition;
            //  moveDirection *= 1.01f;
            //  _playerRigidbody.AddForce(-moveDirection, ForceMode.VelocityChange);
            // _playerRigidbody.transform.position -= moveDirection;
        }
    }

    void _OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Hand")) return;
        _isCollision = true;

        // 衝突イベントを発火
        OnCollisionEnterEvent?.Invoke(this, _collisionPointTransform.position, Vector3.zero);
    }

    void _OnTriggerStay(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Hand")) return;
        _isCollision = true;
        Debug.Log("OnTriggerEnter: " + other.gameObject.name);
    }
    
    void _OnTriggerExit(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Hand")) return;
        _isCollision = false;

        // 衝突終了イベントを発火
        OnCollisionExitEvent?.Invoke(this, _collisionPointTransform.position, Vector3.zero);
    }
    

    // Update is called once per frame
    void FixedUpdate()
    {
        if(_target == null) return;

        _rigidbody.angularVelocity = Vector3.zero;
        
        Vector3 toTarget = _target.position - _rigidbody.position;
        _rigidbody.AddForce(toTarget / Time.fixedDeltaTime - _rigidbody.velocity, ForceMode.VelocityChange);
        
        // 回転の制御を改善
        Quaternion currentRotation = _rigidbody.rotation;
        Quaternion targetRotation = _target.rotation;
        
        // 現在の回転から目標の回転への最短経路を計算
        float angle;
        Vector3 axis;
        Quaternion deltaRotation = targetRotation * Quaternion.Inverse(currentRotation);
        deltaRotation.ToAngleAxis(out angle, out axis);
        
        // 180度以上の場合、最短経路に修正
        if (angle > 180f)
        {
            angle -= 360f;
        }
        
        // 角速度を計算して適用
        Vector3 angularVelocity = (angle * Mathf.Deg2Rad * axis) / Time.fixedDeltaTime;
        _rigidbody.AddTorque(angularVelocity * 1.1f, ForceMode.VelocityChange);

    }

    public void SetTarget(Transform target) {
        _target = target;
    }

    void OnCollisionEnter(Collision other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("UI")) return;
        Vector3 collisionPoint = other.contacts[0].point;            
        if (other.gameObject.layer != LayerMask.NameToLayer("Hand")){
            Vector3 relativeVelocity = other.relativeVelocity;
            _collisionPointTransform.position = collisionPoint;
            
            // 衝突力イベントを発火
            OnCollisionForceEvent?.Invoke(this, _collisionPointTransform.position, relativeVelocity);
        }
        

        // エフェクトの生成（衝突位置で生成）
        if (_floorHitEffect != null)
        {
            Instantiate(_floorHitEffect, other.contacts[0].point, Quaternion.identity);
        }
    }


    /// <summary>
    /// 衝突応答の力を計算
    /// </summary>
    public Vector3 CalculateCollisionResponseForce(Transform playerTransform, Vector3 collisionRelativePosition)
    {
        if (!_isCollision) return Vector3.zero;
        
        var relativePosition = _collisionPointTransform.position - playerTransform.position;
        Vector3 moveDirection = relativePosition - collisionRelativePosition;
        // moveDirection;
        return -moveDirection;
    }

}
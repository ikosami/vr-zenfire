using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ターゲットの変形を物理ベースで追跡し、衝突検出を行うクラス
/// VRでの手が障害物をすり抜けないようにする
/// </summary>
public class FollowTargetPhysics : MonoBehaviour
{
    [Header("ターゲット設定")]
    [SerializeField] Transform _target;
    [SerializeField] Rigidbody _rigidbody;

    [Header("衝突設定")]
    [SerializeField] ColliderEventListener[] _colliderEventListeners;
    [SerializeField] Transform _collisionPointTransform;

    private bool _isCollision = false;
    private Vector3 _collisionRelativePosition;
    private float _delay = 0f;

    // 衝突イベント用のデリゲート
    public delegate void CollisionEventHandler(FollowTargetPhysics sender, Vector3 collisionPoint, Vector3 relativeVelocity);
    public event CollisionEventHandler OnCollisionEnterEvent;
    public event CollisionEventHandler OnCollisionExitEvent;
    public event CollisionEventHandler OnCollisionForceEvent;

    /// <summary>
    /// オブジェクトが現在衝突中かどうかを取得
    /// </summary>
    public bool IsCollision => _isCollision;

    /// <summary>
    /// 衝突点の相対位置を取得
    /// </summary>
    public Vector3 CollisionRelativePosition => _collisionRelativePosition;

    /// <summary>
    /// 衝突点のトランスフォームを取得
    /// </summary>
    public Transform CollisionPointTransform => _collisionPointTransform;

    /// <summary>
    /// 衝突イベントリスナーを初期化
    /// </summary>
    void Start()
    {
        foreach(var colliderEventListener in _colliderEventListeners) {
            colliderEventListener._OnTriggerEnter += _OnTriggerEnter;
            colliderEventListener._OnTriggerExit += _OnTriggerExit;
        }
    }

    /// <summary>
    /// 物理更新を処理
    /// </summary>
    void FixedUpdate()
    {
        if(_target == null) return;
        
        _delay -= Time.fixedDeltaTime;
        _rigidbody.angularVelocity = Vector3.zero;
        
        // ターゲット位置に追従するための力を適用
        Vector3 toTarget = _target.position - _rigidbody.position;
        _rigidbody.AddForce(toTarget / Time.fixedDeltaTime - _rigidbody.velocity, ForceMode.VelocityChange);
        
        // 回転制御の改善
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
        _rigidbody.AddTorque(angularVelocity * 0.99f, ForceMode.VelocityChange);
    }

    /// <summary>
    /// 追従するターゲットを設定
    /// </summary>
    public void SetTarget(Transform target) 
    {
        _target = target;
    }

    /// <summary>
    /// トリガー入力イベントを処理
    /// </summary>
    void _OnTriggerEnter(Collider other) 
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Hand")) return;
        _isCollision = true;
        Debug.Log("OnTriggerEnter: " + other.gameObject.name);
        
        // 衝突イベントを発火
        OnCollisionEnterEvent?.Invoke(this, _collisionPointTransform.position, Vector3.zero);
    }

    /// <summary>
    /// トリガー滞在イベントを処理
    /// </summary>
    void _OnTriggerStay(Collider other) 
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Hand")) return;
        _isCollision = true;
        Debug.Log("OnTriggerStay: " + other.gameObject.name);
    }
    
    /// <summary>
    /// トリガー終了イベントを処理
    /// </summary>
    void _OnTriggerExit(Collider other) 
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Hand")) return;
        _isCollision = false;
        Debug.Log("OnTriggerExit: " + other.gameObject.name);
        
        // 衝突終了イベントを発火
        OnCollisionExitEvent?.Invoke(this, _collisionPointTransform.position, Vector3.zero);
    }

    /// <summary>
    /// 衝突開始イベントを処理
    /// </summary>
    void OnCollisionEnter(Collision other) 
    {
        if(other.gameObject.layer == LayerMask.NameToLayer("Hand")) return;
        
        Debug.Log("OnCollisionEnter: " + other.gameObject.name);
        Vector3 relativeVelocity = other.relativeVelocity;
        _collisionPointTransform.position = other.contacts[0].point;
        _collisionRelativePosition = _collisionPointTransform.position - transform.position;
        
        // 衝突力イベントを発火
        OnCollisionForceEvent?.Invoke(this, _collisionPointTransform.position, relativeVelocity);
        
        _delay = 0.1f;
    }

    /// <summary>
    /// 衝突滞在イベントを処理
    /// </summary>
    void OnCollisionStay(Collision other) 
    {
        if(!_isCollision) return;
    }

    /// <summary>
    /// 衝突終了イベントを処理
    /// </summary>
    void OnCollisionExit(Collision other) 
    {
        // 衝突終了処理はトリガー終了で行われる
    }
    
    /// <summary>
    /// 衝突応答の力を計算
    /// </summary>
    public Vector3 CalculateCollisionResponseForce(Transform playerTransform)
    {
        if (!_isCollision) return Vector3.zero;
        
        var relativePosition = _collisionPointTransform.position - playerTransform.position;
        Vector3 moveDirection = relativePosition - _collisionRelativePosition;
        moveDirection *= 1.01f;
        return -moveDirection;
    }
}

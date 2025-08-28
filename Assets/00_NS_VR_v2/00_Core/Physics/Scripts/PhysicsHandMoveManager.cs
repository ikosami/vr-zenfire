using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 両手のFollowTargetPhysicsコンポーネントを管理するクラス
/// プレイヤーのRigidbodyの制御を一元管理する
/// </summary>
public class PhysicsHandMoveManager : MonoBehaviour
{
    [Header("左手設定")]
    [SerializeField] private Transform _leftHandTarget;
    [SerializeField] private FollowTargetPhysics _leftHandPhysics;

    [Header("右手設定")]
    [SerializeField] private Transform _rightHandTarget;
    [SerializeField] private FollowTargetPhysics _rightHandPhysics;

    [Header("プレイヤー設定")]
    [SerializeField] private Rigidbody _playerRigidbody;
    [SerializeField] private float _collisionForceMultiplier = 1.01f;

    // 衝突状態の追跡
    private bool _isLeftHandColliding = false;
    private bool _isRightHandColliding = false;
    private Vector3 _leftHandCollisionVelocity = Vector3.zero;
    private Vector3 _rightHandCollisionVelocity = Vector3.zero;

    Vector3 _collisionRelativePosition = Vector3.zero;

    /// <summary>
    /// 初期化処理
    /// </summary>
    void Start()
    {
        InitializeHandPhysics();
    }

    /// <summary>
    /// 両手の物理コンポーネントを初期化
    /// </summary>
    private void InitializeHandPhysics()
    {
        if (_leftHandPhysics != null && _leftHandTarget != null)
        {
            _leftHandPhysics.SetTarget(_leftHandTarget);
            SubscribeToHandEvents(_leftHandPhysics, true);
        }
        else
        {
            Debug.LogWarning("左手のFollowTargetPhysicsまたはターゲットが設定されていません");
        }

        if (_rightHandPhysics != null && _rightHandTarget != null)
        {
            _rightHandPhysics.SetTarget(_rightHandTarget);
            SubscribeToHandEvents(_rightHandPhysics, false);
        }
        else
        {
            Debug.LogWarning("右手のFollowTargetPhysicsまたはターゲットが設定されていません");
        }
    }

    /// <summary>
    /// 手の衝突イベントを購読
    /// </summary>
    private void SubscribeToHandEvents(FollowTargetPhysics handPhysics, bool isLeftHand)
    {
        if (handPhysics == null) return;

        handPhysics.OnCollisionEnterEvent += (sender, collisionPoint, relativeVelocity) => 
        {
            HandleCollisionEnter(isLeftHand, collisionPoint, relativeVelocity);
        };

        handPhysics.OnCollisionExitEvent += (sender, collisionPoint, relativeVelocity) => 
        {
            HandleCollisionExit(isLeftHand, collisionPoint, relativeVelocity);
        };

        handPhysics.OnCollisionForceEvent += (sender, collisionPoint, relativeVelocity) => 
        {
            HandleCollisionForce(isLeftHand, collisionPoint, relativeVelocity);
        };
    }

    /// <summary>
    /// 衝突開始イベントを処理
    /// </summary>
    private void HandleCollisionEnter(bool isLeftHand, Vector3 collisionPoint, Vector3 relativeVelocity)
    {
        if (isLeftHand)
        {
            _isLeftHandColliding = true;
            _leftHandCollisionVelocity = relativeVelocity;
        }
        else
        {
            _isRightHandColliding = true;
            _rightHandCollisionVelocity = relativeVelocity;
        }

        // プレイヤーのRigidbodyを更新
        UpdatePlayerRigidbodyState();
    }

    /// <summary>
    /// 衝突終了イベントを処理
    /// </summary>
    private void HandleCollisionExit(bool isLeftHand, Vector3 collisionPoint, Vector3 relativeVelocity)
    {
        if (isLeftHand)
        {
            _isLeftHandColliding = false;
        }
        else
        {
            _isRightHandColliding = false;
        }

        // プレイヤーのRigidbodyを更新
        UpdatePlayerRigidbodyState();
    }

    /// <summary>
    /// 衝突力イベントを処理
    /// </summary>
    private void HandleCollisionForce(bool isLeftHand, Vector3 collisionPoint, Vector3 relativeVelocity)
    {
        if (_playerRigidbody == null) return;

        _collisionRelativePosition = collisionPoint - _playerRigidbody.position;
        // 衝突力を適用
        if (_playerRigidbody.velocity.magnitude < 0.1f)
        {
            Vector3 force = relativeVelocity;
            force.x *= 1.4f;
            force.y *= 1f;
            force.z *= 1.4f;
            _playerRigidbody.AddForce(force, ForceMode.VelocityChange);
        }

        _playerRigidbody.isKinematic = false;
    }

    /// <summary>
    /// プレイヤーのRigidbody状態を更新
    /// </summary>
    private void UpdatePlayerRigidbodyState()
    {
        if (_playerRigidbody == null) return;

        bool isEitherHandColliding = _isLeftHandColliding || _isRightHandColliding;

        // 衝突状態に基づいて重力と速度を制御
        _playerRigidbody.useGravity = !isEitherHandColliding;
        
        if (isEitherHandColliding)
        {
            _playerRigidbody.velocity = Vector3.zero;
        }
    }

    /// <summary>
    /// 衝突応答の力を適用
    /// </summary>
    void LateUpdate()
    {
        if (_playerRigidbody == null) return;

        Vector3 totalForce = Vector3.zero;

        // 左手の衝突応答力を計算
        if (_isLeftHandColliding && _leftHandPhysics != null)
        {
            totalForce += _leftHandPhysics.CalculateCollisionResponseForce(_playerRigidbody.transform, _collisionRelativePosition);
        }

        // 右手の衝突応答力を計算
        if (_isRightHandColliding && _rightHandPhysics != null)
        {
            totalForce += _rightHandPhysics.CalculateCollisionResponseForce(_playerRigidbody.transform, _collisionRelativePosition);
        }

        // 合計力を適用
        if (totalForce != Vector3.zero)
        {
            _playerRigidbody.AddForce(totalForce, ForceMode.VelocityChange);
        }
    }

    /// <summary>
    /// 左手のFollowTargetPhysicsを取得
    /// </summary>
    public FollowTargetPhysics GetLeftHandPhysics()
    {
        return _leftHandPhysics;
    }

    /// <summary>
    /// 右手のFollowTargetPhysicsを取得
    /// </summary>
    public FollowTargetPhysics GetRightHandPhysics()
    {
        return _rightHandPhysics;
    }

    /// <summary>
    /// 左手のターゲットを設定
    /// </summary>
    public void SetLeftHandTarget(Transform target)
    {
        _leftHandTarget = target;
        if (_leftHandPhysics != null)
        {
            _leftHandPhysics.SetTarget(target);
        }
    }

    /// <summary>
    /// 右手のターゲットを設定
    /// </summary>
    public void SetRightHandTarget(Transform target)
    {
        _rightHandTarget = target;
        if (_rightHandPhysics != null)
        {
            _rightHandPhysics.SetTarget(target);
        }
    }

    /// <summary>
    /// 両手の衝突状態を確認
    /// </summary>
    public bool IsEitherHandColliding()
    {
        return _isLeftHandColliding || _isRightHandColliding;
    }

    /// <summary>
    /// 両手の物理コンポーネントを有効/無効化
    /// </summary>
    public void SetHandPhysicsEnabled(bool enabled)
    {
        if (_leftHandPhysics != null)
        {
            _leftHandPhysics.enabled = enabled;
        }

        if (_rightHandPhysics != null)
        {
            _rightHandPhysics.enabled = enabled;
        }
    }
}

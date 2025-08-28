using UnityEngine;

public class HandJointBehaviour : MonoBehaviour
{
    [SerializeField] private Transform _controller;
    [SerializeField] LayerMask _collidingLayer;
    private bool _isTouch;
    private Rigidbody _rigidbody;
    private FixedJoint _joint;

    

    private void Start()
    {
        _rigidbody = GetComponent<Rigidbody>();
        _joint = _controller.GetComponent<FixedJoint>();
        _rigidbody.transform.SetPositionAndRotation(_controller.position, _controller.rotation);
        _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
    }

    private void Update()
    {
        // 回転は常に追従
        transform.rotation = _controller.rotation;

        // 触れていないときはコントローラーを追従する
        if (!_isTouch)
        {
            transform.position = _controller.position;
        }
    }

    // void _OnTriggerEnter(Collider other)
    // {
    //     if(other.gameObject.layer == LayerMask.NameToLayer("Ground"))
    //     {
    //         _isTouch = true;
    //         _joint.connectedBody = _rigidbody;   // FixedJointに接続
    //         _rigidbody.isKinematic = false;
    //     }
    // }

    

    private void _OnCollisionStay(Collision other)
    {
        // 衝突しているオブジェクトが_collidingLayerに含まれている場合
        if(_collidingLayer.value == (_collidingLayer.value | (1 << other.gameObject.layer)))
        {
            _isTouch = true;
            _joint.connectedBody = _rigidbody;   // FixedJointに接続
            _rigidbody.constraints = RigidbodyConstraints.None;
            // _rigidbody.isKinematic = false;
        }
    }

    
    private void _OnCollisionExit(Collision other)
    {
        // 衝突しているオブジェクトが_collidingLayerに含まれていない場合
        if(_collidingLayer.value != (_collidingLayer.value | (1 << other.gameObject.layer)))
        {
            _isTouch = false;
            _joint.connectedBody = null;    // FixedJointから切断
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            // _rigidbody.isKinematic = true;
        }
    }


    private void OnCollisionStay(Collision other)
    {
        // 衝突しているオブジェクトが_collidingLayerに含まれている場合
        if(_collidingLayer.value == (_collidingLayer.value | (1 << other.gameObject.layer)))
        {
            _isTouch = true;
            _joint.connectedBody = _rigidbody;   // FixedJointに接続
            _rigidbody.constraints = RigidbodyConstraints.None;
            // _rigidbody.isKinematic = false;
        }
    }

    
    private void OnCollisionExit(Collision other)
    {
        // 衝突しているオブジェクトが_collidingLayerに含まれていない場合
        if(_collidingLayer.value != (_collidingLayer.value | (1 << other.gameObject.layer)))
        {
            _isTouch = false;
            _joint.connectedBody = null;    // FixedJointから切断
            _rigidbody.constraints = RigidbodyConstraints.FreezeAll;
            // _rigidbody.isKinematic = true;
        }
    }
}
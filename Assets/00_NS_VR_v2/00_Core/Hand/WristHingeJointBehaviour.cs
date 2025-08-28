using UnityEngine;

public class WristHingeJointBehaviour : MonoBehaviour
{
    [SerializeField] private Rigidbody _hand;
    [SerializeField] private HingeJoint _joint;
    [SerializeField] private Transform _controller;
    private bool _isTouch;

    private void Start()
    {
        // _hand.isKinematic = true;
    }

    private void Update()
    {
        // // 回転は常に追従
        // transform.rotation = _controller.rotation;

        // // 触れていないときはコントローラーのローカル回転のY軸を追従する
        if (!_isTouch)
        {
            var localEuler = transform.localRotation.eulerAngles;
            localEuler.y = _controller.localRotation.eulerAngles.y;
            transform.localRotation = Quaternion.Euler(localEuler);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("OnTriggerEnter");
        // _joint.connectedBody = _hand; // FixedJointに接続
        // _hand.isKinematic = false;
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("OnTriggerStay");
    }

    private void OnCollisionStay(Collision other)
    {
        Debug.Log("OnCollisionStay");
        _isTouch = true;
        _joint.connectedBody = _hand; // FixedJointに接続
        _hand.constraints = RigidbodyConstraints.None;
    }
    
    private void OnCollisionExit(Collision other)
    {
        Debug.Log("OnCollisionExit");
        _isTouch = false;
        _joint.connectedBody = null;    // FixedJointから切断
        _hand.constraints = RigidbodyConstraints.FreezeAll;
    }
}
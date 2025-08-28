using UnityEngine;

[RequireComponent(typeof(Collider))]
public class ColliderEventListener : MonoBehaviour
{
    public System.Action<Collision> _OnCollisionEnter;

    public System.Action<Collision> _OnCollisionExit;

    public System.Action<Collider> _OnTriggerEnter;

    public System.Action<Collider> _OnTriggerExit;


    public Rigidbody Rigidbody;

    void Awake() {
        Rigidbody = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        _OnCollisionEnter?.Invoke(collision);
    }

    void OnCollisionExit(Collision collision)
    {
        _OnCollisionExit?.Invoke(collision);
    }

    void OnTriggerEnter(Collider other)
    {
        _OnTriggerEnter?.Invoke(other);
    }

    void OnTriggerExit(Collider other)
    {
        _OnTriggerExit?.Invoke(other);
    }

}

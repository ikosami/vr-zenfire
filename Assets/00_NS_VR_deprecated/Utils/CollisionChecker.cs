using System;
using UnityEngine;

public class CollisionChecker : MonoBehaviour
{
    // Trigger判定用のAction
    public Action<Collider> OnTriggerEnterAction { get; internal set; }
    public Action<Collider> OnTriggerExitAction { get; internal set; }
    public Action<Collider> OnTriggerStayAction { get; internal set; }

    // Collision判定用のAction
    public Action<Collision> OnCollisionEnterAction { get; internal set; }
    public Action<Collision> OnCollisionExitAction { get; internal set; }
    public Action<Collision> OnCollisionStayAction { get; internal set; }

    // Trigger判定のメソッド
    private void OnTriggerEnter(Collider other)
    {
        OnTriggerEnterAction?.Invoke(other);
    }
    private void OnTriggerExit(Collider other)
    {
        OnTriggerExitAction?.Invoke(other);
    }
    private void OnTriggerStay(Collider other)
    {
        OnTriggerStayAction?.Invoke(other);
    }

    // Collision判定のメソッド
    private void OnCollisionEnter(Collision collision)
    {
        //Debug.LogError("OnCollisionEnter");
        OnCollisionEnterAction?.Invoke(collision);
    }
    private void OnCollisionExit(Collision collision)
    {
        //Debug.LogError("OnCollisionExit");
        OnCollisionExitAction?.Invoke(collision);
    }
    private void OnCollisionStay(Collision collision)
    {
        //Debug.LogError("OnCollisionStay");
        OnCollisionStayAction?.Invoke(collision);
    }
}

using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class GrabbableItem : MonoBehaviour
{
    public Transform grabPoint;
    public Transform grabPointL;
    public string handPause = "GrabTeacup";
    public bool hasPose = true;

    public bool isInitAttach = true;

    private void Awake()
    {
        if (isInitAttach) Attach();
    }

    public void Attach()
    {
        //すでについていたらつけない
        if (GetComponentInParent<Grabbable>() != null) { return; }

        // プレハブをロード
        var prefab = Resources.Load<GameObject>("Grabbable");
        if (prefab == null)
        {
            Debug.LogError("Grabbableのprefabがありません");
            return;
        }

        // プレハブを生成
        var item = Instantiate(prefab, transform.position, Quaternion.identity);
        item.transform.parent = transform.parent;
        var grabbable = item.GetComponent<Grabbable>();
        grabbable._poseName = handPause;
        grabbable._hasPose = hasPose;
        // other の Rigidbody を取得
        Rigidbody oldRb = GetComponent<Rigidbody>();
        if (oldRb != null)
        {
            // 新しいオブジェクトに Rigidbody を追加
            Rigidbody newRb = item.GetComponent<Rigidbody>();

            // 重要なプロパティをコピー
            newRb.mass = oldRb.mass;
            newRb.drag = oldRb.drag;
            newRb.angularDrag = oldRb.angularDrag;
            newRb.useGravity = oldRb.useGravity;
            newRb.isKinematic = oldRb.isKinematic;
            newRb.interpolation = oldRb.interpolation;
            newRb.collisionDetectionMode = oldRb.collisionDetectionMode;
            newRb.constraints = oldRb.constraints;

            // 古い Rigidbody を削除
            Destroy(oldRb);
        }

        // 親子関係を設定
        transform.SetParent(null);
        transform.SetParent(item.transform);

        var xrGrabInteractable = item.GetComponent<XRGrabInteractable>();
        if (xrGrabInteractable != null)
        {
            if (grabPoint != null)
            {
                grabbable._grabbedPoint_R = grabPoint;
            }

            if (grabPointL != null)
            {
                grabbable._grabbedPoint_L = grabPointL;
            }

            if (grabPoint == null && grabPointL == null)
            {
                grabbable._grabbedPoint_R = transform;
                grabbable._grabbedPoint_L = transform;
            }
            grabbable.CreateMirror();


            xrGrabInteractable.colliders.AddRange(GetComponentsInChildren<Collider>().ToList());
            xrGrabInteractable.enabled = false;
            xrGrabInteractable.enabled = true;
        }
    }
}

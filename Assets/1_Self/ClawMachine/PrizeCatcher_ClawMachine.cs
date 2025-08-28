using System.Linq;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;

public class PrizeCatcher_ClawMachine : MonoBehaviour
{
    [SerializeField] TripleCatcherManager tripleCatcherManager;
    public void OnTriggerEnter(Collider other)
    {
        if (other.GetComponentInParent<Grabbable>() != null)
        {
            return;
        }

        var itemComponent = other.GetComponentInParent<ClawMachineItem>();
        if (itemComponent)
        {
            // プレハブをロード
            var prefab = Resources.Load<GameObject>("Grabbable");
            if (prefab == null)
            {
                Debug.LogError("Grabbable prefab not found!");
                return;
            }
            tripleCatcherManager.GetItem(itemComponent);

            // プレハブを生成
            var item = Instantiate(prefab, itemComponent.transform.position, Quaternion.identity);
            var grabbable = item.GetComponent<Grabbable>();
            grabbable._poseName = itemComponent.handPause;
            grabbable._hasPose = itemComponent.hasPose;
            // other の Rigidbody を取得
            Rigidbody oldRb = itemComponent.GetComponent<Rigidbody>();
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
            itemComponent.transform.SetParent(null);
            itemComponent.transform.SetParent(item.transform);

            var xrGrabInteractable = item.GetComponent<XRGrabInteractable>();
            if (xrGrabInteractable != null)
            {
                if (itemComponent.grabPoint != null)
                {
                    grabbable._grabbedPoint_R = itemComponent.grabPoint;
                    grabbable._grabbedPoint_L = itemComponent.grabPoint;
                    if (itemComponent.grabPointL != null)
                    {
                        grabbable._grabbedPoint_L = itemComponent.grabPointL;
                    }
                }


                xrGrabInteractable.colliders.AddRange(itemComponent.GetComponentsInChildren<Collider>().ToList());
                xrGrabInteractable.enabled = false;
                xrGrabInteractable.enabled = true;
            }
        }
    }

}

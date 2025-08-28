
using UnityEngine;

public class HandRay : MonoBehaviour
{
    [SerializeField] Transform rayOrigin;
    [SerializeField] ControllerSide side = ControllerSide.Left;

    bool isTriggerPressed1 = false;
    bool isTriggerPressed2 = false;
    void Update()
    {
        if (VRInputManager.Instance.IsTriggerPressed(side, true))
        {
            if (isTriggerPressed1) return;
            isTriggerPressed1 = true;

            // Hand, Player, HandCollider レイヤーを除外
            int mask = ~LayerMask.GetMask("Hand", "Player", "HandCollider");
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, Mathf.Infinity, mask))
            {
                var door = hit.collider.GetComponentInParent<Door>();
                if (door != null)
                {
                    door.Open();
                }

            }
        }
        else
        {
            isTriggerPressed1 = false;
        }
        if (VRInputManager.Instance.IsTriggerPressed(side, false))
        {
            if (isTriggerPressed2) return;
            isTriggerPressed2 = true;

            // Hand, Player, HandCollider レイヤーを除外
            int mask = ~LayerMask.GetMask("Hand", "Player", "HandCollider");
            RaycastHit hit;
            if (Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, Mathf.Infinity, mask))
            {
                var car = hit.collider.GetComponentInParent<PlayerCar>();
                if (car != null && !car.isPlayer && !car.IsLock)
                {
                    PlayerController.Instance.SetCar(car);
                }
            }
        }
        else
        {
            isTriggerPressed2 = false;
        }

        {
            RaycastHit hit;
            int mask = LayerMask.GetMask("NoGunSpace");

            // "NoGunSpace" レイヤーに当たるかどうかを判定
            bool isHit = Physics.Raycast(rayOrigin.position, rayOrigin.forward, out hit, Mathf.Infinity, mask);
            // "NoGunSpace" に当たる場合は撃てない
            PlayerController.Instance.weapon.SetCanShoot(!isHit);

        }
    }
}

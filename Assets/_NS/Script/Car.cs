using UnityEngine;
using UnityEngine.AI;

public class Car : WayMoveBase
{
    public Transform rayOrigin; // レイ起点をInspectorで設定
    [SerializeField] float rayDistance = 1f; // チェックする距離
    [SerializeField] float radius = 0.8f; // 車幅に応じて調整
    [SerializeField] NavMeshObstacle obstacle;
    Transform stopTarget = null;
    protected override void Start()
    {
        base.Start();
    }
    protected override void Update()
    {
        // 横に長いカプセルを forward にキャスト
        Vector3 point1 = rayOrigin.position + rayOrigin.right * -0.5f;
        Vector3 point2 = rayOrigin.position + rayOrigin.right * 0.5f;
        float maxDistance = rayDistance;

        IsFrontObsStop = false;
        stopTarget = null;

        if (Physics.CapsuleCast(point1, point2, radius, rayOrigin.forward, out RaycastHit hit, maxDistance, LayerMask.GetMask("NPC")))
        {
            if (hit.collider.transform.IsChildOf(transform)) return;

            var otherCar = hit.collider.GetComponentInParent<Car>();
            if (otherCar != null && otherCar.IsFrontObsStop && otherCar.stopTarget == transform)
            {
                return; // デッドロック回避：相手も自分で止まっているなら止まらない
            }

            IsFrontObsStop = true;
            stopTarget = hit.collider.transform;
        }
        //obstacle.enabled = IsFrontObsStop;

        switch (currentTarget.Type)
        {
            case WayPoint.WayPointType.WaitBlock:
                var objs = currentTarget.NPCCount.GetInsideObjects();
                IsWayPointStop = true;
                break;
        }
        base.Update();
    }

    public override bool TakeDamage(float damage, Vector3 position, Vector3 forward)
    {
        if (damage < 1000) return false;

        base.TakeDamage(damage, position, forward);

        //NavMeshAgentを停止
        Destroy(agent);//.isStopped = true;
        //Rigidbodyを追加し、positionからforward方向に力を加える
        Rigidbody rb = gameObject.GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
        rb.constraints = RigidbodyConstraints.None;
        rb.isKinematic = false;

        forward = forward.normalized;
        forward *= 2;
        forward.y += 10;

        rb.AddForce(forward, ForceMode.VelocityChange);
        rb.AddTorque(new Vector3(90, 90, 90), ForceMode.VelocityChange);

        var particle = References.Instance.GetSceneParticle("explosion");
        var instance = Instantiate(particle, transform.position, Quaternion.identity);
        instance.transform.forward = forward;
        instance.Play();

        return true;
    }
}
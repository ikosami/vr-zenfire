using JetBrains.Annotations;
using System;
using UnityEngine;

public class Heli : MonoBehaviour
{
    [SerializeField] float speed = 5f;
    [SerializeField] Rigidbody rd;

    Vector3 target;
    bool moving = false;

    [SerializeField] GameObject wing;
    public bool IsDead { get; internal set; }

    public void SetTarget(Vector3 endPos)
    {
        target = endPos;
        moving = true;
    }

    public void TakeDamage(float damage, Vector3 position, Vector3 forward)
    {
        Debug.LogError($"Heli TakeDamage {damage} {position} {forward}");
        if (damage < 1000) return;

        if (!moving) return;

        rd.isKinematic = false;
        rd.AddForce(forward * 10, ForceMode.Impulse);

        for (int i = 0; i < 30; i++)
            NPC.Drop(transform.position);

        moving = false;
        Invoke(nameof(DestroySelf), 30f);
    }

    void DestroySelf()
    {
        Destroy(gameObject);
    }


    void Update()
    {
        if (!moving) return;
        wing.transform.rotation = Quaternion.Euler(0, (Time.time * 720) % 360, 0);


        // 向きを移動方向に合わせる
        Vector3 direction = target - transform.position;
        if (direction.sqrMagnitude > 0.001f)
        {
            transform.rotation = Quaternion.LookRotation(direction);
        }

        // 速度で移動
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);

        // 到達したら削除
        if (Vector3.Distance(transform.position, target) < 0.1f)
        {
            Destroy(gameObject);
        }
    }
}

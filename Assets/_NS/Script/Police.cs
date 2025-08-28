using System;
using UnityEngine.AI;
using UnityEngine;
using Random = UnityEngine.Random;

public class Police : MonoBehaviour
{
    [SerializeField] NavMeshAgent agent;
    [SerializeField] float attackDistance = 2f;
    [SerializeField] float rotationSpeed = 5f; // 追加：回転速度（大きくするほど速く向く）

    [SerializeField] GunDataComponent gunDataComponent;
    [SerializeField] Transform target;
    [SerializeField] NPC npc;
    bool isAttacking = false;

    [SerializeField] NPCAnimCallBack npcAnimCallBack;
    public Action<Police> OnDestroyed;

    private void Start()
    {
        npcAnimCallBack.OnShoot += Shoot;
    }

    private void Shoot()
    {
        if (target == null) return;
        var targetPos = target.transform.position;
        targetPos += new Vector3(Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f), Random.Range(-0.5f, 0.5f)); // 目標の位置を少し上にずらす

        gunDataComponent.gunShootAudioName = "gun_shoot";
        gunDataComponent.Shoot(targetPos - gunDataComponent.shootPointTransform.position);
    }

    public void SetTarget(Transform targetTransform)
    {
        target = targetTransform;
    }

    void Update()
    {
        if (target == null) return;
        if (npc.IsDead) return;

        Vector3 selfPos = new Vector3(transform.position.x, 0f, transform.position.z);
        Vector3 targetPos = new Vector3(target.position.x, 0f, target.position.z);
        float distance = Vector3.Distance(selfPos, targetPos);

        if (isAttacking)
        {
            Vector3 direction = (target.position - transform.position).normalized;
            direction.y = 0f;
            if (direction != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }

            if (distance > attackDistance)
            {
                float offset = Random.Range(0f, 1f); // 0〜1の範囲で再生位置をずらす
                npc.animator.Play("Run", 0, offset);
                isAttacking = false;
                agent.isStopped = false;
                agent.SetDestination(new Vector3(target.position.x, transform.position.y, target.position.z));
            }
        }
        else
        {
            if (distance <= attackDistance)
            {
                float offset = Random.Range(0f, 1f); // 0〜1の範囲で再生位置をずらす
                npc.animator.Play("Shoot", 0, offset);
                isAttacking = true;
                agent.isStopped = true;
            }
            else
            {
                agent.SetDestination(new Vector3(target.position.x, transform.position.y, target.position.z));
            }
        }
    }

    void OnDestroy()
    {
        OnDestroyed?.Invoke(this);
    }
}

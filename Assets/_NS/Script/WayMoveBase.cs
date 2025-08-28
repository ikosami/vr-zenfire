using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

public class WayMoveBase : MonoBehaviour
{
    public NavMeshAgent agent;
    public WayPointRoute wayPoints;
    [SerializeField] float MaxHP = 100f; // 最大HP
    [SerializeField] float HP = 100f; // 最大HP

    public bool IsWayPointStop = false;
    public bool IsDamageStop = false;
    public bool IsFrontObsStop = false;

    protected WayPoint currentTarget;
    private WayPoint nextTarget;
    private float waypointReachedThreshold = 0.5f; // この距離内だとWayPointに到着したと判定

    public ObjType objType;
    public enum ObjType
    {
        None,
        Human,
        Car
    }
    // 静的リストで各ObjTypeごとにインスタンスを管理
    public static Dictionary<ObjType, List<WayMoveBase>> InstancesByType = new Dictionary<ObjType, List<WayMoveBase>>()
    {
        { ObjType.Human, new List<WayMoveBase>() },
        { ObjType.Car, new List<WayMoveBase>() }
    };

    public GameObject viewObj;
    public bool IsDead { get; internal set; }

    protected virtual void Start()
    {
        HP = MaxHP;
        if (agent == null)
        {
            agent = GetComponent<NavMeshAgent>();
        }

        if (agent == null)
        {
            Debug.LogError("NavMeshAgentがアタッチされていません。");
            enabled = false;
            return;
        }

        if (wayPoints == null || wayPoints.wayPoints == null || wayPoints.wayPoints.Count == 0)
        {
            Debug.LogError("WayPointsが設定されていないか、WayPointsにポイントがありません。");
            enabled = false;
            return;
        }
        // リストに追加
        if (objType != ObjType.None)
        {
            InstancesByType[objType].Add(this);
        }


        // 最初は一番近いWayPointを目標にする
        currentTarget = FindNearestWayPoint();
        if (currentTarget != null)
        {
            nextTarget = currentTarget.NextPoint;
            //最初のターゲットのほうを向く
            transform.LookAt(currentTarget.transform.position);

            MoveToCurrentTarget();
        }
    }


    protected virtual void Update()
    {
        CheckViewActive();

        if (agent == null) return;

        //if (!agent.isOnNavMesh)
        //{
        //    Debug.LogError($"{gameObject.name} は NavMesh 上にいません。");
        //}

        agent.isStopped = IsWayPointStop || IsDamageStop || IsDead || IsFrontObsStop;
        if (!agent.hasPath && !agent.pathPending)
        {
            agent.isStopped = true;
        }

        if (currentTarget != null && nextTarget != null)
        {
            Vector3 currentDir = (nextTarget.transform.position - currentTarget.transform.position).normalized;
            Vector3 moveDir = (currentTarget.transform.position - transform.position).normalized;

            float dot = Vector3.Dot(currentDir, moveDir);
            if (dot < 0f)
            {
                // 戻っていると判断された場合、次の目標に進む
                MoveToNextTarget();
                return;
            }
        }

        // 目標が設定されていないか到着している場合、次の目標へ
        if (currentTarget == null || HasReachedCurrentTarget())
        {
            MoveToNextTarget();
        }
    }
    private void CheckViewActive()
    {
        float frontDistance = 30f;
        float sideDistance = 10f;
        float backDistance = 3f;

        Transform camT = PlayerController.Instance.Camera.transform;
        Vector3 camForward = camT.forward;
        Vector3 camPos = camT.position;
        Vector3 targetPos = viewObj.transform.position;

        Vector3 toTarget = targetPos - camPos;
        float dot = Vector3.Dot(camForward.normalized, toTarget.normalized);

        float distance = toTarget.magnitude;
        float maxDistance;

        if (dot > 0.7f)
            maxDistance = frontDistance;
        else if (dot > -0.3f)
            maxDistance = sideDistance;
        else
            maxDistance = backDistance;

        bool shouldBeActive = distance < maxDistance;

        if (viewObj.activeSelf != shouldBeActive)
            viewObj.SetActive(shouldBeActive);
    }

    // 最も近いWayPointを見つける
    private WayPoint FindNearestWayPoint()
    {
        if (wayPoints.wayPoints.Count == 0)
            return null;

        WayPoint nearest = null;
        float minDistance = float.MaxValue;

        foreach (WayPoint point in wayPoints.wayPoints)
        {
            if (point == null) continue;

            float distance = Vector3.Distance(transform.position, point.transform.position);
            if (distance < minDistance)
            {
                minDistance = distance;
                nearest = point;
            }
        }

        return nearest;
    }

    // 現在のターゲットへの移動
    private void MoveToCurrentTarget()
    {
        if (currentTarget == null)
        {
            Debug.LogWarning("移動先のWayPointがnullです。");
            return;
        }

        agent.SetDestination(currentTarget.transform.position);
    }

    // 次のターゲットへの移動
    private void MoveToNextTarget()
    {
        // 次のターゲットを現在のターゲットに
        currentTarget = nextTarget;

        // 次のターゲットがなければ終了
        if (currentTarget == null)
        {
            Debug.Log("ルートの終点に到達しました。");
            agent.isStopped = true;
            return;
        }

        // さらに次のターゲットを設定
        nextTarget = currentTarget.NextPoint;

        // 移動開始
        MoveToCurrentTarget();
    }

    // 現在のターゲットに到着したかチェック
    private bool HasReachedCurrentTarget()
    {
        if (currentTarget == null)
            return false;

        // NavMeshAgentが経路計算中なら待つ
        if (agent.pathPending)
            return false;

        // 残りの距離が閾値以下ならターゲットに到着したと判定
        if (agent.remainingDistance <= waypointReachedThreshold)
            return true;

        return false;
    }

    // デバッグ用：Gizmos表示
    private void OnDrawGizmos()
    {
        if (agent != null && agent.hasPath)
        {
            // 移動経路の表示
            Gizmos.color = Color.blue;
            Vector3[] corners = agent.path.corners;
            for (int i = 0; i < corners.Length - 1; i++)
            {
                Gizmos.DrawLine(corners[i], corners[i + 1]);
            }

            // 現在のターゲットの表示
            if (currentTarget != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(currentTarget.transform.position, 0.3f);
            }
        }
    }

    public virtual bool TakeDamage(float damage, Vector3 position, Vector3 forward)
    {
        PoliceController.Instance.PopPolice();

        HP -= damage;
        if (HP <= 0)
        {
            HP = 0;
            IsDead = true;
        }
        return true;
    }

    private void OnDestroy()
    {
        if (objType == ObjType.None) return;

        // リストから削除
        InstancesByType[objType].Remove(this);
    }

}
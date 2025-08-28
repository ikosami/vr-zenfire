using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallBlockObj : MonoBehaviour
{
    //public bool isOKArea => colliderList.Count == 0;
    LayerMask checkLayer;

    // 手の大きさや対象レイヤーに合わせて調整
    [SerializeField] float checkRadius = 0.1f;

    public List<Collider> colliderList = new List<Collider>();
    public Vector3 lastOKPosition;
    public Vector3 fixPosition;

    void Start()
    {
        int layer = LayerMask.NameToLayer("HandBlockWall");
        if (layer == -1)
        {
            Debug.LogError("Layer 'HandBlockWall' が見つかりません！レイヤー設定を確認してください。");
        }
        else
        {
            checkLayer = 1 << layer;
        }
    }
    void LateUpdate()
    {
        if (isOKArea(transform.position))
        {
            lastOKPosition = transform.position;
            fixPosition = transform.position;
        }
        else
        {
            // 可能な限り本来の位置に近い、安全な位置を探す
            fixPosition = FindValidPosition(fixPosition, transform.position);
            transform.position = fixPosition;
        }
    }
    bool isOKArea(Vector3 position)
    {
        return Physics.OverlapSphere(position, checkRadius, checkLayer, QueryTriggerInteraction.Collide).Length == 0;
    }
    Vector3 FindValidPosition(Vector3 current, Vector3 target)
    {
        Vector3 bestPosition = fixPosition;
        float bestDistance = Vector3.Distance(current, target);

        // 8方向（斜め含む）を試しながら、最も安全な位置を探す
        Vector3[] directions = {
            (target - current).normalized, // 目標方向
            Quaternion.Euler(0, 30, 0) * (target - current).normalized,
            Quaternion.Euler(0, -30, 0) * (target - current).normalized,
            Quaternion.Euler(0, 60, 0) * (target - current).normalized,
            Quaternion.Euler(0, -60, 0) * (target - current).normalized,
            Quaternion.Euler(0, 90, 0) * (target - current).normalized,
            Quaternion.Euler(0, -90, 0) * (target - current).normalized
        };

        foreach (Vector3 testDir in directions)
        {
            for (int i = 0; i < 5; i++)
            {
                // 本来の位置がダメなら、少し手前でチェック
                Vector3 testPos = current + testDir * 0.005f * i;
                if (isOKArea(testPos) && Vector3.Distance(testPos, target) < bestDistance)
                {
                    bestDistance = Vector3.Distance(testPos, target);
                    bestPosition = testPos;
                }
            }

        }

        return bestPosition;
    }
    public void Exit(Collider other)
    {
        //colliderList.Remove(other);
        //if (isOKArea)
        //{
        //    lastOKPosition = transform.position;
        //    fixPosition = transform.position;
        //}
    }
}

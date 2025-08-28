using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class WayPointRoute : MonoBehaviour
{
    public List<WayPoint> wayPoints;
    //private void OnDrawGizmos()
    //{
    //    // ルート全体を薄い色で表示
    //    if (wayPoints != null && wayPoints.Count > 1)
    //    {
    //        Gizmos.color = new Color(0.5f, 0.5f, 0.5f, 0.3f);

    //        for (int i = 0; i < wayPoints.Count; i++)
    //        {
    //            if (wayPoints[i] != null && wayPoints[i].NextPoint != null)
    //            {
    //                Gizmos.DrawLine(wayPoints[i].transform.position, wayPoints[i].NextPoint.transform.position);
    //            }
    //        }
    //    }
    //}

}
#if UNITY_EDITOR
// WayPointsのエディタ拡張
[CustomEditor(typeof(WayPointRoute))]
public class WayPointsEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WayPointRoute wayPoints = (WayPointRoute)target;

        if (GUILayout.Button("循環経路として接続"))
        {
            AddAllChildrenAsWayPoints(wayPoints);
            ConnectWayPointsInCircle(wayPoints);
            for (int i = 0; i < wayPoints.wayPoints.Count; i++)
            {
                wayPoints.wayPoints[i].transform.SetAsLastSibling();
            }
        }

        if (GUILayout.Button("向きを次のポイントに向ける"))
        {
            foreach (var point in wayPoints.wayPoints)
            {
                if (point != null && point.NextPoint != null)
                {
                    Vector3 direction = point.NextPoint.transform.position - point.transform.position;
                    if (direction.sqrMagnitude > 0f)
                    {
                        Undo.RecordObject(point.transform, "Rotate WayPoint");
                        point.transform.rotation = Quaternion.LookRotation(direction);
                        EditorUtility.SetDirty(point.transform);
                    }
                }
            }
        }
    }


    // 子オブジェクトをすべてWayPointとして追加
    private void AddAllChildrenAsWayPoints(WayPointRoute wayPointParent)
    {
        wayPointParent.wayPoints.Clear();
        // 現在の子オブジェクトを取得
        Transform parentTransform = wayPointParent.transform;
        List<WayPoint> childWayPoints = new List<WayPoint>();

        // 現在の子オブジェクトからWayPointコンポーネントを持つものを収集
        for (int i = 0; i < parentTransform.childCount; i++)
        {
            Transform child = parentTransform.GetChild(i);
            WayPoint wayPoint = child.GetComponent<WayPoint>();

            // WayPointがなければ追加
            if (wayPoint == null)
            {
                wayPoint = child.gameObject.AddComponent<WayPoint>();
                Undo.RegisterCreatedObjectUndo(wayPoint, "Add WayPoint Component");
            }

            childWayPoints.Add(wayPoint);
        }

        // 現在のwayPoints配列がnullの場合は新しい配列を作成
        List<WayPoint> existingPoints = new List<WayPoint>();
        if (wayPointParent.wayPoints != null)
        {
            existingPoints = new List<WayPoint>(wayPointParent.wayPoints);
        }

        // 重複を避けながら新しいポイントを追加
        bool hasChanges = false;
        foreach (WayPoint wayPoint in childWayPoints)
        {
            if (!existingPoints.Contains(wayPoint))
            {
                existingPoints.Add(wayPoint);
                wayPoint.WayPointParent = wayPointParent;
                EditorUtility.SetDirty(wayPoint);
                hasChanges = true;
            }
        }

        // 変更があった場合のみ更新
        if (hasChanges)
        {
            Undo.RecordObject(wayPointParent, "Add Child WayPoints");
            wayPointParent.wayPoints = existingPoints;
            EditorUtility.SetDirty(wayPointParent);

            Debug.Log($"{childWayPoints.Count}個の子オブジェクトをWayPointsに追加しました（重複を除く）");
        }
        else
        {
            Debug.Log("新しく追加されたWayPointはありません（すべて既に追加済み）");
        }
    }

    // 近い順に接続する
    private void ConnectWayPointsInOrder(WayPointRoute wayPointParent)
    {
        if (wayPointParent.wayPoints == null || wayPointParent.wayPoints.Count < 2)
        {
            Debug.LogWarning("WayPointが2つ以上必要です");
            return;
        }

        // 親の参照を設定
        foreach (var point in wayPointParent.wayPoints)
        {
            if (point != null)
            {
                point.WayPointParent = wayPointParent;
            }
        }

        // 距離に基づいてポイントを並べ替え
        List<WayPoint> sortedPoints = new List<WayPoint>();
        List<WayPoint> remainingPoints = new List<WayPoint>(wayPointParent.wayPoints);

        // 最初のポイントを追加
        sortedPoints.Add(remainingPoints[0]);
        remainingPoints.RemoveAt(0);

        // 最も近いポイントを順番に追加
        while (remainingPoints.Count > 0)
        {
            WayPoint lastPoint = sortedPoints[sortedPoints.Count - 1];
            WayPoint nearestPoint = FindNearestPoint(lastPoint, remainingPoints);

            sortedPoints.Add(nearestPoint);
            remainingPoints.Remove(nearestPoint);
        }

        // Next/Prevポインタを設定
        for (int i = 0; i < sortedPoints.Count; i++)
        {
            sortedPoints[i].NextPoint = (i < sortedPoints.Count - 1) ? sortedPoints[i + 1] : null;
            sortedPoints[i].PrevPoint = (i > 0) ? sortedPoints[i - 1] : null;
        }

        // 配列を更新
        wayPointParent.wayPoints = sortedPoints;

        EditorUtility.SetDirty(wayPointParent);
        foreach (var point in wayPointParent.wayPoints)
        {
            if (point != null)
                EditorUtility.SetDirty(point);
        }
    }

    // 循環経路として接続（最後のポイントと最初のポイントを接続）
    private void ConnectWayPointsInCircle(WayPointRoute wayPointParent)
    {
        ConnectWayPointsInOrder(wayPointParent);

        if (wayPointParent.wayPoints.Count >= 2)
        {
            // 最後のポイントのNextを最初のポイントに
            wayPointParent.wayPoints[wayPointParent.wayPoints.Count - 1].NextPoint = wayPointParent.wayPoints[0];

            // 最初のポイントのPrebを最後のポイントに
            wayPointParent.wayPoints[0].PrevPoint = wayPointParent.wayPoints[wayPointParent.wayPoints.Count - 1];

            EditorUtility.SetDirty(wayPointParent.wayPoints[0]);
            EditorUtility.SetDirty(wayPointParent.wayPoints[wayPointParent.wayPoints.Count - 1]);
        }
    }

    // 最も近いポイントを見つける
    private WayPoint FindNearestPoint(WayPoint source, List<WayPoint> points)
    {
        List<WayPoint> candidates = new List<WayPoint>();
        foreach (var point in points)
        {
            if (point == null) continue;
            Vector3 toPoint = point.transform.position - source.transform.position;
            float angle = Vector3.Angle(source.transform.forward, toPoint);
            if (angle <= 5f)
            {
                candidates.Add(point);
            }
        }
        if (candidates.Count > 0)
        {
            float minDistance = float.MaxValue;
            WayPoint nearestPoint = null;
            foreach (var candidate in candidates)
            {
                float distance = Vector3.Distance(source.transform.position, candidate.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPoint = candidate;
                }
            }
            return nearestPoint;
        }
        else
        {
            // フォールバック：角度制限なしで最も近いポイントを選択
            float minDistance = float.MaxValue;
            WayPoint nearestPoint = null;
            foreach (var point in points)
            {
                if (point == null) continue;
                float distance = Vector3.Distance(source.transform.position, point.transform.position);
                if (distance < minDistance)
                {
                    minDistance = distance;
                    nearestPoint = point;
                }
            }
            return nearestPoint;
        }
    }
}
#endif
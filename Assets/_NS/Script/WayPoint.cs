
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

public class WayPoint : MonoBehaviour
{
    public WayPointRoute WayPointParent;
    public WayPoint NextPoint;
    public WayPoint PrevPoint;

    public enum WayPointType
    {
        None,
        WaitBlock,
        SpeedChange,
        StopTime,
    }
    public WayPointType Type = WayPointType.None;

    public NPCCount NPCCount;


    // ギズモの色とサイズ
    public Color sphereColor = new Color(0.2f, 0.8f, 0.2f, 0.6f);
    public Color lineColor = new Color(1f, 0.5f, 0.2f, 0.8f);
    public float sphereRadius = 0.5f;

    private void OnDrawGizmos()
    {
        // 球体のギズモ
        Gizmos.color = sphereColor;
        Gizmos.DrawSphere(transform.position, sphereRadius);

        var offset = new Vector3(0, 0.4f, 0);

        // 次のポイントへの線
        if (NextPoint != null)
        {
            Gizmos.color = lineColor;
            Gizmos.DrawLine(transform.position + offset, NextPoint.transform.position + offset);

            // 方向を示す矢印
            Vector3 direction = (NextPoint.transform.position - transform.position).normalized;
            Vector3 arrowPos = Vector3.Lerp(transform.position, NextPoint.transform.position, 0.7f);
            direction += offset; // 矢印の位置を少し上に調整
            arrowPos += offset; // 矢印の位置を少し上に調整

            float arrowSize = sphereRadius * 0.8f;

            // 矢印の頭を描画
            Vector3 right = Vector3.Cross(direction, Vector3.up).normalized;
            if (right.magnitude < 0.01f)
                right = Vector3.Cross(direction, Vector3.forward).normalized;

            Vector3 up = Vector3.Cross(right, direction).normalized;

            Gizmos.DrawLine(arrowPos, arrowPos - direction * arrowSize + right * arrowSize * 0.5f);
            Gizmos.DrawLine(arrowPos, arrowPos - direction * arrowSize - right * arrowSize * 0.5f);
            Gizmos.DrawLine(arrowPos, arrowPos - direction * arrowSize + up * arrowSize * 0.5f);
            Gizmos.DrawLine(arrowPos, arrowPos - direction * arrowSize - up * arrowSize * 0.5f);
        }
    }

    // 選択時のギズモ
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sphereRadius * 1.2f);
    }
}

#if UNITY_EDITOR
// WayPointのギズモ表示
[CustomEditor(typeof(WayPoint))]
public class WayPointEditor : Editor
{
    //[DrawGizmo(GizmoType.Selected | GizmoType.NonSelected)]
    //private static void DrawWayPointGizmos(WayPoint wayPoint, GizmoType gizmoType)
    //{
    //    float size = HandleUtility.GetHandleSize(wayPoint.transform.position) * 0.5f;
    //    Color color = Selection.Contains(wayPoint.gameObject) ? Color.yellow : Color.blue;
    //    Handles.color = color;

    //    Vector3 arrowStart = wayPoint.transform.position;
    //    float arrowLength = size * 1f;
    //    Quaternion arrowRotation = Quaternion.LookRotation(wayPoint.transform.forward);
    //    Handles.ArrowHandleCap(0, arrowStart, arrowRotation, arrowLength, EventType.Repaint);
    //}
    private void OnSceneGUI()
    {
        WayPoint wayPoint = (WayPoint)target;

        float size = HandleUtility.GetHandleSize(wayPoint.transform.position) * 0.5f;

        Handles.color = Selection.Contains(wayPoint.gameObject) ? Color.yellow : Color.blue;

        //EditorGUI.BeginChangeCheck();
        //Vector3 newPos = Handles.PositionHandle(wayPoint.transform.position, Quaternion.identity);
        //if (EditorGUI.EndChangeCheck())
        //{
        //    Undo.RecordObject(wayPoint.transform, "Move WayPoint");
        //    wayPoint.transform.position = newPos;
        //}

        // 向きを示す矢印を描画
        Vector3 arrowStart = wayPoint.transform.position;
        float arrowLength = size * 2f;
        Quaternion arrowRotation = Quaternion.LookRotation(wayPoint.transform.forward);
        Handles.ArrowHandleCap(0, arrowStart, arrowRotation, arrowLength, EventType.Repaint);
    }
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        WayPoint current = (WayPoint)target;

        if (targets.Length == 1)
        {
            if (GUILayout.Button("次にWayPointを追加"))
            {
                AddNext(current);
            }
        }

        if (GUILayout.Button("選択中のWayPointを削除"))
        {
            DeleteMultiple(targets);
        }
    }

    private void AddNext(WayPoint current)
    {
        if (current.WayPointParent == null || current.WayPointParent.wayPoints == null)
            return;

        GameObject newObj = Instantiate(current.gameObject, current.transform.parent);
        Undo.RegisterCreatedObjectUndo(newObj, "Add WayPoint");

        WayPoint newWayPoint = newObj.GetComponent<WayPoint>();
        newObj.name = current.gameObject.name;

        int index = current.WayPointParent.wayPoints.IndexOf(current);
        if (index < 0)
            return;

        // リストに挿入
        current.WayPointParent.wayPoints.Insert(index + 1, newWayPoint);

        // Transform順に少しずらす
        newWayPoint.transform.position = current.transform.position + current.transform.forward;

        // 親設定
        newWayPoint.WayPointParent = current.WayPointParent;

        // Next/Prevの設定
        newWayPoint.PrevPoint = current;
        newWayPoint.NextPoint = current.NextPoint;
        if (current.NextPoint != null)
            current.NextPoint.PrevPoint = newWayPoint;

        current.NextPoint = newWayPoint;
        Selection.activeGameObject = newObj;

    }
    private void DeleteMultiple(Object[] objects)
    {
        foreach (Object obj in objects)
        {
            WayPoint wp = obj as WayPoint;
            if (wp == null || wp.WayPointParent == null || wp.WayPointParent.wayPoints == null)
                continue;

            WayPoint prev = wp.PrevPoint;
            WayPoint next = wp.NextPoint;

            if (prev != null) prev.NextPoint = next;
            if (next != null) next.PrevPoint = prev;

            wp.WayPointParent.wayPoints.Remove(wp);
            Undo.DestroyObjectImmediate(wp.gameObject);
            Selection.activeGameObject = prev.NextPoint.gameObject;
        }
    }
}
#endif
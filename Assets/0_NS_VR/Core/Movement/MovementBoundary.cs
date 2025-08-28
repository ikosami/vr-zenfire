using UnityEngine;

/// <summary>
/// プレイヤーの移動範囲を制限し、範囲外に出た場合は画面を暗くするシステム
/// </summary>
public class MovementBoundary : MonoBehaviour
{
    [Header("境界設定")]
    [SerializeField] private BoundaryType boundaryType = BoundaryType.Box;
    [SerializeField] private Vector3 boxSize = new Vector3(10f, 5f, 10f);
    [SerializeField] private float sphereRadius = 5f;
    [SerializeField] private Transform boundaryCenter;

    [Header("フェード設定")]
    [SerializeField] private float fadeDelay = 0.5f;
    [SerializeField] private Color fadeColor = Color.black;

    private Transform playerTransform;
    private bool isOutOfBounds = false;
    private float outOfBoundsTime = 0f;

    public enum BoundaryType
    {
        Box,
        Sphere
    }

    private void Start()
    {
        if (boundaryCenter == null)
        {
            boundaryCenter = transform;
        }

        // プレイヤーのTransformを取得（通常はカメラかXROriginのTransform）
        playerTransform = Camera.main.transform;
    }

    private void Update()
    {
        bool currentlyOutOfBounds = IsOutOfBounds();

        if (currentlyOutOfBounds && !isOutOfBounds)
        {
            // 範囲外に出た瞬間
            outOfBoundsTime = Time.time;
            isOutOfBounds = true;
        }
        else if (!currentlyOutOfBounds && isOutOfBounds)
        {
            // 範囲内に戻った瞬間
            isOutOfBounds = false;
            // フェードを解除
            VRTransitionManager.Instance.Run(
                onOutEnded: () => { /* 範囲内に戻ったので何もしない */ },
                onInEnded: () => { /* フェードインが完了 */ }
            );
        }
        else if (isOutOfBounds && Time.time - outOfBoundsTime >= fadeDelay)
        {
            // 一定時間範囲外にいる場合、画面を暗くする
            VRTransitionManager.Instance.Run(
                onOutEnded: () => { /* フェードアウトが完了 */ },
                onInEnded: () => { /* 範囲外なのでフェードインしない */ }
            );
        }
    }

    private bool IsOutOfBounds()
    {
        Vector3 playerPos = playerTransform.position;
        Vector3 centerPos = boundaryCenter.position;

        switch (boundaryType)
        {
            case BoundaryType.Box:
                Vector3 localPos = boundaryCenter.InverseTransformPoint(playerPos);
                Vector3 halfSize = boxSize * 0.5f;
                return Mathf.Abs(localPos.x) > halfSize.x ||
                       Mathf.Abs(localPos.y) > halfSize.y ||
                       Mathf.Abs(localPos.z) > halfSize.z;

            case BoundaryType.Sphere:
                return Vector3.Distance(playerPos, centerPos) > sphereRadius;

            default:
                return false;
        }
    }

    private void OnDrawGizmos()
    {
        if (boundaryCenter == null) return;

        Gizmos.color = Color.yellow;
        Matrix4x4 rotationMatrix = Matrix4x4.TRS(boundaryCenter.position, boundaryCenter.rotation, Vector3.one);
        Gizmos.matrix = rotationMatrix;

        switch (boundaryType)
        {
            case BoundaryType.Box:
                Gizmos.DrawWireCube(Vector3.zero, boxSize);
                break;

            case BoundaryType.Sphere:
                Gizmos.DrawWireSphere(Vector3.zero, sphereRadius);
                break;
        }
    }
}

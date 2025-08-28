using UnityEngine;

public class HeliManager : MonoBehaviour
{
    [SerializeField] Heli heliPrefab;
    [SerializeField] float interval = 60f;
    float timer;

    void Update()
    {
        timer += Time.deltaTime;
        if (timer >= interval)
        {
            timer = 0f;
            Pop();
        }
    }

    void Pop()
    {
        var playerPos = PlayerController.Instance.Camera.transform.position;
        //プレイヤーの上空の周辺
        playerPos += new Vector3(Random.Range(-40, 40), 40, Random.Range(-40, 40));


        // ランダムな水平方向
        Vector2 dir = Random.insideUnitCircle.normalized;

        // 出現位置と目的地を計算（XZは±150m、Yは40固定）
        Vector3 startOffset = new Vector3(dir.x, 0, dir.y) * -150f;
        Vector3 endOffset = new Vector3(dir.x, 0, dir.y) * 150f;

        Vector3 startPos = playerPos + startOffset;
        Vector3 endPos = playerPos + endOffset;

        var heli = Instantiate(heliPrefab, startPos, Quaternion.identity);
        heli.SetTarget(endPos); // この関数はHeli側で実装済みの想定
    }
}

using UnityEngine;

public class HumanSpawner : MonoBehaviour
{
    [SerializeField] WayPointRoute[] wayPointRoutes;
    [SerializeField] WayMoveBase[] prefabs;
    [SerializeField] int npcCount = 30; // 最大人数
    [SerializeField] float spawnCheckRadius = 1.0f;

    [SerializeField] float SpeedMultiMin = 0.8f;
    [SerializeField] float SpeedMultiMax = 1.2f;
    [SerializeField] WayMoveBase.ObjType objType;
    void Awake()
    {
        SpawnInitialNPCs();
    }

    void Update()
    {
        // 現在の人数を確認
        int currentCount = WayMoveBase.InstancesByType[objType].Count;


        // 不足分を補充
        if (currentCount < npcCount)
        {
            int toSpawn = npcCount - currentCount;
            SpawnNPCs(toSpawn);
        }
    }

    private void SpawnInitialNPCs()
    {
        SpawnNPCs(npcCount);
    }

    private void SpawnNPCs(int count)
    {
        int npcLayerMask = 1 << LayerMask.NameToLayer("NPC");
        int spawned = 0;
        int maxAttempts = count * 15;
        int attempts = 0;

        while (spawned < count && attempts < maxAttempts)
        {
            attempts++;
            var randomRoute = wayPointRoutes[Random.Range(0, wayPointRoutes.Length)];
            var random = randomRoute.wayPoints[Random.Range(0, randomRoute.wayPoints.Count)];

            var pos = random.transform.position;
            pos += (random.NextPoint.transform.position - random.transform.position) * Random.value;

            if (Physics.CheckSphere(pos, spawnCheckRadius, npcLayerMask)) continue;

            spawned++;
            var prefab = prefabs[Random.Range(0, prefabs.Length)];
            var npc = Instantiate(prefab, pos, Quaternion.identity);
            npc.wayPoints = randomRoute;
            npc.agent.speed *= Random.Range(SpeedMultiMin, SpeedMultiMax);
        }
    }
}

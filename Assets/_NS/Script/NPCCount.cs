using System.Collections.Generic;
using UnityEngine;

public class NPCCount : MonoBehaviour
{
    [SerializeField] CollisionChecker[] collisionCheckers;

    private Dictionary<WayMoveBase, int> colliderCounts = new Dictionary<WayMoveBase, int>();

    void Start()
    {
        for (int i = 0; i < collisionCheckers.Length; i++)
        {
            collisionCheckers[i].OnTriggerEnterAction += OnEnter;
            collisionCheckers[i].OnTriggerExitAction += OnExit;
        }
    }

    private void OnEnter(Collider collider)
    {
        var wayMove = collider.GetComponentInParent<WayMoveBase>();
        if (wayMove == null) return;

        if (colliderCounts.ContainsKey(wayMove))
        {
            colliderCounts[wayMove]++;
        }
        else
        {
            colliderCounts[wayMove] = 1;
        }
    }

    private void OnExit(Collider collider)
    {
        var wayMove = collider.GetComponentInParent<WayMoveBase>();
        if (wayMove == null) return;
        if (colliderCounts.ContainsKey(wayMove))
        {
            colliderCounts[wayMove]--;
            if (colliderCounts[wayMove] <= 0)
            {
                colliderCounts.Remove(wayMove);
            }
        }
    }

    public List<WayMoveBase> GetInsideObjects()
    {
        return new List<WayMoveBase>(colliderCounts.Keys);
    }
}

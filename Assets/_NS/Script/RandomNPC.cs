using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[System.Serializable]
public class ChangeMaterialColor
{
    public Renderer mesh;
    public Material[] materials;
}

public class RandomNPC : MonoBehaviour
{
    public static List<RandomNPC> NPCs = new List<RandomNPC>();

    [SerializeField] int kind;
    [SerializeField] ChangeMaterialColor[] materialSettings;

    List<int> _colorIndexes = new List<int>();
    public NPC npc;
    static RandomNPC TargetNPC;

    void Awake()
    {
        NPCs.Add(this);
    }

    void OnDestroy()
    {
        NPCs.Remove(this);
    }

    public void ApplyRandomAppearance(NPC nPC)
    {
        npc = nPC;

        // Material切り替え
        _colorIndexes.Clear();
        if (materialSettings != null && materialSettings.Length > 0)
        {
            for (int i = 0; i < materialSettings.Length; i++)
            {
                var setting = materialSettings[i];

                int colorIndex = -1;
                int retry = 100; // 再試行回数の上限
                while (retry > 0)
                {
                    colorIndex = (setting.materials != null && setting.materials.Length > 0)
                        ? Random.Range(0, setting.materials.Length)
                        : -1;

                    // TargetNPCと被らないか確認
                    if (TargetNPC == null || TargetNPC.GetAppearanceHash() != GetAppearanceHashWithColorIndex(i, colorIndex))
                    {
                        break;
                    }

                    retry--;
                }

                if (colorIndex >= 0)
                {
                    setting.mesh.material = setting.materials[colorIndex];
                }

                _colorIndexes.Add(colorIndex);
            }
        }
    }

    // 指定したインデックスと色インデックスで仮のハッシュを計算
    private long GetAppearanceHashWithColorIndex(int index, int colorIndex)
    {
        unchecked
        {
            long hash = kind;
            for (int i = 0; i < _colorIndexes.Count; i++)
            {
                hash *= 10;
                hash += (i == index) ? colorIndex : _colorIndexes[i];
            }
            return hash;
        }
    }
    public long GetAppearanceHash()
    {
        unchecked
        {
            long hash = 0;
            hash += kind;
            foreach (var idx in _colorIndexes)
            {
                hash *= 10;
                hash += idx;
            }
            return hash;
        }
    }

    public static RandomNPC RefreshToAvoidDuplicateFromMiddle()
    {
        var playerPos = PlayerController.Instance.transform.position;
        var sorted = NPCs.OrderBy(n => Vector3.Distance(n.transform.position, playerPos)).ToList();
        if (sorted.Count == 0)
        {
            Debug.LogError("NPCがいません");
            return null;
        }

        var midNPC = sorted[sorted.Count / 2];

        var hash = midNPC.GetAppearanceHash();

        foreach (var npc in NPCs)
        {
            if (npc == midNPC) continue;
            if (npc.npc.IsDead) continue;

            int retry = 100;
            while (true)
            {
                // 違う見た目
                if (hash != npc.GetAppearanceHash())
                {
                    break;
                }
                npc.ApplyRandomAppearance(npc.npc);

                retry--;
                if (retry <= 0)
                {
                    Debug.LogError("違う見た目が作れませんでした");
                }
            }

        }

        TargetNPC = midNPC;
        return midNPC;
    }
}

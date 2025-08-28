using UnityEngine;
using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using Random = UnityEngine.Random;
using UnityEngine.Audio;

public class PoliceController : MonoBehaviour
{
    public static PoliceController Instance;

    [SerializeField] Police[] policePrefabs;

    [SerializeField] AudioSource audioSource;

    bool isAlert = false;
    List<Police> activePolice = new List<Police>();
    float alertCancelDistance = 20f; // 警戒解除距離（必要に応じて調整）

    [SerializeField] Transform playerRespawnPoint; // 警察のスポーンポイント

    private void Awake()
    {
        Instance = this;
    }

    private void Start()
    {
        // 警察の初期化や設定が必要な場合はここに記述

        PlayerController.Instance.OnDead += () =>
        {
            foreach (var police in activePolice)
            {
                if (police != null) Destroy(police.gameObject);
            }
            activePolice.Clear();
            CancelAlert();

            isAlert = false;

            // 警察のスポーンポイントにプレイヤーをリスポーン
            PlayerController.Instance.transform.position = playerRespawnPoint.position;
            PlayerController.Instance.transform.rotation = playerRespawnPoint.rotation;
        };
    }

    public void PopPolice()
    {
        if (isAlert) return;

        audioSource.Play();

        int spawnCount = 0;
        int maxAttempts = 100;
        int attempts = 0;
        LayerMask obstacleMask = ~(LayerMask.GetMask("NPC", "Player", "PlayerCar"));

        while (spawnCount < 5 && attempts < maxAttempts)
        {
            attempts++;

            Vector3 randomOffset = new Vector3(
                Random.Range(10f, 20f) * (Random.value > 0.5f ? 1 : -1),
                0f,
                Random.Range(10f, 20f) * (Random.value > 0.5f ? 1 : -1)
            );
            Vector3 spawnPos = transform.position + randomOffset;
            spawnPos.y = transform.position.y;

            Vector3 dirToCam = (PlayerController.Instance.Camera.transform.position - spawnPos).normalized;
            Ray ray = new Ray(spawnPos, dirToCam);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, obstacleMask))
            {
                Police police = Instantiate(policePrefabs[Random.Range(0, policePrefabs.Length)], spawnPos, Quaternion.identity);
                police.SetTarget(PlayerController.Instance.Camera.transform);
                police.OnDestroyed += OnPoliceDestroyed;
                activePolice.Add(police);
                spawnCount++;
            }
        }

        if (spawnCount > 0)
        {
            isAlert = true;
        }
    }

    void Update()
    {
        if (!isAlert) return;

        // 全員いなくなった場合
        if (activePolice.Count == 0)
        {
            CancelAlert();
            return;
        }

        // 一番近い警官との距離が一定以上になった場合
        float minDistance = float.MaxValue;
        Vector3 selfPos = new Vector3(transform.position.x, 0f, transform.position.z);

        foreach (var police in activePolice)
        {
            if (police == null) continue;

            Vector3 policePos = new Vector3(police.transform.position.x, 0f, police.transform.position.z);
            float dist = Vector3.Distance(policePos, selfPos);
            if (dist < minDistance)
            {
                minDistance = dist;
            }
        }

        if (minDistance > alertCancelDistance)
        {
            foreach (var police in activePolice)
            {
                if (police != null) Destroy(police.gameObject);
            }
            activePolice.Clear();
            CancelAlert();
        }
    }

    void OnPoliceDestroyed(Police police)
    {
        activePolice.Remove(police);
    }

    void CancelAlert()
    {
        audioSource.Stop();
        isAlert = false;
    }
}

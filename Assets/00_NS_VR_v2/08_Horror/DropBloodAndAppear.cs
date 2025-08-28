using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropBloodAndAppear : MonoBehaviour
{
    [SerializeField] LookAtObjectFromAreaEventHandler _lookAtObjectFromAreaEventHandler;
    [SerializeField] Blood bloodPrefab;
    [SerializeField] SpriteRenderer[] bloodSpriteRenderers;

    [SerializeField] Transform bloodStartPoint;
    [SerializeField] Transform bloodEndPoint;

    [SerializeField] GameObject enemyObj;
    Vector3 enemyPopPos;

    bool isInTrigger = false;
    [SerializeField] float viewAngleThreshold = 30f; // 許容する角度

    int lastIndex = -1;
    bool isEventEnd = false;
    float speed = 4f;

    void Start()
    {

        _lookAtObjectFromAreaEventHandler.OnEnterArea.AddListener(_OnEnterArea);
        _lookAtObjectFromAreaEventHandler.OnExitArea.AddListener(_OnExitArea);

        enemyPopPos = enemyObj.transform.position;
        enemyObj.transform.position = new Vector3(enemyPopPos.x, enemyPopPos.y + 1f, enemyPopPos.z);

        StartCoroutine(SpawnBloodRoutine());
    }
    void Update()
    {
        if (isEventEnd) return;

        Vector3 targetPos;

        if (_lookAtObjectFromAreaEventHandler.IsEnterArea)
        {
            targetPos = enemyPopPos;
        }
        else
        {
            targetPos = enemyPopPos + new Vector3(0, 1f, 0);
        }
        enemyObj.transform.position = Vector3.MoveTowards(
            enemyObj.transform.position,
            targetPos,
            speed * Time.deltaTime
        );

        if (!_lookAtObjectFromAreaEventHandler.IsEnterArea) return;

        if (_lookAtObjectFromAreaEventHandler.IsLookAt)
        {
            // イベント発火（1回のみ等の制御が必要なら別途フラグを立てて管理）
            isEventEnd = true;
            StartCoroutine(DeadIE());
        }
    }
    private IEnumerator DeadIE()
    {
        float duration = 1f;
        float time = 0f;

        Vector3 startPos = enemyObj.transform.position;
        Vector3 targetPos = Camera.main.transform.position;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;
            enemyObj.transform.position = Vector3.Lerp(startPos, targetPos, t);
            yield return null;
        }

        // ゲームオーバー
        // PlayerController.Instance.GameOver();
    }


    private void _OnEnterArea()
    {
        isInTrigger = true;
        enemyObj.SetActive(true);
    }

    private void _OnExitArea()
    {
        isInTrigger = false;
    }
    IEnumerator SpawnBloodRoutine()
    {
        while (true)
        {
            if (isInTrigger)
            {
                SpawnBlood();
            }

            yield return new WaitForSeconds(1f);
        }
    }

    void SpawnBlood()
    {
        int index = GetRandomIndexExcludingLast();
        SpriteRenderer selectedRenderer = bloodSpriteRenderers[index];
        lastIndex = index;

        Blood blood = Instantiate(bloodPrefab);
        blood.SetPos(bloodStartPoint.position, bloodEndPoint.position, () =>
        {
            selectedRenderer.color = new Color(1f, 0f, 0f, 1f);
            StartCoroutine(FadeOutBlood(selectedRenderer));
            SoundManager.Instance.Play3D("blood_drop", bloodEndPoint.position);
        });
        
    }

    int GetRandomIndexExcludingLast()
    {
        if (bloodSpriteRenderers.Length <= 1) return 0;

        int index;
        do
        {
            index = Random.Range(0, bloodSpriteRenderers.Length);
        } while (index == lastIndex);

        return index;
    }

    IEnumerator FadeOutBlood(SpriteRenderer renderer)
    {
        float duration = 2f;
        float elapsed = 0f;
        Color color = renderer.color;

        while (elapsed < duration)
        {
            float alpha = Mathf.Lerp(1f, 0f, elapsed / duration);
            renderer.color = new Color(color.r, color.g, color.b, alpha);
            elapsed += Time.deltaTime;
            yield return null;
        }

        renderer.color = new Color(color.r, color.g, color.b, 0f);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class FlashAndChase : MonoBehaviour
{
    [SerializeField] LookAtObjectFromAreaEventHandler _lookAtObjectFromAreaEventHandler;
    bool isInTrigger = false;
    bool isEventEnd = false;

    [SerializeField] GameObject enemyObj;
    [SerializeField] Light _light;

    [SerializeField] Transform[] posList;
    [SerializeField] GameObject darkObj;

    int state = 0;
    float timer = 0f;
    [SerializeField] float lookTime = 1f;
    [SerializeField] float lightDelay = 2f;

    [SerializeField] Animation enemyAnimation;

    public UnityEvent OnEventEnd;
    // Start is called before the first frame update
    void Start()
    {
        _lookAtObjectFromAreaEventHandler.OnStartLookAt.AddListener(_OnStartLookAt);
        _lookAtObjectFromAreaEventHandler.OnExitArea.AddListener(_OnExitArea);
    }

    // Update is called once per frame
    void Update()
    {
        if(isEventEnd) return;
        if(_lookAtObjectFromAreaEventHandler.IsLookAt) {
            timer += Time.deltaTime;

            if (state == 0 && timer > lookTime)
            {
                _light.enabled = false;
                if(SoundManager.Instance != null) SoundManager.Instance.Play3D("light_blink", _light.transform.position);
                // PlayerController.Instance.spotLight.enabled = false;
                enemyObj.SetActive(true); // 敵を再表示
                darkObj.SetActive(true);
                enemyObj.transform.position = posList[0].position;
                state = 1;
                timer = 0f;
            }
            else if (state == 2 && timer > lookTime)
            {
                _light.enabled = false;
                if(SoundManager.Instance != null) SoundManager.Instance.Play3D("light_blink", _light.transform.position);
                // PlayerController.Instance.spotLight.enabled = false;
                enemyObj.SetActive(true); // 敵を再表示
                darkObj.SetActive(true);
                enemyObj.transform.position = posList[1].position;
                state = 3;
                timer = 0f;
            }
            else if (state == 4 && timer > lookTime)
            {
                enemyObj.SetActive(true); // 敵を再表示
                darkObj.SetActive(true);
                enemyAnimation.enabled = true;
                // PlayerController.Instance.spotLight.enabled = true;
                enemyObj.transform.position = posList[2].position;
                StartCoroutine(DeadIE());
                isEventEnd = true;
            }
        }

        // ステート進行
        if (state == 1)
        {
            timer += Time.deltaTime;
            if (timer > lightDelay)
            {
                _light.enabled = true;
                // PlayerController.Instance.spotLight.enabled = true;
                enemyObj.SetActive(false); // 敵を消す
                darkObj.SetActive(false);
                state = 2;
                timer = 0f;
            }
        }
        else if (state == 3)
        {
            timer += Time.deltaTime;
            if (timer > lightDelay)
            {
                _light.enabled = true;
                // PlayerController.Instance.spotLight.enabled = true;
                enemyObj.SetActive(false); // 敵を消す
                darkObj.SetActive(false);
                state = 4;
                timer = 0f;
            }
        }
    }

    void _OnExitArea() {
        isInTrigger = false;

        if (state < 1)
        {
            return;
        }

        // ⑤になる前ならリセット＆再発生不可
        if (state < 4)
        {
            _light.enabled = true;
            enemyObj.SetActive(false); // 敵を非表示にするなどの処理
            darkObj.SetActive(false);
            isEventEnd = true;
            // PlayerController.Instance.spotLight.enabled = true;
        }
    }
    
    void _OnStartLookAt() {
        isInTrigger = true;
    }

    private IEnumerator DeadIE()
    {
        float duration = 0.5f;
        float time = 0f;

        Vector3 startPos = enemyObj.transform.position;

        while (time < duration)
        {
            time += Time.deltaTime;
            float t = time / duration;

            // プレイヤーの現在位置を取得（高さは敵と揃える）
            Vector3 targetPos = Camera.main.transform.position;
            targetPos.y = startPos.y;

            // 現在位置と目標位置から補間移動
            Vector3 currentPos = Vector3.Lerp(startPos, targetPos, t);
            enemyObj.transform.position = currentPos;

            // 向きもリアルタイムで追従
            Vector3 dir = (targetPos - currentPos).normalized;
            if (dir.sqrMagnitude > 0f)
            {
                Quaternion lookRot = Quaternion.LookRotation(dir);
                lookRot = Quaternion.Euler(0f, lookRot.eulerAngles.y, 0f);
                enemyObj.transform.rotation = lookRot;
            }

            yield return null;
        }

        OnEventEnd?.Invoke();
    }
}

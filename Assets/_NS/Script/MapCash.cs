using System.Collections;
using UnityEngine;

public class MapCash : MonoBehaviour
{
    public bool isRepop = true;
    [SerializeField] GameObject cashObj;
    [SerializeField] Collider cashCollider;
    [SerializeField] CollisionChecker[] collisionChecker;

    public float WaitGetTime = 0;

    private void Start()
    {
        // 出現時にコライダー無効化し、一定時間後に有効化
        cashCollider.enabled = false;
        StartCoroutine(EnableColliderAfterDelay(WaitGetTime));

        for (int i = 0; i < collisionChecker.Length; i++)
        {
            collisionChecker[i].OnTriggerEnterAction += (other) =>
            {
                if (!cashCollider.enabled) return;

                if (other.CompareTag("Player") && cashObj.activeSelf)
                {
                    Money();
                }
            };
        }
    }

    private IEnumerator EnableColliderAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        cashCollider.enabled = true;
    }

    private void Update()
    {
        WaitGetTime -= Time.deltaTime;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && cashObj.activeSelf)
        {
            Money();
        }
    }

    void Money()
    {
        CashController.Instance.Cash += ParamData.Instance.DropMoney;
        cashObj.SetActive(false);
        SoundManager.Instance.Play("coin");


        if (isRepop)
        {
            NS.Util.CoroutineRunner.WaitForSeconds(() =>
            {
                cashObj.SetActive(true);
            }, 60);
        }
    }
}

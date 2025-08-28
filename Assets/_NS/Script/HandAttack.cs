using UnityEngine;

public class HandAttack : MonoBehaviour
{
    [SerializeField] string audioName = "punch";
    [SerializeField] float damage = 20;

    // 追加：速度検出用Transformと閾値
    [SerializeField] Transform velocitySource;
    [SerializeField] float velocityThreshold = 2f;

    Vector3 prevLocalPos;
    float currentVelocity;

    void Start()
    {
        if (velocitySource != null)
            prevLocalPos = velocitySource.localPosition;
    }
    void Update()
    {
        if (velocitySource != null)
        {
            Vector3 cur = velocitySource.localPosition;
            currentVelocity = (cur - prevLocalPos).magnitude / Time.deltaTime;
            prevLocalPos = cur;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (velocitySource != null)
        {
            if (currentVelocity < velocityThreshold)
            {
                return;
            }
        }

        WayMoveBase baseEnemyBehavior = other.GetComponentInParent<WayMoveBase>();
        if (baseEnemyBehavior != null)
        {
            if (!baseEnemyBehavior.IsDead)
            {
                // Deal damage to enemy
                if (baseEnemyBehavior.TakeDamage(damage, transform.position, transform.forward))
                {
                    SoundManager.Instance.Play(audioName);
                }
            }
        }
    }
}

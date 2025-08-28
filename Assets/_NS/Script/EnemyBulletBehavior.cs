
using UnityEngine;

public class EnemyBulletBehavior : MonoBehaviour
{
    protected float damage;
    protected float speed;

    protected float selfDestroyDistance;
    protected float distanceTraveled = 0;
    [SerializeField] protected Rigidbody rb;
    public bool isPlayerGun = false;
    [SerializeField] Vector3 initPos = Vector3.zero;
    Vector3 initForward = Vector3.zero;
    float flyTime = 0;
    [SerializeField] string audioName = "";

    float checkTime = 0;


    [SerializeField] GameObject explosionPrefab;

    public virtual void Init(float damage, float speed, float selfDestroyDistance)
    {
        this.damage = damage;
        this.speed = speed;

        this.selfDestroyDistance = selfDestroyDistance;
        distanceTraveled = 0;

        gameObject.SetActive(true);
        initForward = transform.forward;

        checkTime = 0;
    }

    private void Update()
    {
        checkTime += Time.deltaTime;
    }

    protected virtual void FixedUpdate()
    {
        distanceTraveled += speed * Time.fixedDeltaTime;

        Vector3 nextPos = initPos + initForward * distanceTraveled;
        rb.MovePosition(nextPos);

        if (selfDestroyDistance != -1)
        {
            if (distanceTraveled >= selfDestroyDistance)
            {
                SelfDestroy();
            }
        }
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (ApplyDamageToTarget(other))
        {
            if (audioName != "")
                SoundManager.Instance.Play3D(audioName, transform.position);

            Explosion();
            SelfDestroy();
        }
    }

    public void SelfDestroy()
    {
        Destroy(gameObject);
    }

    public void Explosion()
    {
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            explosion.transform.forward = transform.forward;
            Destroy(explosion, 3f);

            // 範囲攻撃の処理
            float explosionRadius = 4f; // 爆発の範囲（半径）
            Collider[] hitColliders = Physics.OverlapSphere(transform.position, explosionRadius);
            foreach (Collider hitCollider in hitColliders)
            {
                ApplyDamageToTarget(hitCollider);
            }
        }
    }

    private bool ApplyDamageToTarget(Collider target)
    {
        //if (checkTime <= 0.1f) return false;

        //Debug.LogError("Bullet target:" + target.gameObject.name + $" |  {transform.position} {distanceTraveled} :  initPos:{initPos}  checkTime:{checkTime}");

        // プレイヤーに当たった場合の処理
        if (!isPlayerGun)
        {
            if (target.gameObject.layer == PhysicsHelper.LAYER_PLAYER)
            {
                PlayerController characterBehaviour = target.GetComponentInParent<PlayerController>();
                if (characterBehaviour != null && characterBehaviour.TakeDamage(damage))
                {
                    return true;
                }
            }
            else if (target.gameObject.layer == PhysicsHelper.LAYER_PLAYER_CAR)
            {
                PlayerController.Instance.TakeDamage(damage);
            }

        }

        // 敵に当たった場合の処理
        if (isPlayerGun)
        {
            WayMoveBase baseEnemyBehavior = target.GetComponentInParent<WayMoveBase>();
            if (baseEnemyBehavior != null && !baseEnemyBehavior.IsDead)
            {
                baseEnemyBehavior.TakeDamage(damage, transform.position, transform.forward);
                return true;
            }

            Heli heli = target.GetComponentInParent<Heli>();
            if (heli != null && !heli.IsDead)
            {
                heli.TakeDamage(damage, transform.position, transform.forward);
                return true;
            }
        }
        // 障害物に当たった場合は即座に破壊
        if (target.gameObject.layer == PhysicsHelper.LAYER_OBSTACLE)
        {
            return true;
        }

        return false;
    }
    // 既存の SetPosition メソッド
    public EnemyBulletBehavior SetPosition(Vector3 position)
    {
        transform.position = position;
        initPos = transform.position;
        return this;
    }

    // 既存の SetEulerAngles メソッド
    public EnemyBulletBehavior SetEulerAngles(Vector3 eulerAngles)
    {
        gameObject.transform.eulerAngles = eulerAngles;
        return this;
    }

    // コメントアウトされていた SetPositionRotation メソッドを復元
    public EnemyBulletBehavior SetPositionRotation(Transform spawnTarget)
    {
        transform.parent = spawnTarget;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
        initPos = transform.position;
        transform.parent = null;
        return this;
    }
}

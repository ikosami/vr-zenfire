
using Ev;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class GunDataComponent : MonoBehaviour
{
    public float spanTime;

    public EnemyBulletBehavior bulletPrefab;
    public float bulletSpeed;
    public Transform shootPointTransform;
    public ParticleSystem gunFireParticle;
    public AudioClip shotAudio;
    public bool isHave = false;
    public bool isEnemy = false;
    public int damage = 200;

    Vector3 defaultScale;
    Vector3 shootDirection;
    bool isShoot = false;
    public string gunShootAudioName = "";

    int BulletNum = 1;

    string _poseName = "Gun";
    string _poseName2 = "Gun2";
    ControllerSide controllerSide = ControllerSide.Right;

    private void Awake()
    {

        EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, new Ev.Events.HandPoseChange()
        {
            PoseName = _poseName,
            ControllerSide = controllerSide,
            HandPartType = HandPartType.All
        });
        EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_LOCK, new Ev.Events.HandPoseLock()
        {
            ControllerSide = controllerSide,
            HandPartType = HandPartType.All
        });
    }

    internal void SetSetting(WeaponData data)
    {
        damage = data.Damage;
        bulletSpeed = data.Speed;
        spanTime = data.SpanTime;
        gunShootAudioName = data.audioName;
        BulletNum = data.BulletNum;
    }

    public void Have(Transform gunParent)
    {
        defaultScale = transform.localScale;
        transform.SetParent(gunParent);
        transform.localScale = Vector3.one;
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;

        int playerLayer = LayerMask.NameToLayer("Player");
        SetLayerRecursively(transform, playerLayer);

        var rigid = GetComponent<Rigidbody>();
        if (rigid != null)
            rigid.isKinematic = true;
        isHave = true;
    }

    public void Release()
    {
        transform.SetParent(null);
        transform.localScale = defaultScale;

        int playerLayer = LayerMask.NameToLayer("EnemyGun");
        SetLayerRecursively(transform, playerLayer);


        var rigid = GetComponent<Rigidbody>();
        if (rigid != null)
            rigid.isKinematic = false;

        isHave = false;
    }



    void SetLayerRecursively(Transform obj, int layer)
    {
        obj.gameObject.layer = layer;
        foreach (Transform child in obj)
        {
            SetLayerRecursively(child, layer);
        }
    }
    float nextShootTime = 0;

    public bool CanShoot { get; internal set; }

    void Update()
    {
        if (!isHave) return;
        if (!CanShoot) return;

        if (isEnemy)
        {
            return;
        }
        else
        {
            if (!VRInputManager.Instance.IsTriggerPressed(ControllerSide.Right, true))
            {
                return;
            }
        }

        var characterBehaviour = PlayerController.Instance;

        if (nextShootTime >= Time.timeSinceLevelLoad)
        {
            return;
        }
        nextShootTime = Time.timeSinceLevelLoad + spanTime;

        VibrationController.Instance.Play("click", ControllerSide.Right);

        EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, new Ev.Events.HandPoseChange() { PoseName = _poseName2, ControllerSide = controllerSide, HandPartType = HandPartType.All });
        Shoot(shootPointTransform.forward);

        NS.Util.CoroutineRunner.Instance.WaitRun(() =>
        {
            EventManager.TriggerEvent(Ev.Events.Name.HAND_POSE_CHANGE, new Ev.Events.HandPoseChange()
            {
                PoseName = _poseName,
                ControllerSide = controllerSide,
                HandPartType = HandPartType.All
            });
        }, 0.2f);
    }

    private void LateUpdate()
    {
        if (!isShoot) return;
        isShoot = false;


        RaycastHit hit;
        int mask = LayerMask.GetMask("NoGunSpace");

        // "NoGunSpace" レイヤーに当たるかどうかを判定
        bool isHit = Physics.Raycast(shootPointTransform.position, shootPointTransform.forward, out hit, Mathf.Infinity, mask);
        if (isHit)
        {
            // 当たった場合、弾を発射しない
            return;
        }

        if (gunShootAudioName != "")
        {
            SoundManager.Instance.Play(gunShootAudioName);
        }
        for (int i = 0; i < BulletNum; i++)
        {
            //bullet.transform.localScale = transform.parent.localScale;
            Vector3 direction = shootDirection.normalized;
            if (i > 0)
            {
                float spreadAngle = 35f; // 散らす角度（度）
                direction = Quaternion.Euler(
                    Random.Range(-spreadAngle, spreadAngle),
                    Random.Range(-spreadAngle, spreadAngle),
                    Random.Range(-spreadAngle, spreadAngle)) * direction;
            }
            Quaternion rotation = Quaternion.LookRotation(direction);
            EnemyBulletBehavior bullet = Instantiate(bulletPrefab, shootPointTransform.position, rotation);

            //タイミング的に生成と設定で2回必要
            bullet.SetPosition(shootPointTransform.position);
            bullet.transform.rotation = rotation;

            bullet.name = "Bullet" + i;
            bullet.isPlayerGun = true;

            var layer = "Bullet";
            if (isEnemy)
            {
                layer = "EnemyBullet";
                bullet.isPlayerGun = false;
            }
            int bulletLayer = LayerMask.NameToLayer(layer);
            SetLayerRecursively(bullet.transform, bulletLayer);


            // 弾を初期化する。現在のダメージ量 (GetCurrentDamage())、設定された弾速 (bulletSpeed)、射程距離？ (200) を渡す
            bullet.Init(damage, bulletSpeed, 200);
        }
        // 銃口のパーティクルエフェクト (gunFireParticle) を再生する
        gunFireParticle.Play();

        // オーディオコントローラーを使って射撃音 (enemyShot) を再生する
        //SoundManager.Instance.Play(shotAudio);
    }

    public void Shoot(Vector3 shootDirection)
    {
        isShoot = true;
        this.shootDirection = shootDirection;
    }

}

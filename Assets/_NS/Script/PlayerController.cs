using System;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.XR.OpenXR.Features.Interactions.HTCViveControllerProfile;
using UnityEngine.SceneManagement;
using System.Runtime.ConstrainedExecution;

public class PlayerController : MonoBehaviour
{
    public Camera Camera;
    [SerializeField] Rigidbody rb;
    [SerializeField] float jumpPower = 10;
    bool isJumping = false;
    private float lastJumpTime = 0f;
    [SerializeField] float moveSpeed = 2;

    public static PlayerController Instance;
    public NPC target;
    GameObject view = null;
    [SerializeField] Transform targetUI;
    public PlayerWeapon weapon;
    [SerializeField] GameObject VRCameraObj;
    [SerializeField] Collider playerCollider;
    [SerializeField] DamageEffect damageEffect;
    public ItemInventory itemInventory = new ItemInventory();
    public Action OnDead;
    float maxHp = 600;
    float currentHp = 100;
    bool isDead => currentHp <= 0;

    private void Awake()
    {
        Instance = this;
        itemInventory.LoadItems();
    }

    private void Start()
    {
        currentHp = maxHp;
    }

    private void Update()
    {
        if (isDead) return;

        if (target == null)
        {
            if (view != null) Destroy(view.gameObject);

            target = RandomNPC.RefreshToAvoidDuplicateFromMiddle().npc;
            if (target == null) return;
            target.IsTarget = true;
            var viewNPC = Instantiate(target, targetUI);
            viewNPC.isUI = true;


            Destroy(viewNPC.GetComponent<LookAtTarget>());
            Destroy(viewNPC.GetComponent<NavMeshAgent>());
            //var anims = viewNPC.GetComponentsInChildren<Animator>();
            //for (int i = 0; i < anims.Length; i++)
            //{
            //    Destroy(anims[i]);
            //}
            var Joints = viewNPC.GetComponentsInChildren<Joint>();
            for (int i = 0; i < Joints.Length; i++)
            {
                Destroy(Joints[i]);
            }
            var rigids = viewNPC.GetComponentsInChildren<Rigidbody>();
            for (int i = 0; i < rigids.Length; i++)
            {
                Destroy(rigids[i]);
            }
            var col = viewNPC.GetComponentsInChildren<Collider>();
            for (int i = 0; i < col.Length; i++)
            {
                Destroy(col[i]);
            }
            view = viewNPC.gameObject;
            Destroy(viewNPC);

            viewNPC.transform.localPosition = Vector3.zero;
            viewNPC.transform.localRotation = Quaternion.identity;

            viewNPC.viewObj.SetActive(true);
        }

        Jump();
        Move();
    }

    private void Move()
    {
        // カメラの向き基準で移動方向ベクトルを計算
        var movementInput = VRInputManager.Instance.LeftStickValue;
        var movementDirection = Camera.transform.forward * movementInput.y + Camera.transform.right * movementInput.x;
        movementDirection.y = 0; // Y方向の移動を無効化

        // 方向ベクトルを正規化し、速度を掛ける
        if (movementDirection.magnitude > 0.01f)
        {
            movementDirection.Normalize();
            Vector3 targetVelocity = movementDirection * moveSpeed;

            // 現在の水平速度を取得
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            // 目標速度に向けて補間（瞬時に変わるのを避けるため）
            Vector3 adjustedVelocity = Vector3.Lerp(horizontalVelocity, targetVelocity, 0.5f);

            // Y速度を保持して水平方向の速度のみを設定
            rb.linearVelocity = new Vector3(adjustedVelocity.x, rb.linearVelocity.y, adjustedVelocity.z);
        }
        else
        {
            // 現在の水平速度を取得
            Vector3 horizontalVelocity = new Vector3(rb.linearVelocity.x, 0, rb.linearVelocity.z);

            // 目標速度に向けて補間（瞬時に変わるのを避けるため）
            Vector3 adjustedVelocity = Vector3.Lerp(horizontalVelocity, Vector3.zero, 0.9f);

            // Y速度を保持して水平方向の速度のみを設定
            rb.linearVelocity = new Vector3(adjustedVelocity.x, rb.linearVelocity.y, adjustedVelocity.z);
        }
    }

    private void Jump()
    {
        if (!isJumping)
        {
            if (VRInputManager.Instance.IsTriggerPressed(ControllerSide.Left, true))
            {
                rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
                lastJumpTime = Time.time;
                isJumping = true;
            }
        }
        else
        {
            if (Time.time - lastJumpTime >= 0.2f)
            {
                int playerLayer = LayerMask.NameToLayer("Player");
                int handLayer = LayerMask.NameToLayer("HandCollider");

                // Create a mask with all layers
                int allLayers = ~0;

                // Remove Player and Hand layers from the mask
                int layerMask = allLayers & ~(1 << playerLayer) & ~(1 << handLayer);

                Vector3 origin = rb.transform.position + Vector3.up * 0.01f;
                float rayLength = 0.5f;

                bool isGrounded = Physics.Raycast(origin, Vector3.down, rayLength, layerMask);

                if (isGrounded)
                {
                    isJumping = false;
                }
            }
        }
    }
    public float damageCooldown = 0.5f;
    private float lastDamageTime = -Mathf.Infinity;
    public bool TakeDamage(float damage)
    {
        if (Time.time - lastDamageTime < damageCooldown)
            return false;

        lastDamageTime = Time.time;

        damageEffect.PlayEffect();

        VibrationController.Instance.Play("click", ControllerSide.Both);

        currentHp -= damage;
        if (currentHp <= 0)
        {
            if (playerCar != null)
                RemoveCar(playerCar);

            lastDamageTime = Time.time + 5;

            VRTransitionManager.Instance.fadeColor = Color.black;
            VRTransitionManager.Instance.fadeDuration = 1f;
            VRTransitionManager.Instance.Run(() =>
            {
                currentHp = maxHp;
                OnDead?.Invoke();
            }, null);
            return true;
        }
        else
        {
            // HPが減ったときの処理をここに追加
            // 例: UIの更新、エフェクトの再生など
            Debug.Log("Player HP: " + currentHp);
        }

        return false;
    }

    public void ChangeWeapon(string weaponName)
    {
        weapon.ChangeWeapon(weaponName);
    }


    PlayerCar playerCar;
    public void SetCar(PlayerCar car)
    {
        if (car == null) return;
        if (car.isPlayer) return;

        if (car.lastRemoveTime + 1 > Time.time)
        {
            return;
        }
        playerCar = car;
        SoundManager.Instance.Play("car_door");

        car.SetPlayer(true);
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        VRCameraObj.transform.SetParent(car.playerPos);
        VRCameraObj.transform.localPosition = Vector3.zero;
        VRCameraObj.transform.localRotation = Quaternion.identity;
        playerCollider.enabled = false;
    }

    public void RemoveCar(PlayerCar car)
    {
        if (car == null) return;
        if (!car.isPlayer) return;
        playerCar = null;

        SoundManager.Instance.Play("car_door");

        //降りる
        car.SetPlayer(false);
        transform.position = car.downPos.position;
        transform.rotation = car.downPos.rotation;
        VRCameraObj.transform.SetParent(this.transform);
        VRCameraObj.transform.localPosition = Vector3.zero;
        VRCameraObj.transform.localRotation = Quaternion.identity;

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
        playerCollider.enabled = true;
        return;
    }
}

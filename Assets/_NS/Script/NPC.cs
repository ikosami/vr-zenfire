using UnityEngine;

public class NPC : WayMoveBase
{
    [SerializeField] LookAtTarget lookAtTarget;
    public Animator animator;
    [SerializeField] RagdollState ragdollState;
    [SerializeField] RandomNPC randomNPC;

    public bool isUI;
    public bool isStopHuman = false;
    public bool isPolice = false;
    public bool IsTarget;
    [SerializeField] GameObject damageEffectPrefab;

    protected override void Start()
    {
        lookAtTarget.target = PlayerController.Instance.Camera.transform;

        if (isPolice) return;
        if (isStopHuman) return;
        if (isUI) return;


        base.Start();

        randomNPC.ApplyRandomAppearance(this);
    }

    void OnEnable()
    {
        if (isPolice) { animator.Play("Run"); return; }
        if (isStopHuman) { animator.Play("Idle"); return; }

        string animName = "Move1";// "Move" + (Random.Range(0, 2) + 1); // Move1 or Move2
        float offset = Random.Range(0f, 1f); // 0〜1の範囲で再生位置をずらす
        animator.Play(animName, 0, offset);

        if (isUI) { animator.speed = 0; return; }
    }
    protected override void Update()
    {
        if (isPolice) return;
        if (isStopHuman) { return; }
        base.Update();

        //animator.SetFloat("Speed", agent.velocity.magnitude / agent.speed);
    }

    public override bool TakeDamage(float damage, Vector3 position, Vector3 forward)
    {
        if (IsDead) return false;

        base.TakeDamage(damage, position, forward);
        SetHitBlood(position, forward);

        if (IsDead)
        {
            Drop(transform.position);
            if (IsTarget)
            {
                SoundManager.Instance.Play("target_drop");
                for (int i = 0; i < 10; i++)
                {
                    Drop(transform.position);
                }
            }

            SetState(false);

            NS.Util.CoroutineRunner.Instance.WaitRun(() =>
            {
                if (isStopHuman)
                {
                    SetState(true);
                }
                else
                {
                    if (gameObject != null && this != null)
                        Destroy(gameObject);
                }
            }, 3);
        }
        else
        {

            animator.Play("Fall");
            IsDamageStop = true;

            NS.Util.CoroutineRunner.Instance.WaitRun(() =>
            {
                if (IsDead) return;
                IsDamageStop = false;
            }, 5);
        }
        return true;
    }

    public static void Drop(Vector3 pos)
    {
        var cash = Instantiate(References.Instance.GetSceneObject("Cash"));
        var mapCash = cash.GetComponent<MapCash>();
        mapCash.WaitGetTime = 1;
        mapCash.isRepop = false;
        cash.transform.position = pos + new Vector3(Random.Range(-1f, 1f), 0, Random.Range(-1f, 1f));
    }

    private void SetState(bool flg)
    {
        var collider = GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = flg;
        }
        lookAtTarget.enabled = flg;

        ragdollState.SetRagdoll(!flg);
        agent.isStopped = !flg;
    }

    public void SetHitBlood(Vector3 pos, Vector3 forward)
    {
        var HitBlood = Instantiate(damageEffectPrefab);
        HitBlood.transform.position = pos;
        HitBlood.transform.forward = -forward;
    }
}

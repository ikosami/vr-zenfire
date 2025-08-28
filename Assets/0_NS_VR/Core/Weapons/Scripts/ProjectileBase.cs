using UnityEngine;

/// <summary>
/// Base class for all projectiles in the game
/// Handles common projectile behavior like movement and collision
/// </summary>
public abstract class ProjectileBase : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] protected float speed = 1000f;
    [SerializeField] protected string impactEffectKey = "effect_hit";
    [SerializeField] protected string impactSoundKey = "hit_projectile";
    [SerializeField] protected LayerMask collisionLayers;

    protected Rigidbody rb;

    protected virtual void Awake()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            rb = gameObject.AddComponent<Rigidbody>();
        }
    }

    protected virtual void Start()
    {
        // Apply initial force
        rb.AddForce(transform.forward * speed);
    }

    protected virtual void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & collisionLayers) != 0)
        {
            HandleImpact(collision);
        }
    }

    /// <summary>
    /// Handle what happens when the projectile hits something
    /// </summary>
    protected virtual void HandleImpact(Collision collision)
    {
        // Play impact effect
        var particle = References.Instance.GetSceneParticle(impactEffectKey);
        if (particle != null)
        {
            var particleObj = Instantiate(particle, collision.contacts[0].point, Quaternion.identity);
            particleObj.Play();
        }

        // Play impact sound
        SoundManager.Instance.Play3D(impactSoundKey, collision.contacts[0].point);

        // Destroy the projectile
        Destroy(gameObject);
    }
}

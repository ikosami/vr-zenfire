using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Base class for destructible objects in VR
/// Handles destruction effects, sounds, and physics
/// </summary>
public class DestructibleObject : MonoBehaviour
{
    [Header("Destruction Settings")]
    [SerializeField] protected float destructionForce = 10f;
    [SerializeField] protected float destructionRadius = 2f;
    [SerializeField] protected bool useGravityOnDestruction = true;
    
    [Header("Effects")]
    [SerializeField] protected string destructionEffectKey = "effect_destruction";
    [SerializeField] protected string destructionSoundKey = "hit_destruction";
    [SerializeField] protected ParticleSystem destructionParticles;
    
    [Header("Fragment Settings")]
    [SerializeField] protected GameObject fragmentPrefab;
    [SerializeField] protected int minFragments = 3;
    [SerializeField] protected int maxFragments = 7;
    
    protected List<Rigidbody> fragments = new List<Rigidbody>();
    protected bool isDestroyed = false;

    protected virtual void Start()
    {
        // Initialize any required components
        if (destructionParticles == null)
        {
            var particleObj = new GameObject("DestructionParticles");
            particleObj.transform.parent = transform;
            particleObj.transform.localPosition = Vector3.zero;
            destructionParticles = particleObj.AddComponent<ParticleSystem>();
        }
    }

    /// <summary>
    /// Trigger the destruction of this object
    /// </summary>
    public virtual void TriggerDestruction()
    {
        if (isDestroyed) return;
        isDestroyed = true;

        // Play destruction effects
        PlayDestructionEffects();

        // Create fragments
        CreateFragments();

        // Apply forces to fragments
        ApplyFragmentForces();

        // Clean up original object
        Destroy(gameObject);
    }

    /// <summary>
    /// Play particle effects and sound for destruction
    /// </summary>
    protected virtual void PlayDestructionEffects()
    {
        // Play particle effect
        if (destructionParticles != null)
        {
            destructionParticles.transform.parent = null;
            destructionParticles.Play();
            Destroy(destructionParticles.gameObject, destructionParticles.main.duration);
        }

        // Play destruction sound
        SoundManager.Instance.Play3D(destructionSoundKey, transform.position, 1f);

        // Get and play additional particle effect if specified
        var additionalParticle = References.Instance.GetSceneParticle(destructionEffectKey);
        if (additionalParticle != null)
        {
            var particleObj = Instantiate(additionalParticle, transform.position, Quaternion.identity);
            particleObj.Play();
        }
    }

    /// <summary>
    /// Create physical fragments of the destroyed object
    /// </summary>
    protected virtual void CreateFragments()
    {
        if (fragmentPrefab == null) return;

        int fragmentCount = Random.Range(minFragments, maxFragments + 1);
        for (int i = 0; i < fragmentCount; i++)
        {
            // Create fragment at random position within object bounds
            var bounds = GetComponent<Collider>().bounds;
            var randomPos = new Vector3(
                Random.Range(bounds.min.x, bounds.max.x),
                Random.Range(bounds.min.y, bounds.max.y),
                Random.Range(bounds.min.z, bounds.max.z)
            );

            var fragment = Instantiate(fragmentPrefab, randomPos, Random.rotation);
            var rb = fragment.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.useGravity = useGravityOnDestruction;
                fragments.Add(rb);
            }
        }
    }

    /// <summary>
    /// Apply explosion-like forces to the fragments
    /// </summary>
    protected virtual void ApplyFragmentForces()
    {
        foreach (var rb in fragments)
        {
            if (rb != null)
            {
                var randomDir = (rb.transform.position - transform.position).normalized;
                rb.AddForce(randomDir * destructionForce, ForceMode.Impulse);
                rb.AddTorque(Random.insideUnitSphere * destructionForce, ForceMode.Impulse);
            }
        }
    }

    /// <summary>
    /// Apply damage to the object, optionally triggering destruction
    /// </summary>
    public virtual void ApplyDamage(float damage, Vector3 hitPoint)
    {
        TriggerDestruction();
    }

    protected virtual void OnDrawGizmosSelected()
    {
        // Visualize destruction radius in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, destructionRadius);
    }
}

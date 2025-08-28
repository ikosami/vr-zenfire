using UnityEngine;

/// <summary>
/// Rocket projectile with explosion effect
/// </summary>
public class Rocket : ProjectileBase
{
    [Header("Explosion Settings")]
    [SerializeField] private float explosionRadius = 5f;
    [SerializeField] private float explosionForce = 1000f;
    [SerializeField] private LayerMask explosionLayers;

    protected override void HandleImpact(Collision collision)
    {
        // Create explosion force
        Collider[] colliders = Physics.OverlapSphere(transform.position, explosionRadius, explosionLayers);
        foreach (var collider in colliders)
        {
            var rb = collider.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.AddExplosionForce(explosionForce, transform.position, explosionRadius);
            }

            // If object is destructible, trigger destruction
            var destructible = collider.GetComponent<DestructibleObject>();
            if (destructible != null)
            {
                destructible.TriggerDestruction();
            }
        }

        // Call base implementation for effects and sound
        base.HandleImpact(collision);
    }

    private void OnDrawGizmosSelected()
    {
        // Draw explosion radius in editor
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
}

using UnityEngine;

/// <summary>
/// A simple destructible crate implementation
/// Breaks into fragments when destroyed
/// </summary>
public class DestructibleCrate : DestructibleObject
{
    [Header("Crate Settings")]
    [SerializeField] private float health = 100f;
    [SerializeField] private Material fragmentMaterial;

    protected override void Start()
    {
        base.Start();

        // If no fragment prefab is set, create a simple cube fragment
        if (fragmentPrefab == null)
        {
            var fragment = GameObject.CreatePrimitive(PrimitiveType.Cube);
            fragment.transform.localScale = Vector3.one * 0.3f;
            
            // Add rigidbody
            var rb = fragment.AddComponent<Rigidbody>();
            rb.mass = 1f;
            rb.drag = 0.5f;
            rb.angularDrag = 0.5f;

            // Set material if provided
            if (fragmentMaterial != null)
            {
                fragment.GetComponent<Renderer>().material = fragmentMaterial;
            }

            // Store as prefab
            fragmentPrefab = fragment;
            fragment.SetActive(false);
        }
    }

    public override void ApplyDamage(float damage, Vector3 hitPoint)
    {
        health -= damage;
        
        // Play hit effect
        var particle = References.Instance.GetSceneParticle("effect_hit_wood");
        if (particle != null)
        {
            var particleObj = Instantiate(particle, hitPoint, Quaternion.identity);
            particleObj.Play();
        }

        // Play hit sound
        SoundManager.Instance.Play3D("hit_wood", hitPoint, 1f);

        if (health <= 0)
        {
            TriggerDestruction();
        }
    }
}

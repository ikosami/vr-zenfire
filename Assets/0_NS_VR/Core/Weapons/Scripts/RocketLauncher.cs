using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Rocket launcher weapon that fires explosive projectiles
/// </summary>
public class RocketLauncher : WeaponBase
{
    [Header("Rocket Settings")]
    [SerializeField] private GameObject rocketPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float launchForce = 1000f;
    [SerializeField] private string launchSoundKey = "fire_rocket";

    protected override void Shoot_()
    {
        if (rocketPrefab == null || spawnPoint == null) return;

        // Spawn rocket
        var rocket = Instantiate(rocketPrefab, spawnPoint.position, spawnPoint.rotation);
        
        // Play launch sound
        SoundManager.Instance.Play3D(launchSoundKey, spawnPoint.position, 1f);
        
        // Apply initial force
        var rb = rocket.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.AddForce(spawnPoint.forward * launchForce);
        }
    }
}

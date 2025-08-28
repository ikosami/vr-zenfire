using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Abstract base class for all VR weapons
/// Provides common functionality like cooldown and input handling
/// </summary>
public abstract class WeaponBase : MonoBehaviour
{
    [Header("Weapon Settings")]
    [SerializeField] protected float cooldownDuration = 0.4f;
    [SerializeField] protected string hapticEffect = "click";
    [SerializeField] protected ControllerSide controllerSide = ControllerSide.Right;

    protected float cooldownTimer = 0f;

    /// <summary>
    /// Abstract method that each weapon must implement for its shooting behavior
    /// </summary>
    protected abstract void Shoot_();

    /// <summary>
    /// Called by the input system when the shoot action is triggered
    /// </summary>
    public void Shoot(InputAction.CallbackContext context)
    {
        if (cooldownTimer > 0) return;
        
        Shoot_();
        cooldownTimer = cooldownDuration;
        
        // Trigger haptic feedback
        VibrationController.Instance.Play(hapticEffect, controllerSide);
    }

    protected virtual void Update()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    /// <summary>
    /// Check if the weapon can currently shoot
    /// </summary>
    public bool CanShoot()
    {
        return cooldownTimer <= 0;
    }
}

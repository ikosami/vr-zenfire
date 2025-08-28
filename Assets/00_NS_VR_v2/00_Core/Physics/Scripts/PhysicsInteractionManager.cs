using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages physics-based interactions in VR
/// Handles force application, constraints, and collision events
/// </summary>
public class PhysicsInteractionManager : MonoBehaviour
{
    private static PhysicsInteractionManager _instance;
    public static PhysicsInteractionManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PhysicsInteractionManager>();
                if (_instance == null)
                {
                    var go = new GameObject("PhysicsInteractionManager");
                    _instance = go.AddComponent<PhysicsInteractionManager>();
                }
            }
            return _instance;
        }
    }

    [Header("Physics Settings")]
    [SerializeField] private float _defaultForce = 10f;
    [SerializeField] private float _maxForce = 100f;
    [SerializeField] private LayerMask _interactableLayers;

    private Dictionary<Collider, RagdollState> _ragdollCache = new Dictionary<Collider, RagdollState>();
    private List<ConfigurableJoint> _activeJoints = new List<ConfigurableJoint>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    /// <summary>
    /// Apply force to a rigidbody at a specific point
    /// </summary>
    public void ApplyForceAtPosition(Rigidbody target, Vector3 force, Vector3 position, ForceMode mode = ForceMode.Impulse)
    {
        if (target == null) return;
        
        // Clamp force magnitude
        force = Vector3.ClampMagnitude(force, _maxForce);
        target.AddForceAtPosition(force, position, mode);

        // Check for ragdoll state
        var ragdoll = GetRagdollState(target.gameObject);
        if (ragdoll != null && !ragdoll.IsRagdoll)
        {
            ragdoll.SetRagdoll(true);
            ragdoll.ApplyForce(force, mode);
        }
    }

    /// <summary>
    /// Create a physics joint between two objects
    /// </summary>
    public ConfigurableJoint CreateJoint(Rigidbody source, Rigidbody target, Vector3 anchor)
    {
        if (source == null || target == null) return null;

        var joint = source.gameObject.AddComponent<ConfigurableJoint>();
        joint.connectedBody = target;
        joint.anchor = source.transform.InverseTransformPoint(anchor);
        
        ConfigureJoint(joint);
        _activeJoints.Add(joint);
        
        return joint;
    }

    /// <summary>
    /// Configure joint properties for VR interaction
    /// </summary>
    private void ConfigureJoint(ConfigurableJoint joint)
    {
        // Configure motion
        joint.xMotion = ConfigurableJointMotion.Limited;
        joint.yMotion = ConfigurableJointMotion.Limited;
        joint.zMotion = ConfigurableJointMotion.Limited;
        
        // Configure rotation
        joint.angularXMotion = ConfigurableJointMotion.Limited;
        joint.angularYMotion = ConfigurableJointMotion.Limited;
        joint.angularZMotion = ConfigurableJointMotion.Limited;
        
        // Configure limits
        var limit = joint.linearLimit;
        limit.limit = 0.2f;
        joint.linearLimit = limit;

        // Configure spring
        var drive = joint.xDrive;
        drive.positionSpring = 1000f;
        drive.positionDamper = 100f;
        joint.xDrive = drive;
        joint.yDrive = drive;
        joint.zDrive = drive;
    }

    /// <summary>
    /// Remove a physics joint
    /// </summary>
    public void RemoveJoint(ConfigurableJoint joint)
    {
        if (joint == null) return;
        
        _activeJoints.Remove(joint);
        Destroy(joint);
    }

    /// <summary>
    /// Get RagdollState component from cache or find in hierarchy
    /// </summary>
    private RagdollState GetRagdollState(GameObject obj)
    {
        if (obj == null) return null;

        var collider = obj.GetComponent<Collider>();
        if (collider != null && _ragdollCache.TryGetValue(collider, out var cachedRagdoll))
        {
            return cachedRagdoll;
        }

        var ragdoll = obj.GetComponentInParent<RagdollState>();
        if (ragdoll != null && collider != null)
        {
            _ragdollCache[collider] = ragdoll;
        }
        
        return ragdoll;
    }

    /// <summary>
    /// Clear cached references
    /// </summary>
    public void ClearCache()
    {
        _ragdollCache.Clear();
        _activeJoints.Clear();
    }

    private void OnDestroy()
    {
        foreach (var joint in _activeJoints)
        {
            if (joint != null)
            {
                Destroy(joint);
            }
        }
        _activeJoints.Clear();
    }
}

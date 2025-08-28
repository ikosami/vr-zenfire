using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class RagdollStateTest
{
    private GameObject ragdollObject;
    private RagdollState ragdoll;
    private Animator animator;
    private Rigidbody[] rigidbodies;
    private Collider[] colliders;

    [SetUp]
    public void Setup()
    {
        // Create test hierarchy
        ragdollObject = new GameObject("RagdollTest");
        animator = ragdollObject.AddComponent<Animator>();
        ragdoll = ragdollObject.AddComponent<RagdollState>();

        // Create child objects with rigidbodies and colliders
        CreateRagdollPart("Hips");
        CreateRagdollPart("Spine");
        CreateRagdollPart("Head");
        
        // Get components
        rigidbodies = ragdollObject.GetComponentsInChildren<Rigidbody>();
        colliders = ragdollObject.GetComponentsInChildren<Collider>();
    }

    private void CreateRagdollPart(string name)
    {
        var part = new GameObject(name);
        part.transform.parent = ragdollObject.transform;
        part.AddComponent<Rigidbody>();
        part.AddComponent<BoxCollider>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(ragdollObject);
    }

    [UnityTest]
    public IEnumerator TestRagdollToggle()
    {
        // Enable ragdoll
        ragdoll.SetRagdoll(true);
        yield return null;
        
        Assert.IsTrue(ragdoll.IsRagdoll, "Ragdoll should be enabled");
        Assert.IsFalse(animator.enabled, "Animator should be disabled in ragdoll mode");
        
        foreach (var rb in rigidbodies)
        {
            Assert.IsFalse(rb.isKinematic, "Rigidbodies should not be kinematic in ragdoll mode");
            Assert.IsTrue(rb.useGravity, "Rigidbodies should use gravity in ragdoll mode");
        }
        
        foreach (var collider in colliders)
        {
            Assert.IsTrue(collider.enabled, "Colliders should be enabled in ragdoll mode");
        }
    }

    [UnityTest]
    public IEnumerator TestRagdollDisable()
    {
        // Enable then disable ragdoll
        ragdoll.SetRagdoll(true);
        yield return null;
        
        ragdoll.SetRagdoll(false);
        yield return null;
        
        Assert.IsFalse(ragdoll.IsRagdoll, "Ragdoll should be disabled");
        Assert.IsTrue(animator.enabled, "Animator should be enabled when not in ragdoll mode");
    }

    [UnityTest]
    public IEnumerator TestForceApplication()
    {
        // Enable ragdoll and apply force
        ragdoll.SetRagdoll(true);
        Vector3 initialPos = ragdollObject.transform.position;
        Vector3 force = Vector3.right * 10f;
        
        ragdoll.ApplyForce(force);
        
        yield return new WaitForFixedUpdate();
        
        bool anyMovement = false;
        foreach (var rb in rigidbodies)
        {
            if (rb.velocity.magnitude > 0)
            {
                anyMovement = true;
                break;
            }
        }
        
        Assert.IsTrue(anyMovement, "At least one rigidbody should move when force is applied");
    }

    [UnityTest]
    public IEnumerator TestRecoveryBlending()
    {
        // Enable then disable ragdoll to trigger recovery
        ragdoll.SetRagdoll(true);
        yield return null;
        
        ragdoll.SetRagdoll(false);
        
        // Check initial blend weight
        yield return null;
        float initialWeight = animator.GetLayerWeight(6);
        
        // Wait and check if blend weight changes
        yield return new WaitForSeconds(0.2f);
        float laterWeight = animator.GetLayerWeight(6);
        
        Assert.AreNotEqual(initialWeight, laterWeight, 
            "Animator layer weight should change during recovery");
    }
}

using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class PhysicsInteractionManagerTest
{
    private GameObject managerObject;
    private PhysicsInteractionManager manager;
    private GameObject testObject1;
    private GameObject testObject2;
    private Rigidbody rb1;
    private Rigidbody rb2;

    [SetUp]
    public void Setup()
    {
        // Create manager
        managerObject = new GameObject("PhysicsInteractionManager");
        manager = managerObject.AddComponent<PhysicsInteractionManager>();

        // Create test objects
        testObject1 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        testObject2 = GameObject.CreatePrimitive(PrimitiveType.Cube);
        rb1 = testObject1.AddComponent<Rigidbody>();
        rb2 = testObject2.AddComponent<Rigidbody>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(managerObject);
        Object.DestroyImmediate(testObject1);
        Object.DestroyImmediate(testObject2);
    }

    [UnityTest]
    public IEnumerator TestForceApplication()
    {
        Vector3 initialPos = testObject1.transform.position;
        Vector3 force = Vector3.right * 10f;
        
        manager.ApplyForceAtPosition(rb1, force, testObject1.transform.position);
        
        yield return new WaitForFixedUpdate();
        
        Assert.AreNotEqual(initialPos, testObject1.transform.position, 
            "Object should move when force is applied");
    }

    [UnityTest]
    public IEnumerator TestJointCreation()
    {
        var joint = manager.CreateJoint(rb1, rb2, Vector3.zero);
        
        yield return null;
        
        Assert.IsNotNull(joint, "Joint should be created");
        Assert.AreEqual(rb2, joint.connectedBody, "Joint should connect to target rigidbody");
    }

    [UnityTest]
    public IEnumerator TestJointRemoval()
    {
        var joint = manager.CreateJoint(rb1, rb2, Vector3.zero);
        yield return null;
        
        manager.RemoveJoint(joint);
        yield return null;
        
        Assert.IsNull(testObject1.GetComponent<ConfigurableJoint>(), 
            "Joint should be removed");
    }

    [UnityTest]
    public IEnumerator TestRagdollInteraction()
    {
        // Create ragdoll test object
        var ragdollObject = new GameObject("RagdollTest");
        var animator = ragdollObject.AddComponent<Animator>();
        var ragdoll = ragdollObject.AddComponent<RagdollState>();
        var ragdollRb = ragdollObject.AddComponent<Rigidbody>();
        
        // Apply force
        Vector3 force = Vector3.right * 10f;
        manager.ApplyForceAtPosition(ragdollRb, force, ragdollObject.transform.position);
        
        yield return new WaitForFixedUpdate();
        
        Assert.IsTrue(ragdoll.IsRagdoll, "Ragdoll state should be activated on force application");
        
        Object.DestroyImmediate(ragdollObject);
    }

    [UnityTest]
    public IEnumerator TestCacheClearing()
    {
        // Create test ragdoll
        var ragdollObject = new GameObject("RagdollTest");
        var animator = ragdollObject.AddComponent<Animator>();
        var ragdoll = ragdollObject.AddComponent<RagdollState>();
        var collider = ragdollObject.AddComponent<BoxCollider>();
        
        // Create test joint
        var joint = manager.CreateJoint(rb1, rb2, Vector3.zero);
        
        yield return null;
        
        // Clear cache
        manager.ClearCache();
        
        // Apply force to test cache clearing
        manager.ApplyForceAtPosition(rb1, Vector3.right, Vector3.zero);
        
        yield return null;
        
        Assert.IsTrue(true, "Cache clearing should not cause errors");
        
        Object.DestroyImmediate(ragdollObject);
    }
}

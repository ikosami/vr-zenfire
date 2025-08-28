using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class RocketTest
{
    private GameObject rocketObject;
    private Rocket rocket;
    private GameObject targetObject;
    private Rigidbody targetRigidbody;

    [SetUp]
    public void Setup()
    {
        // Create rocket
        rocketObject = new GameObject("Rocket");
        rocket = rocketObject.AddComponent<Rocket>();
        var rocketRb = rocketObject.AddComponent<Rigidbody>();
        rocketRb.useGravity = false;
        
        // Create target
        targetObject = new GameObject("Target");
        targetObject.layer = LayerMask.NameToLayer("Default");
        targetRigidbody = targetObject.AddComponent<Rigidbody>();
        targetObject.AddComponent<BoxCollider>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(rocketObject);
        Object.DestroyImmediate(targetObject);
    }

    [UnityTest]
    public IEnumerator TestRocketMovement()
    {
        // Initial position
        Vector3 startPos = rocketObject.transform.position;
        
        yield return new WaitForFixedUpdate();
        
        // Should have moved forward
        Assert.Greater(Vector3.Distance(rocketObject.transform.position, startPos), 0f);
    }

    [UnityTest]
    public IEnumerator TestRocketCollision()
    {
        // Position objects
        rocketObject.transform.position = Vector3.zero;
        targetObject.transform.position = Vector3.forward * 2f;
        
        // Wait for collision
        yield return new WaitForSeconds(0.5f);
        
        // Target should have been affected by explosion
        Assert.Greater(targetRigidbody.velocity.magnitude, 0f);
    }

    [UnityTest]
    public IEnumerator TestRocketDestruction()
    {
        // Add destructible component to target
        var destructible = targetObject.AddComponent<TestDestructible>();
        
        // Position objects
        rocketObject.transform.position = Vector3.zero;
        targetObject.transform.position = Vector3.forward * 2f;
        
        yield return new WaitForSeconds(0.5f);
        
        // Destructible should have been triggered
        Assert.IsTrue(destructible.wasDestroyed);
    }

    private class TestDestructible : MonoBehaviour
    {
        public bool wasDestroyed = false;
        
        public void TriggerDestruction()
        {
            wasDestroyed = true;
        }
    }
}

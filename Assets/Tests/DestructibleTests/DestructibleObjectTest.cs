using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class DestructibleObjectTest
{
    private GameObject destructibleObject;
    private TestDestructibleObject destructible;
    private GameObject fragmentPrefab;

    private class TestDestructibleObject : DestructibleObject
    {
        public bool effectsPlayed = false;
        public bool fragmentsCreated = false;
        public bool forcesApplied = false;

        protected override void PlayDestructionEffects()
        {
            effectsPlayed = true;
        }

        protected override void CreateFragments()
        {
            fragmentsCreated = true;
        }

        protected override void ApplyFragmentForces()
        {
            forcesApplied = true;
        }
    }

    [SetUp]
    public void Setup()
    {
        // Create test objects
        destructibleObject = new GameObject("DestructibleObject");
        destructible = destructibleObject.AddComponent<TestDestructibleObject>();
        destructibleObject.AddComponent<BoxCollider>();

        // Create fragment prefab
        fragmentPrefab = new GameObject("FragmentPrefab");
        fragmentPrefab.AddComponent<Rigidbody>();
        var field = typeof(DestructibleObject).GetField("fragmentPrefab", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(destructible, fragmentPrefab);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(destructibleObject);
        Object.DestroyImmediate(fragmentPrefab);
    }

    [UnityTest]
    public IEnumerator TestDestruction()
    {
        destructible.TriggerDestruction();
        
        yield return null;
        
        Assert.IsTrue(destructible.effectsPlayed, "Destruction effects should be played");
        Assert.IsTrue(destructible.fragmentsCreated, "Fragments should be created");
        Assert.IsTrue(destructible.forcesApplied, "Forces should be applied to fragments");
    }

    [UnityTest]
    public IEnumerator TestMultipleDestructionAttempts()
    {
        destructible.TriggerDestruction();
        bool firstDestruction = destructible.effectsPlayed;
        
        // Reset flags
        destructible.effectsPlayed = false;
        destructible.fragmentsCreated = false;
        destructible.forcesApplied = false;
        
        // Try to destroy again
        destructible.TriggerDestruction();
        
        yield return null;
        
        Assert.IsTrue(firstDestruction, "First destruction should succeed");
        Assert.IsFalse(destructible.effectsPlayed, "Second destruction should not trigger effects");
        Assert.IsFalse(destructible.fragmentsCreated, "Second destruction should not create fragments");
        Assert.IsFalse(destructible.forcesApplied, "Second destruction should not apply forces");
    }

    [UnityTest]
    public IEnumerator TestDamageApplication()
    {
        Vector3 hitPoint = Vector3.right;
        destructible.ApplyDamage(10f, hitPoint);
        
        yield return null;
        
        Assert.IsTrue(destructible.effectsPlayed, "Damage should trigger destruction");
    }
}

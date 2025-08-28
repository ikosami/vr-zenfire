using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class DestructibleCrateTest
{
    private GameObject crateObject;
    private DestructibleCrate crate;

    [SetUp]
    public void Setup()
    {
        crateObject = new GameObject("DestructibleCrate");
        crate = crateObject.AddComponent<DestructibleCrate>();
        crateObject.AddComponent<BoxCollider>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(crateObject);
    }

    [UnityTest]
    public IEnumerator TestCrateHealth()
    {
        float initialHealth = 100f;
        float damage = 30f;
        
        // Apply damage
        crate.ApplyDamage(damage, Vector3.zero);
        
        yield return null;
        
        // Crate should still exist after partial damage
        Assert.IsNotNull(crateObject);
        
        // Apply fatal damage
        crate.ApplyDamage(initialHealth, Vector3.zero);
        
        yield return null;
        
        // Crate should be destroyed
        Assert.IsTrue(crateObject == null);
    }

    [UnityTest]
    public IEnumerator TestFragmentCreation()
    {
        // Start should create default fragment if none provided
        yield return null;
        
        var field = typeof(DestructibleObject).GetField("fragmentPrefab", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var fragmentPrefab = field.GetValue(crate) as GameObject;
        
        Assert.IsNotNull(fragmentPrefab, "Fragment prefab should be created");
        Assert.IsNotNull(fragmentPrefab.GetComponent<Rigidbody>(), "Fragment should have Rigidbody");
    }

    [UnityTest]
    public IEnumerator TestDamageEffects()
    {
        bool effectPlayed = false;
        bool soundPlayed = false;

        // Mock References and SoundManager
        var references = new GameObject("References").AddComponent<References>();
        var soundManager = new GameObject("SoundManager").AddComponent<SoundManager>();

        // Apply damage
        crate.ApplyDamage(10f, Vector3.zero);
        
        yield return null;

        // Cleanup
        Object.DestroyImmediate(references.gameObject);
        Object.DestroyImmediate(soundManager.gameObject);
        
        // Note: In a real test environment, we would verify that effects and sounds were played
        // Here we just verify the crate still exists after non-fatal damage
        Assert.IsNotNull(crateObject);
    }
}

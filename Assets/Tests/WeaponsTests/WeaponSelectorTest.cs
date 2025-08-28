using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class WeaponSelectorTest
{
    private GameObject selectorObject;
    private WeaponSelector weaponSelector;
    private GameObject weapon1Object;
    private GameObject weapon2Object;
    private WeaponBase weapon1;
    private WeaponBase weapon2;

    [SetUp]
    public void Setup()
    {
        // Create test weapons
        weapon1Object = new GameObject("Weapon1");
        weapon2Object = new GameObject("Weapon2");
        weapon1 = weapon1Object.AddComponent<TestWeapon>();
        weapon2 = weapon2Object.AddComponent<TestWeapon>();

        // Create weapon selector
        selectorObject = new GameObject("WeaponSelector");
        weaponSelector = selectorObject.AddComponent<WeaponSelector>();
        
        // Setup weapon data
        var weaponData = new WeaponSelector.WeaponData[2];
        weaponData[0] = new WeaponSelector.WeaponData { Key = "weapon1", Weapon = weapon1 };
        weaponData[1] = new WeaponSelector.WeaponData { Key = "weapon2", Weapon = weapon2 };
        
        // Use reflection to set private field
        var field = typeof(WeaponSelector).GetField("weaponDatas", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(weaponSelector, weaponData);
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(selectorObject);
        Object.DestroyImmediate(weapon1Object);
        Object.DestroyImmediate(weapon2Object);
    }

    [UnityTest]
    public IEnumerator TestWeaponSelection()
    {
        // Initial state
        Assert.IsFalse(weapon1Object.activeSelf, "Weapon1 should be inactive initially");
        Assert.IsFalse(weapon2Object.activeSelf, "Weapon2 should be inactive initially");

        // Select weapon1
        weaponSelector.SelectWeapon("weapon1");
        yield return null;
        
        Assert.IsTrue(weapon1Object.activeSelf, "Weapon1 should be active");
        Assert.IsFalse(weapon2Object.activeSelf, "Weapon2 should be inactive");

        // Switch to weapon2
        weaponSelector.SelectWeapon("weapon2");
        yield return null;
        
        Assert.IsFalse(weapon1Object.activeSelf, "Weapon1 should be inactive");
        Assert.IsTrue(weapon2Object.activeSelf, "Weapon2 should be active");
    }

    [UnityTest]
    public IEnumerator TestInvalidWeaponSelection()
    {
        weaponSelector.SelectWeapon("invalid_weapon");
        yield return null;

        Assert.IsFalse(weapon1Object.activeSelf, "Weapon1 should remain inactive");
        Assert.IsFalse(weapon2Object.activeSelf, "Weapon2 should remain inactive");
    }

    private class TestWeapon : WeaponBase
    {
        protected override void Shoot_()
        {
            // Test implementation
        }
    }
}

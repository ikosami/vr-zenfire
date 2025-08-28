using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.InputSystem;

public class WeaponBaseTest
{
    private GameObject weaponObject;
    private TestWeapon testWeapon;

    private class TestWeapon : WeaponBase
    {
        public bool shotFired = false;

        protected override void Shoot_()
        {
            shotFired = true;
        }
    }

    [SetUp]
    public void Setup()
    {
        weaponObject = new GameObject("TestWeapon");
        testWeapon = weaponObject.AddComponent<TestWeapon>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(weaponObject);
    }

    [UnityTest]
    public IEnumerator TestWeaponCooldown()
    {
        Assert.IsTrue(testWeapon.CanShoot(), "Weapon should be able to shoot initially");

        // Simulate shooting
        testWeapon.Shoot(new InputAction.CallbackContext());
        
        Assert.IsTrue(testWeapon.shotFired, "Weapon should have fired");
        Assert.IsFalse(testWeapon.CanShoot(), "Weapon should be in cooldown");

        // Wait for cooldown
        yield return new WaitForSeconds(0.5f);
        
        Assert.IsTrue(testWeapon.CanShoot(), "Weapon should be ready to shoot after cooldown");
    }

    [UnityTest]
    public IEnumerator TestRapidFire()
    {
        // Try to shoot multiple times rapidly
        testWeapon.Shoot(new InputAction.CallbackContext());
        bool firstShot = testWeapon.shotFired;
        testWeapon.shotFired = false;

        // Try to shoot again immediately
        testWeapon.Shoot(new InputAction.CallbackContext());
        bool secondShot = testWeapon.shotFired;

        yield return null;

        Assert.IsTrue(firstShot, "First shot should succeed");
        Assert.IsFalse(secondShot, "Second shot should fail due to cooldown");
    }
}

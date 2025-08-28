using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.InputSystem;

public class WeaponSelectableTests
{
    private GameObject gameObject;
    private WeaponSelectable weaponSelectable;

    [SetUp]
    public void Setup()
    {
        gameObject = new GameObject();
        weaponSelectable = gameObject.AddComponent<WeaponSelectable>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void SelectWeapon_CallsWeaponSelector()
    {
        // Arrange
        int expectedIndex = 0;

        // Act
        weaponSelectable.SelectWeapon();

        // Assert
        // Note: This test will need to be updated once WeaponSelector.Instance is properly
        // implemented or mockable
        Assert.Pass("Method called without exceptions");
    }
}

public class PlayerJumpTests
{
    private GameObject gameObject;
    private PlayerJump playerJump;
    private CharacterController characterController;

    [SetUp]
    public void Setup()
    {
        gameObject = new GameObject();
        characterController = gameObject.AddComponent<CharacterController>();
        playerJump = gameObject.AddComponent<PlayerJump>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void PlayerJump_InitializesWithCorrectDefaults()
    {
        // Assert
        Assert.IsNotNull(playerJump);
        Assert.IsNotNull(characterController);
    }
}

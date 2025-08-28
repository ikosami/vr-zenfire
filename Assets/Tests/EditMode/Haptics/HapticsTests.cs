using NUnit.Framework;
using UnityEngine;
using Oculus.Haptics;

public class VibrationControllerTests
{
    private GameObject gameObject;
    private VibrationController vibrationController;
    private HapticReferencesScriptable references;

    [SetUp]
    public void Setup()
    {
        gameObject = new GameObject();
        vibrationController = gameObject.AddComponent<VibrationController>();
        references = ScriptableObject.CreateInstance<HapticReferencesScriptable>();
        vibrationController._references = references;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(gameObject);
        Object.DestroyImmediate(references);
    }

    [Test]
    public void Awake_InitializesHapticPlayer()
    {
        // Act
        vibrationController.Awake();

        // Assert
        Assert.IsNotNull(VibrationController.instance);
        Assert.AreEqual(vibrationController, VibrationController.instance);
    }

    [Test]
    public void Play_WithLeftController_DoesNotThrowException()
    {
        // Arrange
        vibrationController.Awake();

        // Act & Assert
        Assert.DoesNotThrow(() => {
            vibrationController.Play("test", ControllerSide.Left);
        });
    }

    [Test]
    public void Play_WithRightController_DoesNotThrowException()
    {
        // Arrange
        vibrationController.Awake();

        // Act & Assert
        Assert.DoesNotThrow(() => {
            vibrationController.Play("test", ControllerSide.Right);
        });
    }

    [Test]
    public void Play_WithBothControllers_DoesNotThrowException()
    {
        // Arrange
        vibrationController.Awake();

        // Act & Assert
        Assert.DoesNotThrow(() => {
            vibrationController.Play("test", ControllerSide.Both);
        });
    }
}

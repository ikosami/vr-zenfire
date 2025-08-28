using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Oculus.Haptics;

public class VibrationControllerPlayModeTests
{
    private GameObject gameObject;
    private VibrationController vibrationController;
    private HapticReferencesScriptable references;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        gameObject = new GameObject();
        vibrationController = gameObject.AddComponent<VibrationController>();
        references = ScriptableObject.CreateInstance<HapticReferencesScriptable>();
        vibrationController._references = references;
        
        yield return null;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(gameObject);
        Object.DestroyImmediate(references);
    }

    [UnityTest]
    public IEnumerator VibrationController_PlayHapticFeedback_Left()
    {
        // Arrange
        vibrationController.Awake();
        yield return null;

        // Act
        vibrationController.Play("test", ControllerSide.Left);
        yield return new WaitForSeconds(0.1f);

        // Assert
        Assert.Pass("Haptic feedback played without errors");
    }

    [UnityTest]
    public IEnumerator VibrationController_PlayHapticFeedback_Right()
    {
        // Arrange
        vibrationController.Awake();
        yield return null;

        // Act
        vibrationController.Play("test", ControllerSide.Right);
        yield return new WaitForSeconds(0.1f);

        // Assert
        Assert.Pass("Haptic feedback played without errors");
    }

    [UnityTest]
    public IEnumerator VibrationController_PlayHapticFeedback_Both()
    {
        // Arrange
        vibrationController.Awake();
        yield return null;

        // Act
        vibrationController.Play("test", ControllerSide.Both);
        yield return new WaitForSeconds(0.1f);

        // Assert
        Assert.Pass("Haptic feedback played without errors");
    }

    [UnityTest]
    public IEnumerator VibrationController_MultiplePlayCalls_HandlesSequentially()
    {
        // Arrange
        vibrationController.Awake();
        yield return null;

        // Act
        vibrationController.Play("test", ControllerSide.Left);
        yield return new WaitForSeconds(0.05f);
        vibrationController.Play("test", ControllerSide.Right);
        yield return new WaitForSeconds(0.05f);
        vibrationController.Play("test", ControllerSide.Both);
        yield return new WaitForSeconds(0.1f);

        // Assert
        Assert.Pass("Multiple haptic feedbacks played without errors");
    }
}

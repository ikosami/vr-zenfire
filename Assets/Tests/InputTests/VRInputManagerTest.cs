using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using NUnit.Framework;
using System.Collections;

public class VRInputManagerTest
{
    private GameObject _managerObject;
    private VRInputManager _inputManager;

    [SetUp]
    public void Setup()
    {
        _managerObject = new GameObject("VRInputManager");
        _inputManager = _managerObject.AddComponent<VRInputManager>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(_managerObject);
    }

    [UnityTest]
    public IEnumerator TestSingletonInstance()
    {
        yield return null;
        Assert.IsNotNull(VRInputManager.Instance, "VRInputManager instance is null");
        Assert.AreEqual(_inputManager, VRInputManager.Instance, "Singleton instance does not match created instance");
    }

    [UnityTest]
    public IEnumerator TestTriggerThreshold()
    {
        // Test trigger threshold detection
        bool isPressed = _inputManager.IsTriggerPressed(ControllerSide.Left, true, 0.5f);
        Assert.IsFalse(isPressed, "Trigger should not be pressed by default");

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestStickValues()
    {
        // Test default stick values
        Vector2 leftStickValue = _inputManager.LeftStickValue;
        Vector2 rightStickValue = _inputManager.RightStickValue;

        Assert.AreEqual(Vector2.zero, leftStickValue, "Left stick should be zero by default");
        Assert.AreEqual(Vector2.zero, rightStickValue, "Right stick should be zero by default");

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestButtonStates()
    {
        // Test button states
        bool buttonAPressed = _inputManager.ButtonAPressed;
        bool buttonBPressed = _inputManager.ButtonBPressed;

        Assert.IsFalse(buttonAPressed, "Button A should not be pressed by default");
        Assert.IsFalse(buttonBPressed, "Button B should not be pressed by default");

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestControllerPositions()
    {
        // Test controller positions
        Vector3 leftPos = _inputManager.LeftControllerPos;
        Vector3 rightPos = _inputManager.RightControllerPos;

        Assert.AreEqual(Vector3.zero, leftPos, "Left controller position should be zero by default");
        Assert.AreEqual(Vector3.zero, rightPos, "Right controller position should be zero by default");

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestControllerRotations()
    {
        // Test controller rotations
        Quaternion leftRot = _inputManager.LeftControllerRot;
        Quaternion rightRot = _inputManager.RightControllerRot;

        Assert.AreEqual(Quaternion.identity, leftRot, "Left controller rotation should be identity by default");
        Assert.AreEqual(Quaternion.identity, rightRot, "Right controller rotation should be identity by default");

        yield return null;
    }

    [UnityTest]
    public IEnumerator TestTriggerValues()
    {
        // Test trigger values
        float leftIndexTrigger = _inputManager.LeftIndexTriggerValue;
        float leftMiddleTrigger = _inputManager.LeftMiddleTriggerValue;
        float rightIndexTrigger = _inputManager.RightIndexTriggerValue;
        float rightMiddleTrigger = _inputManager.RightMiddleTriggerValue;

        Assert.AreEqual(0f, leftIndexTrigger, "Left index trigger should be zero by default");
        Assert.AreEqual(0f, leftMiddleTrigger, "Left middle trigger should be zero by default");
        Assert.AreEqual(0f, rightIndexTrigger, "Right index trigger should be zero by default");
        Assert.AreEqual(0f, rightMiddleTrigger, "Right middle trigger should be zero by default");

        yield return null;
    }
}

using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;
using NUnit.Framework;
using System.Collections;

public class RotatablePuzzleObjectTest
{
    private GameObject puzzleObject;
    private RotatablePuzzleObject rotatablePuzzle;
    private XRGrabInteractable grabInteractable;

    [SetUp]
    public void Setup()
    {
        // Create puzzle object
        puzzleObject = new GameObject("RotatablePuzzle");
        rotatablePuzzle = puzzleObject.AddComponent<RotatablePuzzleObject>();
        grabInteractable = puzzleObject.AddComponent<XRGrabInteractable>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(puzzleObject);
    }

    [UnityTest]
    public IEnumerator TestRotationSnapping()
    {
        // Initial rotation
        puzzleObject.transform.rotation = Quaternion.identity;
        
        // Simulate grab
        var selectArgs = new SelectEnterEventArgs();
        grabInteractable.selectEntered.Invoke(selectArgs);
        
        yield return null;
        
        // Rotate object
        puzzleObject.transform.rotation = Quaternion.Euler(0, 45f, 0);
        
        yield return null;
        
        // Release grab
        var exitArgs = new SelectExitEventArgs();
        grabInteractable.selectExited.Invoke(exitArgs);
        
        // Wait for snap
        yield return new WaitForSeconds(0.5f);
        
        // Should snap to nearest 90 degrees
        Vector3 finalRotation = puzzleObject.transform.rotation.eulerAngles;
        Assert.AreEqual(0f, finalRotation.y, 1f, "Should snap back to 0 degrees");
    }

    [UnityTest]
    public IEnumerator TestStateTracking()
    {
        float initialRotation = 0f;
        float targetRotation = 90f;
        
        // Simulate grab and rotation
        var selectArgs = new SelectEnterEventArgs();
        grabInteractable.selectEntered.Invoke(selectArgs);
        
        yield return null;
        
        puzzleObject.transform.rotation = Quaternion.Euler(0, targetRotation, 0);
        
        yield return null;
        
        var exitArgs = new SelectExitEventArgs();
        grabInteractable.selectExited.Invoke(exitArgs);
        
        yield return new WaitForSeconds(0.5f);
        
        float currentRotation = rotatablePuzzle.GetStateValue<float>("rotation", 0f);
        Assert.AreEqual(90f, currentRotation, 1f, "Rotation state should be tracked");
    }

    [UnityTest]
    public IEnumerator TestSmoothRotation()
    {
        // Set initial rotation
        puzzleObject.transform.rotation = Quaternion.identity;
        
        // Simulate grab
        var selectArgs = new SelectEnterEventArgs();
        grabInteractable.selectEntered.Invoke(selectArgs);
        
        yield return null;
        
        // Set target rotation
        puzzleObject.transform.rotation = Quaternion.Euler(0, 85f, 0);
        
        // Release grab
        var exitArgs = new SelectExitEventArgs();
        grabInteractable.selectExited.Invoke(exitArgs);
        
        // Check intermediate rotation
        yield return new WaitForSeconds(0.25f);
        
        float intermediateY = puzzleObject.transform.rotation.eulerAngles.y;
        Assert.Greater(intermediateY, 0f, "Should rotate smoothly towards target");
        Assert.Less(intermediateY, 90f, "Should not reach target instantly");
    }
}

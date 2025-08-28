using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.InputSystem;
using System.Collections.Generic;
using Ev;

public class HandPoseManagerPlayModeTests
{
    private GameObject gameObject;
    private HandPoseManager handPoseManager;
    private Transform handRoot;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        gameObject = new GameObject();
        handPoseManager = gameObject.AddComponent<HandPoseManager>();
        
        handRoot = new GameObject("HandRoot").transform;
        handRoot.SetParent(gameObject.transform);
        var finger = new GameObject("Finger").transform;
        finger.SetParent(handRoot);
        
        handPoseManager.HandPoseRoot = handRoot;
        handPoseManager.HandPoseScriptables = new List<HandPoseScriptable>();
        handPoseManager.AnimateDuration = 0.1f;

        yield return null;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(gameObject);
    }

    [UnityTest]
    public IEnumerator HandPoseManager_AnimatesPoseOverTime()
    {
        // Arrange
        var pose = ScriptableObject.CreateInstance<HandPoseScriptable>();
        pose.PoseName = "TestPose";
        pose.Rigs = new List<HandRigData> {
            new HandRigData {
                ObjectName = "HandRoot",
                Position = Vector3.one,
                Rotation = Vector3.zero,
                Scale = Vector3.one
            }
        };
        handPoseManager.HandPoseScriptables.Add(pose);
        handPoseManager.GetTransforms();

        // Act
        handPoseManager.SelectPose("TestPose", isImmediate: false, duration: 0.1f);

        // Wait for animation
        yield return new WaitForSeconds(0.05f);
        
        // Assert mid-animation
        var currentPos = handPoseManager.HandRigs["HandRoot"].localPosition;
        Assert.That(currentPos.x, Is.GreaterThan(0).And.LessThan(1));

        // Wait for animation to complete
        yield return new WaitForSeconds(0.1f);
        
        // Assert final position
        Assert.AreEqual(Vector3.one, handPoseManager.HandRigs["HandRoot"].localPosition);
    }

    [UnityTest]
    public IEnumerator HandPoseManager_RespondsToHandPoseChangeEvent()
    {
        // Arrange
        var pose = ScriptableObject.CreateInstance<HandPoseScriptable>();
        pose.PoseName = "TestPose";
        pose.Rigs = new List<HandRigData> {
            new HandRigData {
                ObjectName = "HandRoot",
                Position = Vector3.one
            }
        };
        handPoseManager.HandPoseScriptables.Add(pose);
        handPoseManager.GetTransforms();
        handPoseManager.Start();

        // Act
        EventManager.TriggerEvent(Events.Name.HAND_POSE_CHANGE, 
            new Events.HandPoseChange { 
                PoseName = "TestPose", 
                ControllerSide = handPoseManager.ControllerSide 
            });

        // Wait for animation
        yield return new WaitForSeconds(0.15f);

        // Assert
        Assert.AreEqual(Vector3.one, handPoseManager.HandRigs["HandRoot"].localPosition);
    }
}

public class ControllerHandPosePlayModeTests
{
    private GameObject gameObject;
    private ControllerHandPose controllerHandPose;
    private InputActionAsset inputAsset;
    private InputActionReference triggerAction;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        gameObject = new GameObject();
        controllerHandPose = gameObject.AddComponent<ControllerHandPose>();

        // Setup input action
        inputAsset = ScriptableObject.CreateInstance<InputActionAsset>();
        var actionMap = inputAsset.AddActionMap("Test");
        var action = actionMap.AddAction("Trigger", InputActionType.Button);
        triggerAction = ScriptableObject.CreateInstance<InputActionReference>();
        var field = typeof(InputActionReference).GetField("m_Action", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        field.SetValue(triggerAction, action);

        var serializedField = typeof(ControllerHandPose).GetField("_triggerAction", 
            System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        serializedField.SetValue(controllerHandPose, triggerAction);

        yield return null;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(gameObject);
        Object.DestroyImmediate(inputAsset);
        Object.DestroyImmediate(triggerAction);
    }

    [UnityTest]
    public IEnumerator ControllerHandPose_TriggerPressFiresEvent()
    {
        // Arrange
        bool eventFired = false;
        EventManager.StartListening(Events.Name.HAND_POSE_CHANGE, (IEvent ev) => {
            if (ev is Events.HandPoseChange) eventFired = true;
        });

        // Enable input
        triggerAction.action.Enable();
        yield return null;

        // Act - Simulate trigger press
        triggerAction.action.Start();
        yield return null;

        // Assert
        Assert.IsTrue(eventFired);
    }
}

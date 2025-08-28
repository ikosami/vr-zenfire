using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using System.Collections.Generic;
using Ev;

public class HandPoseManagerTests
{
    private GameObject gameObject;
    private HandPoseManager handPoseManager;
    private Transform handRoot;

    [SetUp]
    public void Setup()
    {
        gameObject = new GameObject();
        handPoseManager = gameObject.AddComponent<HandPoseManager>();
        
        // Setup hand hierarchy
        handRoot = new GameObject("HandRoot").transform;
        handRoot.SetParent(gameObject.transform);
        var finger = new GameObject("Finger").transform;
        finger.SetParent(handRoot);
        
        handPoseManager.HandPoseRoot = handRoot;
        handPoseManager.HandPoseScriptables = new List<HandPoseScriptable>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void GetTransforms_CollectsAllChildTransforms()
    {
        // Act
        handPoseManager.GetTransforms();

        // Assert
        Assert.IsTrue(handPoseManager.HandRigs.ContainsKey("HandRoot"));
        Assert.IsTrue(handPoseManager.HandRigs.ContainsKey("Finger"));
    }

    [Test]
    public void SelectPose_ImmediatelyAppliesPose()
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
        handPoseManager.SelectPose("TestPose", isImmediate: true);

        // Assert
        Assert.AreEqual(Vector3.one, handPoseManager.HandRigs["HandRoot"].localPosition);
    }
}

public class HandPoseScriptableTests
{
    private HandPoseScriptable handPose;

    [SetUp]
    public void Setup()
    {
        handPose = ScriptableObject.CreateInstance<HandPoseScriptable>();
        handPose.PoseName = "TestPose";
        handPose.ControllerSide = ControllerSide.Left;
        handPose.Prefix = "L_";
        handPose.MirroredPrefix = "R_";
        handPose.Rigs = new List<HandRigData> {
            new HandRigData {
                ObjectName = "L_Hand",
                Position = new Vector3(1, 0, 0),
                Rotation = new Vector3(0, 90, 0),
                Scale = Vector3.one
            }
        };
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(handPose);
    }

    [Test]
    public void CreateMirroredPose_MirrorsPositionAndRotation()
    {
        // Act
        var mirroredPose = handPose.CreateMirroredPose();

        // Assert
        Assert.AreEqual(ControllerSide.Right, mirroredPose.ControllerSide);
        Assert.AreEqual("R_Hand", mirroredPose.Rigs[0].ObjectName);
        Assert.AreEqual(new Vector3(-1, 0, 0), mirroredPose.Rigs[0].Position);
        Assert.AreEqual(new Vector3(0, -90, 0), mirroredPose.Rigs[0].Rotation);
    }

    [Test]
    public void CloneRigs_CreatesDeepCopy()
    {
        // Act
        var clonedRigs = handPose.CloneRigs();

        // Assert
        Assert.AreEqual(handPose.Rigs.Count, clonedRigs.Count);
        Assert.AreEqual(handPose.Rigs[0].ObjectName, clonedRigs[0].ObjectName);
        Assert.AreEqual(handPose.Rigs[0].Position, clonedRigs[0].Position);
        
        // Verify it's a deep copy
        clonedRigs[0].Position = Vector3.zero;
        Assert.AreNotEqual(handPose.Rigs[0].Position, clonedRigs[0].Position);
    }
}

public class ControllerHandPoseTests
{
    private GameObject gameObject;
    private ControllerHandPose controllerHandPose;

    [SetUp]
    public void Setup()
    {
        gameObject = new GameObject();
        controllerHandPose = gameObject.AddComponent<ControllerHandPose>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void Component_InitializesCorrectly()
    {
        Assert.IsNotNull(controllerHandPose);
    }
}

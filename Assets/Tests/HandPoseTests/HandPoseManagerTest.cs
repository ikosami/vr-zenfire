using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Ev;

public class HandPoseManagerTest
{
    private GameObject _managerObject;
    private HandPoseManager _handPoseManager;
    private GameObject _handRoot;

    [SetUp]
    public void Setup()
    {
        // Create test hierarchy
        _handRoot = new GameObject("HandRoot");
        CreateFingerHierarchy(_handRoot, "thumb");
        CreateFingerHierarchy(_handRoot, "index");
        CreateFingerHierarchy(_handRoot, "middle");
        
        // Create manager
        _managerObject = new GameObject("HandPoseManager");
        _handPoseManager = _managerObject.AddComponent<HandPoseManager>();
        _handPoseManager.HandPoseRoot = _handRoot.transform;
        _handPoseManager.ControllerSide = ControllerSide.Left;
        
        // Create test pose
        var testPose = ScriptableObject.CreateInstance<HandPoseScriptable>();
        testPose.PoseName = "TestPose";
        testPose.ControllerSide = ControllerSide.Left;
        testPose.Rigs = CreateTestPoseData(_handRoot);
        
        _handPoseManager.HandPoseScriptables = new List<HandPoseScriptable> { testPose };
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(_managerObject);
        Object.DestroyImmediate(_handRoot);
    }

    private void CreateFingerHierarchy(GameObject parent, string fingerName)
    {
        var finger = new GameObject(fingerName);
        finger.transform.parent = parent.transform;
        
        for (int i = 1; i <= 3; i++)
        {
            var joint = new GameObject($"{fingerName}_joint_{i}");
            joint.transform.parent = finger.transform;
        }
    }

    private List<HandRigData> CreateTestPoseData(GameObject root)
    {
        var rigData = new List<HandRigData>();
        
        void AddRigData(Transform transform)
        {
            var data = new HandRigData
            {
                ObjectName = transform.name,
                Position = Vector3.one,
                Rotation = Vector3.forward,
                Scale = Vector3.one
            };
            rigData.Add(data);
            
            foreach (Transform child in transform)
            {
                AddRigData(child);
            }
        }
        
        AddRigData(root.transform);
        return rigData;
    }

    [UnityTest]
    public IEnumerator TestPoseSelection()
    {
        _handPoseManager.GetTransforms();
        _handPoseManager.SelectPose("TestPose", true);
        
        yield return null;
        
        Assert.AreEqual("TestPose", _handPoseManager.CurrentPose.PoseName);
        Assert.AreEqual(Vector3.one, _handRoot.transform.localPosition);
    }

    [UnityTest]
    public IEnumerator TestPoseReset()
    {
        _handPoseManager.GetTransforms();
        _handPoseManager.ResetPoseName = "TestPose";
        _handPoseManager.ResetPose();
        
        yield return null;
        
        Assert.AreEqual("TestPose", _handPoseManager.CurrentPose.PoseName);
    }

    [UnityTest]
    public IEnumerator TestPoseAnimation()
    {
        _handPoseManager.GetTransforms();
        _handPoseManager.SelectPose("TestPose", false, 0.5f);
        
        // Wait for animation
        yield return new WaitForSeconds(0.6f);
        
        Assert.AreEqual("TestPose", _handPoseManager.CurrentPose.PoseName);
        Assert.AreEqual(Vector3.one, _handRoot.transform.localPosition);
    }

    [UnityTest]
    public IEnumerator TestEventBasedPoseChange()
    {
        _handPoseManager.GetTransforms();
        
        // Simulate pose change event
        EventManager.TriggerEvent(Events.Name.HAND_POSE_CHANGE, 
            new Events.HandPoseChange { 
                PoseName = "TestPose", 
                ControllerSide = ControllerSide.Left 
            });
        
        yield return new WaitForSeconds(0.6f);
        
        Assert.AreEqual("TestPose", _handPoseManager.CurrentPose.PoseName);
    }

    [UnityTest]
    public IEnumerator TestEventBasedPoseReset()
    {
        _handPoseManager.GetTransforms();
        _handPoseManager.ResetPoseName = "TestPose";
        
        // Simulate pose reset event
        EventManager.TriggerEvent(Events.Name.HAND_POSE_RESET, 
            new Events.HandPoseReset { 
                ControllerSide = ControllerSide.Left 
            });
        
        yield return new WaitForSeconds(0.6f);
        
        Assert.AreEqual("TestPose", _handPoseManager.CurrentPose.PoseName);
    }
}

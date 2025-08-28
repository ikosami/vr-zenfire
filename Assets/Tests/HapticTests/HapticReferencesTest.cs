using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Oculus.Haptics;

public class HapticReferencesTest
{
    private HapticReferencesScriptable _hapticRefs;
    private HapticClip _testClip;

    [SetUp]
    public void Setup()
    {
        // Create test scriptable object
        _hapticRefs = ScriptableObject.CreateInstance<HapticReferencesScriptable>();
        
        // Create test haptic clip
        _testClip = ScriptableObject.CreateInstance<HapticClip>();
        
        // Create test haptic reference data
        var hapticData = new HapticReferenceData
        {
            Key = "test_haptic",
            Value = _testClip
        };
        
        // Setup test data
        _hapticRefs._haptics = new List<HapticReferenceData> { hapticData };
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(_hapticRefs);
        Object.DestroyImmediate(_testClip);
    }

    [UnityTest]
    public IEnumerator TestGetClip()
    {
        var clip = _hapticRefs.GetClip("test_haptic");
        
        yield return null;
        
        Assert.IsNotNull(clip, "Should retrieve valid haptic clip");
        Assert.AreEqual(_testClip, clip, "Should retrieve correct haptic clip");
    }

    [UnityTest]
    public IEnumerator TestGetInvalidClip()
    {
        var clip = _hapticRefs.GetClip("invalid_haptic");
        
        yield return null;
        
        Assert.IsNull(clip, "Should return null for invalid haptic key");
    }

    [UnityTest]
    public IEnumerator TestLoadingState()
    {
        // First access should trigger loading
        var clip1 = _hapticRefs.GetClip("test_haptic");
        yield return null;
        Assert.IsNotNull(clip1, "First access should load and return clip");
        
        // Second access should use cached data
        var clip2 = _hapticRefs.GetClip("test_haptic");
        yield return null;
        Assert.AreEqual(clip1, clip2, "Subsequent access should return same clip instance");
    }

    [UnityTest]
    public IEnumerator TestMultipleClips()
    {
        // Create additional test clip
        var additionalClip = ScriptableObject.CreateInstance<HapticClip>();
        var additionalData = new HapticReferenceData
        {
            Key = "additional_haptic",
            Value = additionalClip
        };
        
        // Add to collection
        _hapticRefs._haptics.Add(additionalData);
        
        // Test both clips
        var clip1 = _hapticRefs.GetClip("test_haptic");
        var clip2 = _hapticRefs.GetClip("additional_haptic");
        
        yield return null;
        
        Assert.IsNotNull(clip1, "Should retrieve first clip");
        Assert.IsNotNull(clip2, "Should retrieve second clip");
        Assert.AreNotEqual(clip1, clip2, "Clips should be different instances");
        
        // Cleanup
        Object.DestroyImmediate(additionalClip);
    }
}

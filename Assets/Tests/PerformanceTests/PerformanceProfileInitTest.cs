using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using UnityEngine.XR;

public class PerformanceProfileInitTest
{
    private GameObject _managerObject;
    private PerformanceProfileInit _performanceProfile;

    [SetUp]
    public void Setup()
    {
        _managerObject = new GameObject("PerformanceProfileInit");
        _performanceProfile = _managerObject.AddComponent<PerformanceProfileInit>();
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(_managerObject);
    }

    [UnityTest]
    public IEnumerator TestFrameRateSettings()
    {
        yield return null;
        
        Assert.AreEqual(72, Application.targetFrameRate, "Frame rate should be set to 72Hz");
        Assert.AreEqual(72f, OVRPlugin.systemDisplayFrequency, "OVR display frequency should be 72Hz");
    }

    [UnityTest]
    public IEnumerator TestQualitySettings()
    {
        yield return null;
        
        Assert.AreEqual(0, QualitySettings.vSyncCount, "VSync should be disabled");
        Assert.AreEqual(2, QualitySettings.maxQueuedFrames, "Frame queue should be limited to 2");
        Assert.AreEqual(20f, QualitySettings.shadowDistance, "Shadow distance should be 20 units");
        Assert.AreEqual(ShadowResolution.Low, QualitySettings.shadowResolution, "Shadow resolution should be Low");
        Assert.AreEqual(2, QualitySettings.shadowCascades, "Shadow cascades should be 2");
        Assert.AreEqual(4, QualitySettings.antiAliasing, "Anti-aliasing should be MSAA 4x");
        Assert.AreEqual(1, QualitySettings.globalTextureMipmapLimit, "Texture mipmap limit should be 1");
    }

    [UnityTest]
    public IEnumerator TestFoveatedRendering()
    {
        yield return null;
        
        Assert.AreEqual(OVRManager.FoveatedRenderingLevel.High, OVRManager.foveatedRenderingLevel, 
            "Foveated rendering level should be High");
        Assert.IsTrue(OVRManager.useDynamicFoveatedRendering, 
            "Dynamic foveated rendering should be enabled");
    }

    [UnityTest]
    public IEnumerator TestDynamicResolutionScaling()
    {
        // Initial scale should be 1.0
        Assert.AreEqual(1.0f, XRSettings.eyeTextureResolutionScale, 
            "Initial eye texture resolution scale should be 1.0");
        
        // Test decreasing resolution
        _performanceProfile.AdjustRenderQuality(false);
        yield return null;
        Assert.Less(XRSettings.eyeTextureResolutionScale, 1.0f, 
            "Resolution scale should decrease");
        
        // Test increasing resolution
        float currentScale = XRSettings.eyeTextureResolutionScale;
        _performanceProfile.AdjustRenderQuality(true);
        yield return null;
        Assert.Greater(XRSettings.eyeTextureResolutionScale, currentScale, 
            "Resolution scale should increase");
        
        // Test minimum bound
        for (int i = 0; i < 10; i++)
        {
            _performanceProfile.AdjustRenderQuality(false);
            yield return null;
        }
        Assert.GreaterOrEqual(XRSettings.eyeTextureResolutionScale, 0.7f, 
            "Resolution scale should not go below minimum");
        
        // Test maximum bound
        for (int i = 0; i < 10; i++)
        {
            _performanceProfile.AdjustRenderQuality(true);
            yield return null;
        }
        Assert.LessOrEqual(XRSettings.eyeTextureResolutionScale, 1.2f, 
            "Resolution scale should not exceed maximum");
    }
}

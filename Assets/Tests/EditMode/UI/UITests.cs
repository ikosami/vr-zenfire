using NUnit.Framework;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.EventSystems;

public class UIPrefabTests
{
    private GameObject uiPrefab;
    private GameObject spatialPanelPrefab;

    [SetUp]
    public void Setup()
    {
        uiPrefab = Resources.Load<GameObject>("Assets/0_NS_VR/UI/Prefabs/UI.prefab");
        spatialPanelPrefab = Resources.Load<GameObject>("Assets/0_NS_VR/UI/Prefabs/Spatial Panel Manipulator UI Caution.prefab");
    }

    [TearDown]
    public void Teardown()
    {
        Resources.UnloadUnusedAssets();
    }

    [Test]
    public void UIPrefab_HasRequiredComponents()
    {
        // Arrange & Act
        var instance = Object.Instantiate(uiPrefab);

        // Assert
        Assert.IsNotNull(instance.GetComponentInChildren<TrackedDeviceGraphicRaycaster>());
        Assert.IsNotNull(instance.GetComponentInChildren<XRInteractableSnapVolume>());
        Assert.IsNotNull(instance.GetComponentInChildren<Canvas>());

        Object.DestroyImmediate(instance);
    }

    [Test]
    public void SpatialPanel_HasRequiredComponents()
    {
        // Arrange & Act
        var instance = Object.Instantiate(spatialPanelPrefab);

        // Assert
        Assert.IsNotNull(instance.GetComponentInChildren<XRGrabInteractable>());
        Assert.IsNotNull(instance.GetComponentInChildren<Canvas>());

        Object.DestroyImmediate(instance);
    }

    [Test]
    public void UIPrefab_CanvasConfiguredCorrectly()
    {
        // Arrange & Act
        var instance = Object.Instantiate(uiPrefab);
        var canvas = instance.GetComponentInChildren<Canvas>();

        // Assert
        Assert.IsTrue(canvas.renderMode == RenderMode.WorldSpace);
        Assert.IsNotNull(canvas.worldCamera);

        Object.DestroyImmediate(instance);
    }

    [Test]
    public void SpatialPanel_InteractableConfiguredCorrectly()
    {
        // Arrange & Act
        var instance = Object.Instantiate(spatialPanelPrefab);
        var interactable = instance.GetComponentInChildren<XRGrabInteractable>();

        // Assert
        Assert.IsTrue(interactable.movementType == XRBaseInteractable.MovementType.VelocityTracking);
        Assert.IsTrue(interactable.throwOnDetach);

        Object.DestroyImmediate(instance);
    }
}

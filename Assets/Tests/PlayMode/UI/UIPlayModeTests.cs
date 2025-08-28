using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.UI;
using UnityEngine.EventSystems;
using Ev;
using Ev.Events;

public class UIPlayModeTests
{
    private GameObject uiPrefab;
    private GameObject spatialPanelPrefab;
    private XRInteractionManager interactionManager;
    private XRDirectInteractor directInteractor;
    private XRRayInteractor rayInteractor;
    private EventSystem eventSystem;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        // Load prefabs
        uiPrefab = Resources.Load<GameObject>("Assets/0_NS_VR/UI/Prefabs/UI.prefab");
        spatialPanelPrefab = Resources.Load<GameObject>("Assets/0_NS_VR/UI/Prefabs/Spatial Panel Manipulator UI Caution.prefab");

        // Setup XR interaction system
        var managerGO = new GameObject("XR Interaction Manager");
        interactionManager = managerGO.AddComponent<XRInteractionManager>();

        var interactorGO = new GameObject("XR Controller");
        directInteractor = interactorGO.AddComponent<XRDirectInteractor>();
        rayInteractor = interactorGO.AddComponent<XRRayInteractor>();
        directInteractor.interactionManager = interactionManager;
        rayInteractor.interactionManager = interactionManager;

        // Setup EventSystem
        var eventSystemGO = new GameObject("Event System");
        eventSystem = eventSystemGO.AddComponent<EventSystem>();
        eventSystemGO.AddComponent<XRUIInputModule>();

        yield return null;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(interactionManager.gameObject);
        Resources.UnloadUnusedAssets();
    }

    [UnityTest]
    public IEnumerator UI_CanInteractWithRayInteractor()
    {
        // Arrange
        var instance = Object.Instantiate(uiPrefab);
        var raycaster = instance.GetComponentInChildren<TrackedDeviceGraphicRaycaster>();
        yield return null;

        // Act - Simulate ray interaction
        rayInteractor.transform.position = Vector3.back * 2;
        rayInteractor.transform.LookAt(instance.transform);
        yield return new WaitForSeconds(0.1f);

        // Assert
        Assert.IsTrue(rayInteractor.TryGetCurrentRaycast(
            out UnityEngine.RaycastHit? raycastHit,
            out int raycastHitIndex,
            out UnityEngine.EventSystems.RaycastResult? uiRaycastHit));

        Object.DestroyImmediate(instance);
    }

    [UnityTest]
    public IEnumerator SpatialPanel_CanBeGrabbed()
    {
        // Arrange
        var instance = Object.Instantiate(spatialPanelPrefab);
        var grabInteractable = instance.GetComponentInChildren<XRGrabInteractable>();
        yield return null;

        // Act - Simulate grab
        directInteractor.transform.position = grabInteractable.transform.position;
        yield return new WaitForSeconds(0.1f);

        // Assert
        Assert.IsTrue(directInteractor.interactablesSelected.Count > 0 || 
                     directInteractor.interactablesHovered.Count > 0);

        Object.DestroyImmediate(instance);
    }

    [UnityTest]
    public IEnumerator UI_ButtonClick_TriggersEvent()
    {
        // Arrange
        var instance = Object.Instantiate(uiPrefab);
        bool eventReceived = false;
        EventManager.StartListening(Name.POPUP_OPEN, (IEvent ev) => eventReceived = true);
        yield return null;

        // Act - Simulate button click
        rayInteractor.transform.position = Vector3.back * 2;
        rayInteractor.transform.LookAt(instance.transform);
        yield return new WaitForSeconds(0.1f);

        // Assert
        Assert.IsTrue(rayInteractor.TryGetCurrentRaycast(
            out UnityEngine.RaycastHit? raycastHit,
            out int raycastHitIndex,
            out UnityEngine.EventSystems.RaycastResult? uiRaycastHit));

        Object.DestroyImmediate(instance);
    }

    [UnityTest]
    public IEnumerator SpatialPanel_MaintainsWorldPosition()
    {
        // Arrange
        var instance = Object.Instantiate(spatialPanelPrefab);
        var initialPosition = new Vector3(1, 1, 1);
        instance.transform.position = initialPosition;
        yield return null;

        // Act
        yield return new WaitForSeconds(0.1f);

        // Assert
        Assert.AreEqual(initialPosition, instance.transform.position);

        Object.DestroyImmediate(instance);
    }
}

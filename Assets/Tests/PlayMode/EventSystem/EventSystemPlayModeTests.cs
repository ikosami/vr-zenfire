using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Ev;
using Ev.Events;

public class EventManagerPlayModeTests
{
    private GameObject gameObject;
    private EventManager eventManager;
    private bool eventReceived;
    private IEvent receivedEvent;

    [UnitySetUp]
    public IEnumerator Setup()
    {
        gameObject = new GameObject();
        eventManager = gameObject.AddComponent<EventManager>();
        eventReceived = false;
        receivedEvent = null;
        yield return null;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(gameObject);
    }

    [UnityTest]
    public IEnumerator EventManager_PersistsThroughSceneLoad()
    {
        // Arrange
        void EventHandler(IEvent ev) { eventReceived = true; receivedEvent = ev; }
        EventManager.StartListening(Name.POPUP_OPEN, EventHandler);
        
        // Act - Simulate scene reload by destroying and recreating objects
        Object.DestroyImmediate(gameObject);
        yield return null;
        
        gameObject = new GameObject();
        eventManager = gameObject.AddComponent<EventManager>();
        yield return null;

        EventManager.TriggerEvent(Name.POPUP_OPEN, new PopUpOpen { PopUpName = "TestPopup" });
        yield return null;

        // Assert
        Assert.IsTrue(eventReceived);
        Assert.IsNotNull(receivedEvent);
        Assert.AreEqual(Name.POPUP_OPEN, receivedEvent.EventName);
    }

    [UnityTest]
    public IEnumerator EventManager_HandlesMultipleEventsInSequence()
    {
        // Arrange
        int eventsReceived = 0;
        void PopupHandler(IEvent ev) { if (ev.EventName == Name.POPUP_OPEN) eventsReceived++; }
        void HandPoseHandler(IEvent ev) { if (ev.EventName == Name.HAND_POSE_CHANGE) eventsReceived++; }
        
        EventManager.StartListening(Name.POPUP_OPEN, PopupHandler);
        EventManager.StartListening(Name.HAND_POSE_CHANGE, HandPoseHandler);
        yield return null;

        // Act
        EventManager.TriggerEvent(Name.POPUP_OPEN, new PopUpOpen { PopUpName = "Test1" });
        yield return null;
        
        EventManager.TriggerEvent(Name.HAND_POSE_CHANGE, new HandPoseChange { 
            PoseName = "TestPose",
            ControllerSide = ControllerSide.Left
        });
        yield return null;

        // Assert
        Assert.AreEqual(2, eventsReceived);
    }

    [UnityTest]
    public IEnumerator EventHandlerBase_HandlesEventsInRuntime()
    {
        // Arrange
        var handler = ScriptableObject.CreateInstance<TestEventHandler>();
        handler.HandleEventNames = new System.Collections.Generic.List<string> { Name.POPUP_OPEN };
        bool onEndCalled = false;
        
        handler.StartListening((ev) => onEndCalled = true);
        yield return null;

        // Act
        EventManager.TriggerEvent(Name.POPUP_OPEN, new PopUpOpen { PopUpName = "Test" });
        yield return null;

        // Assert
        Assert.IsTrue(handler.handleCalled);
        Assert.IsTrue(onEndCalled);
        Assert.IsNotNull(handler.lastEvent);
        Assert.AreEqual(Name.POPUP_OPEN, handler.lastEvent.EventName);
        
        Object.DestroyImmediate(handler);
    }

    private class TestEventHandler : EventHandlerBase
    {
        public bool handleCalled;
        public IEvent lastEvent;

        public override void Handle(IEvent e)
        {
            handleCalled = true;
            lastEvent = e;
        }
    }
}

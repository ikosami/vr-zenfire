using NUnit.Framework;
using UnityEngine;
using Ev;
using Ev.Events;

public class EventManagerTests
{
    private GameObject gameObject;
    private EventManager eventManager;
    private bool eventReceived;
    private IEvent receivedEvent;

    [SetUp]
    public void Setup()
    {
        gameObject = new GameObject();
        eventManager = gameObject.AddComponent<EventManager>();
        eventReceived = false;
        receivedEvent = null;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(gameObject);
    }

    [Test]
    public void StartListening_RegistersEventHandler()
    {
        // Arrange
        void EventHandler(IEvent ev) { eventReceived = true; receivedEvent = ev; }

        // Act
        EventManager.StartListening(Name.POPUP_OPEN, EventHandler);
        EventManager.TriggerEvent(Name.POPUP_OPEN, new PopUpOpen { PopUpName = "TestPopup" });

        // Assert
        Assert.IsTrue(eventReceived);
        Assert.IsNotNull(receivedEvent);
        Assert.AreEqual(Name.POPUP_OPEN, receivedEvent.EventName);
    }

    [Test]
    public void StopListening_UnregistersEventHandler()
    {
        // Arrange
        void EventHandler(IEvent ev) { eventReceived = true; }
        EventManager.StartListening(Name.POPUP_OPEN, EventHandler);

        // Act
        EventManager.StopListening(Name.POPUP_OPEN, EventHandler);
        EventManager.TriggerEvent(Name.POPUP_OPEN, new PopUpOpen { PopUpName = "TestPopup" });

        // Assert
        Assert.IsFalse(eventReceived);
    }

    [Test]
    public void TriggerEvent_WithDefaultEvent_WorksCorrectly()
    {
        // Arrange
        void EventHandler(IEvent ev) { eventReceived = true; receivedEvent = ev; }
        EventManager.StartListening(Name.POPUP_OPEN, EventHandler);

        // Act
        EventManager.TriggerEvent(Name.POPUP_OPEN);

        // Assert
        Assert.IsTrue(eventReceived);
        Assert.IsNotNull(receivedEvent);
        Assert.IsTrue(receivedEvent is DefaultEvent);
    }
}

public class EventHandlerBaseTests
{
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

    private TestEventHandler handler;

    [SetUp]
    public void Setup()
    {
        handler = ScriptableObject.CreateInstance<TestEventHandler>();
        handler.HandleEventNames = new System.Collections.Generic.List<string> { Name.POPUP_OPEN };
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(handler);
    }

    [Test]
    public void StartListening_ReceivesEvents()
    {
        // Arrange
        handler.StartListening(null);

        // Act
        EventManager.TriggerEvent(Name.POPUP_OPEN, new PopUpOpen { PopUpName = "Test" });

        // Assert
        Assert.IsTrue(handler.handleCalled);
        Assert.IsNotNull(handler.lastEvent);
        Assert.AreEqual(Name.POPUP_OPEN, handler.lastEvent.EventName);
    }

    [Test]
    public void StopListening_StopsReceivingEvents()
    {
        // Arrange
        handler.StartListening(null);
        handler.StopListening();
        handler.handleCalled = false;

        // Act
        EventManager.TriggerEvent(Name.POPUP_OPEN, new PopUpOpen { PopUpName = "Test" });

        // Assert
        Assert.IsFalse(handler.handleCalled);
    }
}

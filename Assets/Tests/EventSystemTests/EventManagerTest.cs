using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using Ev;

public class EventManagerTest
{
    private bool _eventTriggered;
    private string _receivedData;

    [SetUp]
    public void Setup()
    {
        // Create EventManager GameObject for testing
        GameObject eventManagerObj = new GameObject("EventManager");
        eventManagerObj.AddComponent<EventManager>();
    }

    [TearDown]
    public void Teardown()
    {
        // Clean up event subscriptions
        EventManager.StopListening("TEST_EVENT", OnTestEvent);
        EventManager.StopListening("TEST_DATA_EVENT", OnTestDataEvent);
        
        // Destroy the EventManager
        Object.DestroyImmediate(EventManager.instance.gameObject);
    }

    [UnityTest]
    public IEnumerator TestEventTrigger()
    {
        _eventTriggered = false;
        EventManager.StartListening("TEST_EVENT", OnTestEvent);

        // Trigger event
        EventManager.TriggerEvent("TEST_EVENT");

        yield return null; // Wait a frame for event processing
        Assert.IsTrue(_eventTriggered, "Event was not triggered");
    }

    [UnityTest]
    public IEnumerator TestEventData()
    {
        _receivedData = null;
        EventManager.StartListening("TEST_DATA_EVENT", OnTestDataEvent);

        // Create test event data
        var testData = new TestEventData { message = "test_message" };
        EventManager.TriggerEvent("TEST_DATA_EVENT", testData);

        yield return null; // Wait a frame for event processing
        Assert.AreEqual("test_message", _receivedData, "Event data was not received correctly");
    }

    [UnityTest]
    public IEnumerator TestEventUnsubscribe()
    {
        _eventTriggered = false;
        EventManager.StartListening("TEST_EVENT", OnTestEvent);
        EventManager.StopListening("TEST_EVENT", OnTestEvent);

        // Trigger event after unsubscribing
        EventManager.TriggerEvent("TEST_EVENT");

        yield return null; // Wait a frame for event processing
        Assert.IsFalse(_eventTriggered, "Event was triggered after unsubscribing");
    }

    [UnityTest]
    public IEnumerator TestMultipleSubscribers()
    {
        int triggerCount = 0;
        System.Action<IEvent> listener1 = (_) => triggerCount++;
        System.Action<IEvent> listener2 = (_) => triggerCount++;

        EventManager.StartListening("MULTI_EVENT", listener1);
        EventManager.StartListening("MULTI_EVENT", listener2);

        EventManager.TriggerEvent("MULTI_EVENT");

        yield return null; // Wait a frame for event processing
        Assert.AreEqual(2, triggerCount, "Not all subscribers received the event");

        // Cleanup
        EventManager.StopListening("MULTI_EVENT", listener1);
        EventManager.StopListening("MULTI_EVENT", listener2);
    }

    private void OnTestEvent(IEvent evt)
    {
        _eventTriggered = true;
    }

    private void OnTestDataEvent(IEvent evt)
    {
        if (evt is TestEventData testData)
        {
            _receivedData = testData.message;
        }
    }
}

// Test event data class
public class TestEventData : IEvent
{
    public string message;
}

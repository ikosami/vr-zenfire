using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;

public class TestPuzzleState : PuzzleState
{
    protected override bool CheckCompletionConditions()
    {
        return GetStateValue<bool>("isComplete", false);
    }
}

public class PuzzleStateTest
{
    private GameObject puzzleObject;
    private TestPuzzleState puzzleState;
    private bool eventTriggered;
    private Dictionary<string, object> lastEventData;

    [SetUp]
    public void Setup()
    {
        puzzleObject = new GameObject("TestPuzzle");
        puzzleState = puzzleObject.AddComponent<TestPuzzleState>();
        eventTriggered = false;
        lastEventData = null;
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(puzzleObject);
    }

    [UnityTest]
    public IEnumerator TestStateUpdate()
    {
        puzzleState.UpdateState("testKey", "testValue");
        
        yield return null;
        
        Assert.AreEqual("testValue", puzzleState.GetStateValue<string>("testKey"));
    }

    [UnityTest]
    public IEnumerator TestCompletionCheck()
    {
        Assert.IsFalse(puzzleState.IsCompleted, "Puzzle should start incomplete");
        
        puzzleState.UpdateState("isComplete", true);
        
        yield return null;
        
        Assert.IsTrue(puzzleState.IsCompleted, "Puzzle should complete when conditions are met");
    }

    [UnityTest]
    public IEnumerator TestStateReset()
    {
        puzzleState.UpdateState("testKey", "testValue");
        puzzleState.UpdateState("isComplete", true);
        
        yield return null;
        
        puzzleState.ResetState();
        
        Assert.IsFalse(puzzleState.IsCompleted, "Puzzle should be incomplete after reset");
        Assert.IsNull(puzzleState.GetStateValue<string>("testKey"), "State data should be cleared after reset");
    }

    [UnityTest]
    public IEnumerator TestStartCompleted()
    {
        Object.DestroyImmediate(puzzleObject);
        
        puzzleObject = new GameObject("TestPuzzle");
        puzzleState = puzzleObject.AddComponent<TestPuzzleState>();
        puzzleState.startCompleted = true;
        
        yield return null;
        
        Assert.IsTrue(puzzleState.IsCompleted, "Puzzle should start completed when startCompleted is true");
    }

    [UnityTest]
    public IEnumerator TestStateEvents()
    {
        eventTriggered = false;
        lastEventData = null;
        
        // Add test event
        var stateEvent = new PuzzleStateEvent
        {
            eventName = "TestEvent",
            onTrigger = new UnityEngine.Events.UnityEvent<Dictionary<string, object>>()
        };
        stateEvent.onTrigger.AddListener((data) => {
            eventTriggered = true;
            lastEventData = data;
        });
        
        puzzleState.onStateChangeEvents.Add(stateEvent);
        
        // Update state
        puzzleState.UpdateState("testKey", "testValue");
        
        yield return null;
        
        Assert.IsTrue(eventTriggered, "State change event should trigger");
        Assert.IsNotNull(lastEventData, "Event should receive state data");
        Assert.AreEqual("testValue", lastEventData["testKey"]);
    }
}

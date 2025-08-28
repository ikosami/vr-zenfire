using UnityEngine;
using UnityEngine.TestTools;
using NUnit.Framework;
using System.Collections;

public class PuzzleManagerTest
{
    private GameObject managerObject;
    private PuzzleManager manager;
    private GameObject[] puzzleObjects;
    private TestPuzzleState[] puzzleStates;

    [SetUp]
    public void Setup()
    {
        // Create manager
        managerObject = new GameObject("PuzzleManager");
        manager = managerObject.AddComponent<PuzzleManager>();
        
        // Create test puzzles
        puzzleObjects = new GameObject[3];
        puzzleStates = new TestPuzzleState[3];
        
        for (int i = 0; i < 3; i++)
        {
            puzzleObjects[i] = new GameObject($"Puzzle_{i}");
            puzzleStates[i] = puzzleObjects[i].AddComponent<TestPuzzleState>();
            puzzleStates[i].puzzleId = $"puzzle_{i}";
        }
    }

    [TearDown]
    public void Teardown()
    {
        Object.DestroyImmediate(managerObject);
        foreach (var obj in puzzleObjects)
        {
            Object.DestroyImmediate(obj);
        }
        PlayerPrefs.DeleteKey("puzzle_progress");
    }

    [UnityTest]
    public IEnumerator TestPuzzleRegistration()
    {
        manager.RegisterPuzzle(puzzleStates[0]);
        
        yield return null;
        
        Assert.IsFalse(manager.IsPuzzleCompleted("puzzle_0"), "Newly registered puzzle should not be completed");
    }

    [UnityTest]
    public IEnumerator TestPuzzleCompletion()
    {
        manager.RegisterPuzzle(puzzleStates[0]);
        puzzleStates[0].UpdateState("isComplete", true);
        
        yield return null;
        
        Assert.IsTrue(manager.IsPuzzleCompleted("puzzle_0"), "Puzzle should be marked as completed");
    }

    [UnityTest]
    public IEnumerator TestCompletionPercentage()
    {
        // Register all puzzles
        foreach (var puzzle in puzzleStates)
        {
            manager.RegisterPuzzle(puzzle);
        }
        
        yield return null;
        
        Assert.AreEqual(0f, manager.GetCompletionPercentage(), "Initial completion should be 0%");
        
        // Complete two puzzles
        puzzleStates[0].UpdateState("isComplete", true);
        puzzleStates[1].UpdateState("isComplete", true);
        
        yield return null;
        
        Assert.AreEqual(2f/3f, manager.GetCompletionPercentage(), "Completion percentage should be 66.6%");
    }

    [UnityTest]
    public IEnumerator TestProgressSaveLoad()
    {
        // Register and complete puzzles
        manager.RegisterPuzzle(puzzleStates[0]);
        puzzleStates[0].UpdateState("isComplete", true);
        
        yield return null;
        
        // Destroy and recreate manager
        Object.DestroyImmediate(managerObject);
        managerObject = new GameObject("PuzzleManager");
        manager = managerObject.AddComponent<PuzzleManager>();
        
        yield return null;
        
        // Register puzzle again
        manager.RegisterPuzzle(puzzleStates[0]);
        
        Assert.IsTrue(manager.IsPuzzleCompleted("puzzle_0"), "Puzzle completion should persist after reload");
    }

    [UnityTest]
    public IEnumerator TestProgressReset()
    {
        // Register and complete puzzles
        foreach (var puzzle in puzzleStates)
        {
            manager.RegisterPuzzle(puzzle);
            puzzle.UpdateState("isComplete", true);
        }
        
        yield return null;
        
        manager.ResetProgress();
        
        yield return null;
        
        Assert.AreEqual(0f, manager.GetCompletionPercentage(), "Progress should be reset to 0%");
        foreach (var puzzle in puzzleStates)
        {
            Assert.IsFalse(manager.IsPuzzleCompleted(puzzle.PuzzleId), "All puzzles should be incomplete after reset");
        }
    }
}

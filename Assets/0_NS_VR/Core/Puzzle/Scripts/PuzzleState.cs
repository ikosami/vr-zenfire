using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Base class for managing puzzle states and transitions
/// </summary>
public abstract class PuzzleState : MonoBehaviour
{
    [Header("Puzzle Settings")]
    [SerializeField] protected string puzzleId;
    [SerializeField] protected bool startCompleted = false;
    
    [Header("State Events")]
    [SerializeField] protected List<PuzzleStateEvent> onStateChangeEvents = new List<PuzzleStateEvent>();
    [SerializeField] protected List<PuzzleStateEvent> onCompletionEvents = new List<PuzzleStateEvent>();

    protected bool isCompleted = false;
    protected Dictionary<string, object> stateData = new Dictionary<string, object>();

    public bool IsCompleted => isCompleted;
    public string PuzzleId => puzzleId;

    protected virtual void Start()
    {
        if (startCompleted)
        {
            CompletePuzzle();
        }
    }

    /// <summary>
    /// Update puzzle state
    /// </summary>
    public virtual void UpdateState(string key, object value)
    {
        stateData[key] = value;
        OnStateChanged();
        CheckCompletion();
    }

    /// <summary>
    /// Check if puzzle completion conditions are met
    /// </summary>
    protected abstract bool CheckCompletionConditions();

    /// <summary>
    /// Called when puzzle state changes
    /// </summary>
    protected virtual void OnStateChanged()
    {
        foreach (var evt in onStateChangeEvents)
        {
            evt.TriggerEvent(stateData);
        }
    }

    /// <summary>
    /// Complete the puzzle and trigger completion events
    /// </summary>
    public virtual void CompletePuzzle()
    {
        if (isCompleted) return;
        
        isCompleted = true;
        foreach (var evt in onCompletionEvents)
        {
            evt.TriggerEvent(stateData);
        }
        
        // Notify puzzle manager
        PuzzleManager.Instance.OnPuzzleCompleted(this);
    }

    /// <summary>
    /// Reset puzzle state
    /// </summary>
    public virtual void ResetState()
    {
        isCompleted = startCompleted;
        stateData.Clear();
    }

    /// <summary>
    /// Check completion conditions and update state if needed
    /// </summary>
    protected virtual void CheckCompletion()
    {
        if (!isCompleted && CheckCompletionConditions())
        {
            CompletePuzzle();
        }
    }

    /// <summary>
    /// Get current state value
    /// </summary>
    public T GetStateValue<T>(string key, T defaultValue = default)
    {
        if (stateData.TryGetValue(key, out object value))
        {
            if (value is T typedValue)
            {
                return typedValue;
            }
        }
        return defaultValue;
    }
}

/// <summary>
/// Event triggered by puzzle state changes
/// </summary>
[Serializable]
public class PuzzleStateEvent
{
    public string eventName;
    public UnityEngine.Events.UnityEvent<Dictionary<string, object>> onTrigger;

    public void TriggerEvent(Dictionary<string, object> stateData)
    {
        onTrigger?.Invoke(stateData);
    }
}

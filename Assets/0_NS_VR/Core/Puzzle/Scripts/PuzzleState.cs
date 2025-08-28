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
    /// <summary>
    /// パズルを完了状態に設定し、関連するイベントを発火
    /// </summary>
    /// <remarks>
    /// 実行される処理：
    /// 1. 完了フラグの設定
    /// 2. 登録された完了イベントの実行
    /// 3. PuzzleManagerへの完了通知
    /// 
    /// 注意：既に完了している場合は何も実行されません
    /// </remarks>
    public virtual void CompletePuzzle()
    {
        // 既に完了している場合は処理をスキップ
        if (isCompleted) return;
        
        // 完了フラグを設定
        isCompleted = true;

        // 登録された完了イベントを実行
        foreach (var evt in onCompletionEvents)
        {
            evt.TriggerEvent(stateData);
        }
        
        // パズルマネージャーに完了を通知
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
    /// <summary>
    /// パズルの完了条件をチェックし、条件を満たしている場合は完了処理を実行
    /// </summary>
    /// <remarks>
    /// このメソッドは状態が変更されるたびに呼び出され、
    /// CheckCompletionConditionsで定義された条件に基づいて
    /// パズルの完了状態を更新します
    /// </remarks>
    protected virtual void CheckCompletion()
    {
        // 未完了かつ完了条件を満たしている場合、完了処理を実行
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

using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages puzzle states and completion tracking across the game
/// </summary>
public class PuzzleManager : MonoBehaviour
{
    private static PuzzleManager _instance;
    public static PuzzleManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<PuzzleManager>();
                if (_instance == null)
                {
                    var go = new GameObject("PuzzleManager");
                    _instance = go.AddComponent<PuzzleManager>();
                }
            }
            return _instance;
        }
    }

    [Header("Puzzle Settings")]
    [SerializeField] private bool autoSave = true;
    [SerializeField] private string saveKey = "puzzle_progress";

    private Dictionary<string, PuzzleState> puzzles = new Dictionary<string, PuzzleState>();
    private HashSet<string> completedPuzzles = new HashSet<string>();

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }
        _instance = this;
        DontDestroyOnLoad(gameObject);

        LoadProgress();
    }

    /// <summary>
    /// Register a puzzle with the manager
    /// </summary>
    public void RegisterPuzzle(PuzzleState puzzle)
    {
        if (!puzzles.ContainsKey(puzzle.PuzzleId))
        {
            puzzles[puzzle.PuzzleId] = puzzle;
            if (completedPuzzles.Contains(puzzle.PuzzleId))
            {
                puzzle.CompletePuzzle();
            }
        }
    }

    /// <summary>
    /// Called when a puzzle is completed
    /// </summary>
    public void OnPuzzleCompleted(PuzzleState puzzle)
    {
        if (!completedPuzzles.Contains(puzzle.PuzzleId))
        {
            completedPuzzles.Add(puzzle.PuzzleId);
            if (autoSave)
            {
                SaveProgress();
            }
        }
    }

    /// <summary>
    /// Check if a puzzle is completed
    /// </summary>
    public bool IsPuzzleCompleted(string puzzleId)
    {
        return completedPuzzles.Contains(puzzleId);
    }

    /// <summary>
    /// Get total completion percentage
    /// </summary>
    public float GetCompletionPercentage()
    {
        if (puzzles.Count == 0) return 0f;
        return (float)completedPuzzles.Count / puzzles.Count;
    }

    /// <summary>
    /// Reset all puzzle progress
    /// </summary>
    public void ResetProgress()
    {
        completedPuzzles.Clear();
        foreach (var puzzle in puzzles.Values)
        {
            puzzle.ResetState();
        }
        if (autoSave)
        {
            SaveProgress();
        }
    }

    /// <summary>
    /// Save puzzle progress
    /// </summary>
    private void SaveProgress()
    {
        string[] completed = new string[completedPuzzles.Count];
        completedPuzzles.CopyTo(completed);
        string progress = JsonUtility.ToJson(new PuzzleProgress { completedPuzzles = completed });
        PlayerPrefs.SetString(saveKey, progress);
        PlayerPrefs.Save();
    }

    /// <summary>
    /// Load puzzle progress
    /// </summary>
    private void LoadProgress()
    {
        if (PlayerPrefs.HasKey(saveKey))
        {
            string progress = PlayerPrefs.GetString(saveKey);
            PuzzleProgress data = JsonUtility.FromJson<PuzzleProgress>(progress);
            completedPuzzles = new HashSet<string>(data.completedPuzzles);
        }
    }
}

[System.Serializable]
public class PuzzleProgress
{
    public string[] completedPuzzles;
}

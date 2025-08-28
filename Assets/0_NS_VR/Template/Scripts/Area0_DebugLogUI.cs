using UnityEngine;
using TMPro;

public class Area0_DebugLogUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI logText;
    [SerializeField] private int maxLines = 10;
    private System.Collections.Generic.Queue<string> logLines = new System.Collections.Generic.Queue<string>();

    private void Awake()
    {
        Application.logMessageReceived += HandleLog;
        logText.text = "Debug Log UI Ready...";
    }

    private void OnDestroy()
    {
        Application.logMessageReceived -= HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string formattedLog = $"[{type}] {logString}";
        logLines.Enqueue(formattedLog);
        
        while (logLines.Count > maxLines)
        {
            logLines.Dequeue();
        }

        logText.text = string.Join("\n", logLines);
    }
}

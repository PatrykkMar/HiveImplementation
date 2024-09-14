using System;
using System.IO;
using UnityEngine;

public class LogToFile
{
    private string _logFilePath;

    // Inicjalizacja klasy i subskrypcja logów Unity
    public LogToFile()
    {
        Initialize();
    }

    public void Initialize()
    {
        string projectRootPath = Directory.GetParent(Application.dataPath).FullName;
        string logDirectory = Path.Combine(projectRootPath, "Logs");
        if (!Directory.Exists(logDirectory))
        {
            Directory.CreateDirectory(logDirectory);
        }

        _logFilePath = Path.Combine(logDirectory, "unity_log.txt");

        Application.logMessageReceived += HandleLog;
    }

    private void HandleLog(string logString, string stackTrace, LogType type)
    {
        string logEntry = $"{DateTime.Now}: {type} - {logString}\n";

        if (type == LogType.Exception || type == LogType.Error)
        {
            logEntry += $"{stackTrace}\n";
        }

        File.AppendAllText(_logFilePath, logEntry);
    }

    public void Log(string message, LogType type = LogType.Log)
    {
        HandleLog(message, string.Empty, type);
    }
}
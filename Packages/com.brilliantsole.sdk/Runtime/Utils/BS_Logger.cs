using System.Collections.Generic;
using System.Diagnostics;

public class BS_Logger
{
    public enum LogLevel
    {
        Log,
        Warn,
        Error,
        None
    }

    private static readonly Dictionary<string, BS_Logger> instances = new Dictionary<string, BS_Logger>();

    private readonly string category;
    private LogLevel currentLogLevel;

    // Private constructor that allows specifying the default log level
    private BS_Logger(string category, LogLevel logLevel)
    {
        this.category = category;
        currentLogLevel = logLevel;
    }

    // Get or create an instance for the specified category with an optional log level
    public static BS_Logger GetLogger(string category, LogLevel logLevel = LogLevel.Log)
    {
        if (!instances.TryGetValue(category, out var logger))
        {
            // Create a new logger with the specified log level
            logger = new BS_Logger(category, logLevel);
            instances[category] = logger;
        }
        else
        {
            // Update the log level if the instance already exists
            logger.SetLogLevel(logLevel);
        }
        return logger;
    }

    // Set the log level for this specific logger instance
    public void SetLogLevel(LogLevel logLevel)
    {
        currentLogLevel = logLevel;
    }

    // Static method to set the log level for a specific category
    public static void SetLogLevel(string category, LogLevel logLevel)
    {
        if (instances.TryGetValue(category, out var logger))
        {
            logger.currentLogLevel = logLevel;
        }
    }

    // Log message based on current log level
    [Conditional("UNITY_EDITOR")]
    public void Log(string message)
    {
        if (currentLogLevel <= LogLevel.Log)
        {
            UnityEngine.Debug.Log($"[{category}] {message}");
        }
    }

    [Conditional("UNITY_EDITOR")]
    public void LogWarning(string message)
    {
        if (currentLogLevel <= LogLevel.Warn)
        {
            UnityEngine.Debug.LogWarning($"[{category}] {message}");
        }
    }

    [Conditional("UNITY_EDITOR")]
    public void LogError(string message)
    {
        if (currentLogLevel <= LogLevel.Error)
        {
            UnityEngine.Debug.LogError($"[{category}] {message}");
        }
    }
}

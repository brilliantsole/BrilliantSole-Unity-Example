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

    private static readonly Dictionary<string, BS_Logger> instances = new();

    private readonly string category;
    private LogLevel currentLogLevel;

    private BS_Logger(string category, LogLevel logLevel)
    {
        this.category = category;
        currentLogLevel = logLevel;
    }

    public static BS_Logger GetLogger(string category, LogLevel logLevel = LogLevel.Error)
    {
        if (!instances.TryGetValue(category, out var logger))
        {
            logger = new BS_Logger(category, logLevel);
            instances[category] = logger;
        }
        else
        {
            logger.SetLogLevel(logLevel);
        }
        return logger;
    }

    public void SetLogLevel(LogLevel logLevel)
    {
        currentLogLevel = logLevel;
    }

    public static void SetLogLevel(string category, LogLevel logLevel)
    {
        if (instances.TryGetValue(category, out var logger))
        {
            logger.currentLogLevel = logLevel;
        }
    }


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

using UnityEngine;

public class BS_DevicePair
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_DevicePair", BS_Logger.LogLevel.Log);

    public static readonly BS_DevicePair Instance;
    public bool IsInstance => Instance == this;

    static BS_DevicePair()
    {
        Instance = new();
        // FILL - add deviceManager listeners
    }
}

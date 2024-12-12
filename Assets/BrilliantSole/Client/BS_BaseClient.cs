using System;
using UnityEngine;

public abstract partial class BS_BaseClient
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseClient", BS_Logger.LogLevel.Log);

    protected virtual void SendData(byte[] data) { Logger.Log($"sending {data.Length} bytes..."); }
}

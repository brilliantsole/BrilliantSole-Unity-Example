using System;
using System.Collections.Generic;
using UnityEngine.Events;

public class BS_UdpClientManager : BS_SingletonMonoBehavior<BS_UdpClientManager>
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_UdpClientManager");
    private static BS_BaseClient Client => BS_UdpClient.Instance;

    private void OnEnable()
    {
        // Add Listeners
    }

    private void OnDisable()
    {
        // Remove Listeners
    }
}

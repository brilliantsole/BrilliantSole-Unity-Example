using System.Net.Sockets;

#if UNITY_EDITOR
using UnityEditor;
#endif

public partial class BS_UdpClient : BS_BaseClient<BS_UdpClient>
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_UdpClient", BS_Logger.LogLevel.Log);

    private UdpClient UdpClient;
    private bool IsRunning = false;

    protected override void Reset()
    {
        base.Reset();
        messageQueue.Clear();
        DidSetRemoteReceivePort = false;
    }

    public BS_UdpClient()
    {
#if UNITY_EDITOR
        EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
#endif
    }

    public override void Update()
    {
        base.Update();
        ParseReceivedMessages();
        CheckPongTimeout();
    }

#if UNITY_EDITOR
    private void OnPlayModeStateChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingPlayMode || state == PlayModeStateChange.EnteredEditMode)
        {
            Disconnect();
        }
    }
#endif
}
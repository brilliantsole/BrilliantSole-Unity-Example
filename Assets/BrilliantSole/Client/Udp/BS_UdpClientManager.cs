public partial class BS_UdpClientManager : BS_BaseClientManager<BS_UdpClientManager, BS_UdpClient>
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_UdpClientManager", BS_Logger.LogLevel.Log);
    protected override BS_UdpClient Client => BS_UdpClient.Instance;

    protected override void OnEnable()
    {
        base.OnEnable();
    }
    protected override void OnDisable()
    {
        base.OnDisable();
    }
}
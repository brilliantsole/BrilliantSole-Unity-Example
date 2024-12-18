public class BS_UdpClientScrollView : BS_BaseScannerScrollView
{
    protected override IBS_ScannerManager ScannerManager => BS_UdpClientManager.Instance;
}

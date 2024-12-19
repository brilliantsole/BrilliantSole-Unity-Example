using TMPro;
using UnityEngine.UI;
using static BS_ConnectionStatus;

public class BS_UdpClientScrollView : BS_BaseScannerScrollView
{
    private BS_UdpClientManager ClientManager => BS_UdpClientManager.Instance;
    protected override IBS_ScannerManager ScannerManager => ClientManager;

    public Button ToggleConnectionButton;

    protected override void OnEnable()
    {
        base.OnEnable();
        ToggleConnectionButton.onClick.AddListener(ClientManager.ToggleConnection);
        ClientManager.OnConnectionStatus.AddListener(UpdateToggleConnectionButton);
        ClientManager.OnNotConnected.AddListener(Clear);
        UpdateToggleConnectionButton(ClientManager.ConnectionStatus);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        ToggleConnectionButton.onClick.RemoveListener(ClientManager.ToggleConnection);
        ClientManager.OnConnectionStatus.RemoveListener(UpdateToggleConnectionButton);
        ClientManager.OnNotConnected.RemoveListener(Clear);

    }

    private void UpdateToggleConnectionButton(BS_ConnectionStatus connectionStatus)
    {
        var toggleConnectionButtonText = ToggleConnectionButton.transform.GetComponentInChildren<TextMeshProUGUI>();
        toggleConnectionButtonText.text = connectionStatus switch
        {
            NotConnected => "Connect",
            Connecting => "Connecting",
            Connected => "Disconnect",
            Disconnecting => "Disconnecting",
            _ => throw new System.NotImplementedException()
        };
    }
}

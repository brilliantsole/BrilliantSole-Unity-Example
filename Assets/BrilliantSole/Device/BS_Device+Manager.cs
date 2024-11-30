public partial class BS_Device
{
    private readonly BS_InformationManager InformationManager = new();
    private readonly BS_SensorConfigurationManager SensorConfigurationManager = new();
    private readonly BS_SensorDataManager SensorDataManager = new();
    private readonly BS_VibrationManager VibrationManager = new();
    private readonly BS_BaseManager[] Managers;

    private void SetupManagers()
    {
        foreach (var Manager in Managers) { Manager.SendTxMessages = SendTxMessages; }
    }

    private void ResetManagers()
    {
        foreach (var Manager in Managers) { Manager.Reset(); }
    }
}

public partial class BS_Device
{
    private readonly BS_InformationManager InformationManager = new();
    private readonly BS_SensorConfigurationManager SensorConfigurationManager = new();
    private readonly BS_SensorDataManager SensorDataManager = new();
    private readonly BS_VibrationManager VibrationManager = new();
    private readonly BS_BaseManager[] Managers;

    private void SetupManagers()
    {
        foreach (var BaseManager in Managers) { BaseManager.SendTxMessages = SendTxMessages; }
    }

}
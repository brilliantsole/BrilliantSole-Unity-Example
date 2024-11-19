public abstract class BS_BaseScanner
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger(typeof(BS_BaseScanner).Name, BS_Logger.LogLevel.Warn);

    protected void StartScan()
    {
        Logger.Log("Scan started.");
    }

    protected void StopScan()
    {
        Logger.Log("Scan started.");
    }
}
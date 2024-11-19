using UnityEngine;

[CreateAssetMenu(fileName = "BS_Scanner", menuName = "BrilliantSole/Scanner")]
public class BS_ScannerSO : ScriptableObject
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_ScannerSO", BS_Logger.LogLevel.Log);

    private static BS_ScannerSO _instance;
    public static BS_ScannerSO Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Resources.Load<BS_ScannerSO>("BS_Scanner");

                if (_instance == null)
                {
                    Logger.LogError("BS_Scanner ScriptableObject is not found in Resources");
                }
            }
            return _instance;
        }
    }

    // FILL - dispatch events
    // FILL - trigger functions
}

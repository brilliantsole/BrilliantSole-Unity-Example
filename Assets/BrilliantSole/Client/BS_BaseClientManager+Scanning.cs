using UnityEngine;

public abstract partial class BS_BaseClientManager<TClientManager, TClient> : BS_SingletonMonoBehavior<TClientManager>
    where TClientManager : MonoBehaviour
    where TClient : BS_BaseClient
{
    public BoolUnityEvent OnIsScanning;
    public BoolUnityEvent OnIsScanningAvailable;

    public bool IsScanning => Client.IsScanning;
    public bool IsScanningAvailable => Client.IsScanningAvailable;

    private void onIsScanningAvailable(BS_BaseClient client, bool isScanningAvailable) { OnIsScanningAvailable?.Invoke(isScanningAvailable); }
    private void onIsScanning(BS_BaseClient client, bool isScanning) { OnIsScanning?.Invoke(isScanning); }

}

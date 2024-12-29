using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class BS_EyeTrackingButton : BS_BaseEyeTrackingUIImageComponent
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_EyeTrackingButton", BS_Logger.LogLevel.Log);

    private Button button;

    protected override void Start()
    {
        base.Start();
        button = GetComponent<Button>();
    }

    protected override void OnTap(BS_InsoleSide insoleSide)
    {
        base.OnTap(insoleSide);
        button.onClick.Invoke();
    }
}

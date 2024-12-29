using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static BS_VibrationWaveformEffect;

[RequireComponent(typeof(Button))]
public class BS_EyeTrackingButton : BS_BaseEyeTrackingUIImageComponent
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_EyeTrackingButton");

    private Button button;

    protected override void Start()
    {
        base.Start();
        button = GetComponent<Button>();
    }

    public List<BS_VibrationConfiguration> ClickVibrationConfigurations = new()
    {
        new () {
            Locations = BS_VibrationLocationFlag.Front | BS_VibrationLocationFlag.Rear,
            Type = BS_VibrationType.WaveformEffect,
            WaveformEffectSequence = new() {new(StrongClick_100)}
        }
    };

    protected override void OnTap(BS_InsoleSide insoleSide)
    {
        base.OnTap(insoleSide);
        Device?.TriggerVibration(ClickVibrationConfigurations);
        button.onClick.Invoke();
    }
}

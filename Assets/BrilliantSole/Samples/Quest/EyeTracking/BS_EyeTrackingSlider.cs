using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using static BS_VibrationWaveformEffect;

[RequireComponent(typeof(Slider))]
public class BS_EyeTrackingSlider : BS_BaseEyeTrackingUIImageComponent
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_EyeTrackingSlider", BS_Logger.LogLevel.Log);

    private Slider slider;

    protected override void Start()
    {
        base.Start();
        image = GetComponentsInChildren<Image>().Last();
        slider = GetComponent<Slider>();
    }

    public BS_SensorRate SensorRate = BS_SensorRate._20ms;
    private readonly BS_SensorType SensorType = BS_SensorType.GameRotation;

    protected override void OnEnable()
    {
        base.OnEnable();

        PitchRange.Reset();

        if (Device == null) { return; }
        Device.OnGameRotation += OnGameRotation;
    }
    protected override void OnDisable()
    {
        base.OnDisable();

        if (Device == null) { return; }
        Device.OnGameRotation -= OnGameRotation;
        if (!gameObject.scene.isLoaded) return;
        Device.ClearSensorRate(SensorType);
    }

    private readonly BS_Range PitchRange = new();
    public bool InvertPitch = false;
    private void OnGameRotation(BS_Device device, Quaternion gameRotation, ulong timestamp)
    {
        var pitch = gameRotation.GetPitch();
        var value = PitchRange.UpdateAndGetNormalization(pitch, false);
        if (InvertPitch)
        {
            value = 1 - value;
        }
        //Logger.Log($"pitch: {pitch}, sliderValue: {value}");
        slider.value = value;
        slider.onValueChanged.Invoke(value);
    }

    public List<BS_VibrationConfiguration> StartSlidingVibrationConfigurations = new()
    {
        new () {
            Locations = BS_VibrationLocationFlag.Front | BS_VibrationLocationFlag.Rear,
            Type = BS_VibrationType.WaveformEffect,
            WaveformEffectSequence = new() {new(StrongClick_100)}
        }
    };
    public List<BS_VibrationConfiguration> StopSlidingVibrationConfigurations = new()
    {
        new () {
            Locations = BS_VibrationLocationFlag.Front | BS_VibrationLocationFlag.Rear,
            Type = BS_VibrationType.WaveformEffect,
            WaveformEffectSequence = new() {new(DoubleClick_100)}
        }
    };

    private bool isSliding = false;
    public bool IsSliding
    {
        get => isSliding;
        private set
        {
            if (value == isSliding) { return; }
            isSliding = value;
            Logger.Log($"updated IsSliding to {IsSliding}");

            if (Device == null) { return; }
            if (IsSliding)
            {
                Device.SetSensorRate(SensorType, SensorRate, false);
                Device.TriggerVibration(StartSlidingVibrationConfigurations);
            }
            else
            {
                Device.ClearSensorRate(SensorType, false);
                Device.TriggerVibration(StopSlidingVibrationConfigurations);
            }
        }
    }

    protected override void OnIsHovered(BS_EyeInteractable eyeInteractable, bool IsHovered)
    {
        base.OnIsHovered(eyeInteractable, IsHovered);
        if (!IsHovered)
        {
            IsSliding = false;
        }
    }

    protected override void OnTap(BS_InsoleSide insoleSide)
    {
        base.OnTap(insoleSide);
        IsSliding = true;
    }
}

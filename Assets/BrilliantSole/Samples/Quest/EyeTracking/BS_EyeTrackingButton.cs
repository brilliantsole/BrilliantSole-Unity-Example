using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static BS_VibrationWaveformEffect;

[RequireComponent(typeof(Button))]
public class BS_EyeTrackingButton : BS_BaseEyeTrackingUIImageComponent, IPointerDownHandler, IPointerUpHandler
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_EyeTrackingButton");

    private Button button;

    public Color PressedColor = Color.yellow;

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

    protected override void SetColor(Color color)
    {
        var colors = button.colors;
        colors.highlightedColor = color;
        colors.selectedColor = color;
        colors.pressedColor = color;
        colors.normalColor = color;
        button.colors = colors;
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnPointerDown();
    }
    public void OnPointerDown()
    {
        SetColor(PressedColor);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        OnPointerUp();
    }
    public void OnPointerUp()
    {
        SetColor(IsHovered ? HoverColor : DefaultColor);
    }
}

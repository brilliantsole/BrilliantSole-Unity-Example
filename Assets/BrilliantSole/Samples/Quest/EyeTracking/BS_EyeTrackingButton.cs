using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
[RequireComponent(typeof(BS_EyeInteractable))]
public class BS_EyeTrackingButton : BS_BaseEyeTrackingUIComponent
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_EyeTrackingButton", BS_Logger.LogLevel.Log);

    private Image image;
    public Color HoverColor = Color.yellow;

    protected override void Start()
    {
        base.Start();
        image = GetComponent<Image>();
    }

    protected override void OnIsHovered(BS_EyeInteractable eyeInteractable, bool IsHovered)
    {
        base.OnIsHovered(eyeInteractable, IsHovered);
        image.color = IsHovered ? HoverColor : Color.white;
    }
}

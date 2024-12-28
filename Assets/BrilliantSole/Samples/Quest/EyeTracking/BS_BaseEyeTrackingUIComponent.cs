using UnityEngine;

[RequireComponent(typeof(BS_EyeInteractable))]
public abstract class BS_BaseEyeTrackingUIComponent : MonoBehaviour
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseEyeTrackingUIComponent", BS_Logger.LogLevel.Log);

    protected BS_EyeInteractable eyeInteractable;
    protected virtual void Start()
    {
        eyeInteractable = GetComponent<BS_EyeInteractable>();
        eyeInteractable.OnHover += OnHover;
        eyeInteractable.OnUnhover += OnUnHover;
        eyeInteractable.OnIsHovered += OnIsHovered;
    }

    protected virtual void OnHover(BS_EyeInteractable eyeInteractable)
    {
        Logger.Log("OnHover");
    }
    protected virtual void OnUnHover(BS_EyeInteractable eyeInteractable)
    {
        Logger.Log("OnUnHover");
    }
    protected virtual void OnIsHovered(BS_EyeInteractable eyeInteractable, bool IsHovered)
    {
        Logger.Log($"IsHovered? {IsHovered}");
    }
}

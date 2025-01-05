using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BS_BaseEyeTrackingUIImageComponent : BS_BaseEyeTrackingUIComponent, IPointerEnterHandler, IPointerExitHandler
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseEyeTrackingUIImageComponent");

    protected Image image;
    public Color HoverColor = Color.yellow;

    protected override void Start()
    {
        base.Start();
        image = GetComponent<Image>();
    }

    protected virtual void OnEnable()
    {
        DevicePair.OnDeviceTfliteClassification += OnDeviceTfliteClassification;
    }
    protected virtual void OnDisable()
    {
        DevicePair.OnDeviceTfliteClassification -= OnDeviceTfliteClassification;
        if (!gameObject.scene.isLoaded) return;
    }

    private void OnDeviceTfliteClassification(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, string classification, float value, ulong timestamp)
    {
        if (classification == "tap" && IsHovered && insoleSide == InsoleSide)
        {
            OnTap(insoleSide);
        }
    }

    protected virtual void OnTap(BS_InsoleSide insoleSide)
    {
        Logger.Log("Tap");
    }

    protected override void OnIsHovered(BS_EyeInteractable eyeInteractable, bool IsHovered)
    {
        base.OnIsHovered(eyeInteractable, IsHovered);
        image.color = IsHovered ? HoverColor : Color.white;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
#if UNITY_EDITOR
        eyeInteractable.SetIsHovered(true);
        image.color = HoverColor;
#else
        if (eventData.pointerId == BS_EyeTrackingCanvas.PointerId)
        {
            eyeInteractable.SetIsHovered(true);
            image.color = HoverColor;
        }
#endif
    }

    public void OnPointerExit(PointerEventData eventData)
    {

#if UNITY_EDITOR
        eyeInteractable.SetIsHovered(false);
        image.color = Color.white;
#else
        if (eventData.pointerId == BS_EyeTrackingCanvas.PointerId)
        {
            eyeInteractable.SetIsHovered(false);
            image.color = Color.white;
        }
#endif
    }
}

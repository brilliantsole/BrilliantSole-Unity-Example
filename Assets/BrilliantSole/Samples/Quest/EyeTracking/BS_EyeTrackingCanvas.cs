using UnityEngine;

public class BS_EyeTrackingCanvas : BS_BaseEyeTrackingUIComponent
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseEyeTrackingUIComponent", BS_Logger.LogLevel.Log);

    public GameObject Cursor;
    protected override void OnIsHovered(BS_EyeInteractable eyeInteractable, bool IsHovered)
    {
        base.OnIsHovered(eyeInteractable, IsHovered);
        Cursor.SetActive(IsHovered);
    }
    void Update()
    {
        if (!eyeInteractable.IsHovered) { return; }
        Logger.Log("updating cursor position");
        Cursor.transform.position = Vector3.Lerp(Cursor.transform.position, eyeInteractable.hitPoint, 0.7f);
    }
}

using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class BS_EyeTrackingCanvas : BS_BaseEyeTrackingUIComponent
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_BaseEyeTrackingUIComponent");

    public GameObject Cursor;
    public Canvas targetCanvas;
    protected override void OnIsHovered(BS_EyeInteractable eyeInteractable, bool IsHovered)
    {
        base.OnIsHovered(eyeInteractable, IsHovered);
        Cursor.SetActive(IsHovered);
    }
    void Update()
    {
        if (!eyeInteractable.IsHovered) { return; }
        //Logger.Log("updating cursor position");
        //Cursor.transform.position = Vector3.Lerp(Cursor.transform.position, eyeInteractable.hitPoint, 0.7f);
        var position = Vector3.Lerp(eyeInteractable.leftHitPoint, eyeInteractable.rightHitPoint, 0.5f);
        Cursor.transform.position = position;
        //Cursor.transform.position = eyeInteractable.hitPoint;

        SimulatePointerMove(position);
    }

    public Camera vrCamera;

    static public int PointerId = 1995;

    private readonly HashSet<GameObject> hoveredGameObjects = new();
    public void SimulatePointerMove(Vector3 globalPosition)
    {
        if (targetCanvas == null || vrCamera == null)
        {
            Debug.LogError("Target Canvas or VR Camera is not set.");
            return;
        }

        if (targetCanvas.renderMode != RenderMode.WorldSpace)
        {
            Debug.LogError("Canvas must be in World Space mode.");
            return;
        }

        // Convert world position to screen position relative to the VR camera
        Vector2 screenPoint = targetCanvas.worldCamera.WorldToScreenPoint(globalPosition);
        //RectTransformUtility.ScreenPointToLocalPointInRectangle(targetCanvas.transform as RectTransform, screenPoint, targetCanvas.worldCamera, out Vector2 localPoint);

        // Create PointerEventData
        PointerEventData pointerData = new(EventSystem.current)
        {
            position = screenPoint,
            pointerId = PointerId
        };

        // Raycast to find UI elements under the pointer
        var raycastResults = new List<RaycastResult>();
        EventSystem.current.RaycastAll(pointerData, raycastResults);

        HashSet<GameObject> hoveredGameObjectsToRemove = new(hoveredGameObjects);
        HashSet<GameObject> enteredGameObjects = new();

        if (raycastResults.Count > 0)
        {
            //Debug.Log($"raycast {raycastResults.Count} objects");
            foreach (var result in raycastResults)
            {
                //Debug.Log($"Pointer moved over: {result.gameObject.name}");
                //ExecuteEvents.Execute(result.gameObject, pointerData, ExecuteEvents.pointerMoveHandler);
                var isNew = hoveredGameObjects.Add(result.gameObject);
                if (isNew)
                {
                    enteredGameObjects.Add(result.gameObject);
                }
                hoveredGameObjectsToRemove.Remove(result.gameObject);
            }
        }
        else
        {
            //Debug.Log("No UI element was hit by the pointer move.");
        }

        foreach (var _gameObject in hoveredGameObjectsToRemove)
        {
            hoveredGameObjects.Remove(_gameObject);
            ExecuteEvents.Execute(_gameObject, pointerData, ExecuteEvents.pointerExitHandler);
        }
        foreach (var _gameObject in enteredGameObjects)
        {
            ExecuteEvents.Execute(_gameObject, pointerData, ExecuteEvents.pointerEnterHandler);
        }
    }
}

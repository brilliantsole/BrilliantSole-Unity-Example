// based on https://www.youtube.com/watch?v=ZoySn7QlMfQ

using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Events;

[RequireComponent(typeof(LineRenderer))]
public class BS_EyeTrackingRay : MonoBehaviour
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_EyeTrackingRay", BS_Logger.LogLevel.Log);

    [SerializeField]
    private float rayDistance = 1.0f;

    [SerializeField]
    private float rayWidth = 0.01f;

    [SerializeField]
    private LayerMask layersToInclude;

    [SerializeField]
    private Color rayColorDefaultState = Color.yellow;
    [SerializeField]
    private Color rayColorHoverState = Color.red;

    private LineRenderer lineRenderer;

    private readonly List<BS_EyeInteractable> eyeInteractables = new();

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        SetupRay();
    }

    void SetupRay()
    {
        lineRenderer.useWorldSpace = false;
        lineRenderer.positionCount = 2;
        lineRenderer.startWidth = rayWidth;
        lineRenderer.endWidth = 2;
        lineRenderer.startColor = rayColorDefaultState;
        lineRenderer.endColor = rayColorDefaultState;
        lineRenderer.SetPosition(0, transform.position);
        lineRenderer.SetPosition(1, new Vector3(transform.position.x, transform.position.y, transform.position.z + rayDistance));
    }

    readonly RaycastHit[] hits = new RaycastHit[10];
    private void FixedUpdate()
    {
        Vector3 raycastDirection = transform.TransformDirection(Vector3.forward) * rayDistance;

        var hitCount = Physics.RaycastNonAlloc(transform.position, raycastDirection, hits, 5.0f, layersToInclude);
        if (hitCount > 0)
        {
            UnSelect();
            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = hits[i];

                lineRenderer.startColor = rayColorHoverState;
                lineRenderer.endColor = rayColorHoverState;

                var eyeInteractable = hit.transform.GetComponent<BS_EyeInteractable>();
                if (eyeInteractable && !eyeInteractable.ShouldIgnore)
                {
                    eyeInteractables.Add(eyeInteractable);
                    eyeInteractable.IsHovered = true;
                    eyeInteractable.hitPoint.Set(hit.point.x, hit.point.y, hit.point.z);
                }
            }
        }
        else
        {
            lineRenderer.startColor = rayColorDefaultState;
            lineRenderer.endColor = rayColorDefaultState;
            UnSelect(true);
        }
    }

    void UnSelect(bool clear = false)
    {
        foreach (var eyeInteractable in eyeInteractables)
        {
            eyeInteractable.IsHovered = false;
        }
        if (clear)
        {
            eyeInteractables.Clear();
        }
    }
}
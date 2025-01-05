// based on https://www.youtube.com/watch?v=ZoySn7QlMfQ

using UnityEngine;
using System.Collections.Generic;
using System.Linq;

[RequireComponent(typeof(LineRenderer))]
public class BS_EyeTrackingRay : MonoBehaviour
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_EyeTrackingRay");

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

    public enum Side
    {
        Left,
        Right
    };
    public Side side;

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

    public GameObject HitPointPrefab;
    public Color HitPointColor;
    private readonly Dictionary<BS_EyeInteractable, GameObject> HitPointGameObjects = new();

    readonly RaycastHit[] hits = new RaycastHit[10];
    private void FixedUpdate()
    {
        Vector3 raycastDirection = transform.TransformDirection(Vector3.forward) * rayDistance;
        List<BS_EyeInteractable> eyeInteractablesToUnhover = eyeInteractables.Where(eyeInteractable => eyeInteractable.IsHovered).ToList();

        var hitCount = Physics.RaycastNonAlloc(transform.position, raycastDirection, hits, 5.0f, layersToInclude);
        if (hitCount > 0)
        {
            Logger.Log($"hit {hitCount} interactables");
            eyeInteractables.Clear();
            for (int i = 0; i < hitCount; i++)
            {
                RaycastHit hit = hits[i];

                lineRenderer.startColor = rayColorHoverState;
                lineRenderer.endColor = rayColorHoverState;

                var eyeInteractable = hit.transform.GetComponent<BS_EyeInteractable>();
                if (eyeInteractable && !eyeInteractable.ShouldIgnore)
                {
                    eyeInteractables.Add(eyeInteractable);
                    eyeInteractablesToUnhover.Remove(eyeInteractable);
                    Logger.Log($"hovered on {eyeInteractable.gameObject.name}");
                    eyeInteractable.SetIsHovered(true);
                    eyeInteractable.hitPoint = hit.point;
                    if (side == Side.Left)
                    {
                        eyeInteractable.leftHitPoint = hit.point;
                    }
                    else
                    {
                        eyeInteractable.rightHitPoint = hit.point;
                    }

                    if (showMarkers)
                    {
                        if (HitPointGameObjects.TryGetValue(eyeInteractable, out var hitPointGameObject))
                        {
                            hitPointGameObject.transform.position = hit.point;
                            hitPointGameObject.SetActive(true);
                        }
                        else
                        {
                            hitPointGameObject = Instantiate(HitPointPrefab, hit.point, Quaternion.identity);
                            hitPointGameObject.GetComponent<Renderer>().material.color = HitPointColor;
                            HitPointGameObjects.Add(eyeInteractable, hitPointGameObject);
                        }
                    }
                }
            }
        }
        else
        {
            lineRenderer.startColor = rayColorDefaultState;
            lineRenderer.endColor = rayColorDefaultState;
            eyeInteractables.Clear();
        }

        foreach (var eyeInteractable in eyeInteractablesToUnhover)
        {
            Logger.Log($"unhovering {eyeInteractable.gameObject.name}");
            eyeInteractable.SetIsHovered(false);

            if (showMarkers)
            {
                if (HitPointGameObjects.TryGetValue(eyeInteractable, out var hitPointGameObject))
                {
                    hitPointGameObject.SetActive(false);
                }
            }
        }
    }

    private bool showMarkers = false;
}
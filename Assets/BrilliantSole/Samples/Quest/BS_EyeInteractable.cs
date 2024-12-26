// from https://www.youtube.com/watch?v=ZoySn7QlMfQ

using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class BS_EyeInteractable : MonoBehaviour
{
    [SerializeField]
    private UnityEvent<GameObject> OnObjectHover;

    [SerializeField]
    private UnityEvent<GameObject> OnObjectUnHover;

    [SerializeField]
    private Material OnHoverActiveMaterial;

    [SerializeField]
    private Material OnHoverInactiveMaterial;

    private MeshRenderer meshRenderer;
    public BoxCollider _collider;

    public Vector3 hitPoint;

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        _collider = GetComponent<BoxCollider>();
    }

    public bool ShouldIgnore = false;

    private bool _IsHovered = false;
    public bool IsHovered
    {
        get
        {
            return _IsHovered;
        }
        set
        {
            if (_IsHovered != value)
            {
                _IsHovered = value;
                if (_IsHovered)
                {
                    if (meshRenderer && OnHoverActiveMaterial)
                    {
                        meshRenderer.material = OnHoverActiveMaterial;
                    }
                    OnObjectHover.Invoke(gameObject);
                }
                else
                {
                    if (meshRenderer && OnHoverInactiveMaterial)
                    {
                        meshRenderer.material = OnHoverInactiveMaterial;
                    }
                    OnObjectUnHover.Invoke(gameObject);
                }
            }
        }
    }
}
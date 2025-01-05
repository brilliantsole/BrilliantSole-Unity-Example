// based on https://www.youtube.com/watch?v=ZoySn7QlMfQ

using System;
using Unity.VisualScripting;
using UnityEngine;

[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(Rigidbody))]
public class BS_EyeInteractable : MonoBehaviour
{
    private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_EyeInteractable");

    public Action<BS_EyeInteractable> OnHover;
    public Action<BS_EyeInteractable> OnUnhover;
    public Action<BS_EyeInteractable, bool> OnIsHovered;

    [HideInInspector]
    public BoxCollider _collider;

    [HideInInspector]
    public Vector3 hitPoint;

    [HideInInspector]
    public Vector3 leftHitPoint;
    [HideInInspector]
    public Vector3 rightHitPoint;

    void Start()
    {
        _collider = GetComponent<BoxCollider>();
    }

    public bool ShouldIgnore = false;

    public void SetIsHovered(bool newIsHovered, bool invoke = true)
    {
        if (invoke)
        {
            IsHovered = newIsHovered;
        }
        else
        {
            _IsHovered = newIsHovered;
        }
    }

    private bool _IsHovered = false;
    public bool IsHovered
    {
        get
        {
            return _IsHovered;
        }
        private set
        {
            if (_IsHovered != value)
            {
                _IsHovered = value;
                Logger.Log($"\"{name}\" IsHovered updated to {IsHovered}");
                if (_IsHovered)
                {
                    OnHover?.Invoke(this);
                }
                else
                {
                    OnUnhover?.Invoke(this);
                }
                OnIsHovered?.Invoke(this, IsHovered);
            }
        }
    }
}
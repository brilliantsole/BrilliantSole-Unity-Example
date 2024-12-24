using System;
using UnityEngine;

public class BS_ColliderBroadcaster : MonoBehaviour
{
    public Action<GameObject, Collider> OnCollider;

    private void Awake()
    {
        BS_Collider[] colliders = GetComponentsInChildren<BS_Collider>();

        foreach (var collider in colliders)
        {
            collider.OnColliderEvent += HandleChildColliderEvent;
        }
    }

    private void HandleChildColliderEvent(Collider other)
    {
        //Debug.Log($"Collision received from child {other.gameObject.name}");
        OnCollider?.Invoke(gameObject, other);
    }
}

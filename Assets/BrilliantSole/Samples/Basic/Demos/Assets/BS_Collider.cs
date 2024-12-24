using System;
using UnityEngine;

public class BS_Collider : MonoBehaviour
{
    public Action<Collider> OnColliderEvent;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Collision received from: {other.gameObject.name}");
        OnColliderEvent?.Invoke(other);
    }
}
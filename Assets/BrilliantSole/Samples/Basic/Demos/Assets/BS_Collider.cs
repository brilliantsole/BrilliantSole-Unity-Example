using System;
using UnityEngine;

public class BS_Collider : MonoBehaviour
{
    public delegate void OnColliderDelegate(Collider collider);
    public OnColliderDelegate OnColliderEvent;

    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log($"Collision received from: {other.gameObject.name}");
        OnColliderEvent?.Invoke(other);
    }
}
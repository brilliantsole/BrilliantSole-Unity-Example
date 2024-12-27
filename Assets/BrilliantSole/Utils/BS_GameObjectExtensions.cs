using UnityEngine;

public static class BS_GameObjectExtensions
{
    public static T GetAncestorWithComponent<T>(this GameObject obj) where T : Component
    {
        Transform current = obj.transform;

        while (current != null)
        {
            if (current.TryGetComponent<T>(out var component))
            {
                return component;
            }

            current = current.parent;
        }

        return null;
    }
}

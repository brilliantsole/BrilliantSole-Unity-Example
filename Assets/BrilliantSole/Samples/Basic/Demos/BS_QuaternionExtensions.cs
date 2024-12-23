using UnityEngine;

public static class BS_QuaternionExtensions
{
    public static float GetPitch(this Quaternion q)
    {
        float sinP = 2 * (q.w * q.x + q.y * q.z);
        float cosP = 1 - 2 * (q.x * q.x + q.y * q.y);

        return Mathf.Atan2(sinP, cosP) * Mathf.Rad2Deg;
    }
}
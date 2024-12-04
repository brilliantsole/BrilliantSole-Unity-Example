using UnityEngine;

public class BS_BasicMotionDemo : MonoBehaviour
{
    public GameObject LeftInsole;
    public GameObject RightInsole;

    private BS_DevicePair DevicePair => BS_DevicePair.Instance;

    private void OnEnable()
    {
        setActive(true);

        DevicePair.OnDeviceGameRotation += OnDeviceQuaternion;
        DevicePair.OnDeviceRotation += OnDeviceQuaternion;

        DevicePair.OnDeviceGyroscope += OnDeviceEulerAngles;
        DevicePair.OnDeviceOrientation += OnDeviceEulerAngles;

        DevicePair.OnDeviceLinearAcceleration += OnDevicePosition;
    }
    private void OnDisable()
    {
        setActive(false);

        DevicePair.OnDeviceGameRotation -= OnDeviceQuaternion;
        DevicePair.OnDeviceRotation -= OnDeviceQuaternion;

        DevicePair.OnDeviceGyroscope -= OnDeviceEulerAngles;
        DevicePair.OnDeviceOrientation -= OnDeviceEulerAngles;

        DevicePair.OnDeviceLinearAcceleration -= OnDevicePosition;

    }

    private void setActive(bool active)
    {
        LeftInsole?.SetActive(active);
        RightInsole?.SetActive(active);
    }

    private void OnDeviceQuaternion(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, Quaternion quaternion, ulong timestamp)
    {
        // FILL
    }
    private void OnDeviceEulerAngles(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, Vector3 eulerAngles, ulong timestamp)
    {
        // FILL
    }
    private void OnDevicePosition(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, Vector3 position, ulong timestamp)
    {
        // FILL
    }
}

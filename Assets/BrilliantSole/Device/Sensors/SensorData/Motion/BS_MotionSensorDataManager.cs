using System;
using System.Collections.Generic;
using UnityEngine;
using static BS_SensorType;

public class BS_MotionSensorDataManager : BS_BaseSensorDataManager
{
  private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_MotionSensorDataManager");

  private readonly HashSet<BS_SensorType> sensorTypes = new(){
        Acceleration,
        Gravity,
        LinearAcceleration,
        BS_SensorType.Gyroscope,
        Magnetometer,
        GameRotation,
        Rotation,

        Orientation,
        Activity,
        StepCount,
        StepDetection,
        BS_SensorType.DeviceOrientation,
      };
  protected override HashSet<BS_SensorType> SensorTypes => sensorTypes;

  public override void ParseSensorDataMessage(BS_SensorType sensorType, in byte[] data, in ulong timestamp, in float scalar)
  {
    base.ParseSensorDataMessage(sensorType, data, timestamp, scalar);
    switch (sensorType)
    {
      case Acceleration:
      case Gravity:
      case LinearAcceleration:
      case BS_SensorType.Gyroscope:
      case Magnetometer:
        ParseVector(sensorType, data, timestamp, scalar);
        break;
      case GameRotation:
      case Rotation:
        ParseQuaternion(sensorType, data, timestamp, scalar);
        break;
      case Orientation:
        ParseOrientation(data, timestamp, scalar);
        break;
      case Activity:
        ParseActivity(data, timestamp);
        break;
      case StepCount:
        ParseStepCount(data, timestamp);
        break;
      case StepDetection:
        ParseStepDetection(data, timestamp);
        break;
      case BS_SensorType.DeviceOrientation:
        ParseDeviceOrientation(data, timestamp);
        break;
      default:
        throw new ArgumentException($"uncaught sensorType {sensorType}");
    }
  }

  public Action<Vector3, ulong> OnAcceleration;
  public Action<Vector3, ulong> OnGravity;
  public Action<Vector3, ulong> OnLinearAcceleration;
  public Action<Vector3, ulong> OnGyroscope;
  public Action<Vector3, ulong> OnMagnetometer;

  private void ParseVector(BS_SensorType sensorType, in byte[] data, in ulong timestamp, in float scalar)
  {
    Logger.Log($"parsing {sensorType} vector");
    var x = BS_ByteUtils.ParseNumber<short>(data, 0, true);
    var y = BS_ByteUtils.ParseNumber<short>(data, 2, true);
    var z = BS_ByteUtils.ParseNumber<short>(data, 4, true);
    Logger.Log($"raw vector: [{x}, {y}, {z}]");

    Vector3 vector3 = new(x, y, -z);
    vector3 *= scalar;
    Logger.Log($"{sensorType}: {vector3}");

    switch (sensorType)
    {
      case Acceleration:
        OnAcceleration?.Invoke(vector3, timestamp);
        break;
      case Gravity:
        OnGravity?.Invoke(vector3, timestamp);
        break;
      case LinearAcceleration:
        OnLinearAcceleration?.Invoke(vector3, timestamp);
        break;
      case BS_SensorType.Gyroscope:
        Vector3 eulerAngles = new(-vector3.x, -vector3.y, -vector3.z);
        OnGyroscope?.Invoke(eulerAngles, timestamp);
        break;
      case Magnetometer:
        OnMagnetometer?.Invoke(vector3, timestamp);
        break;
      default:
        throw new ArgumentException($"uncaught sensorType {sensorType}");
    }
  }

  public Action<Quaternion, ulong> OnGameRotation;
  public Action<Quaternion, ulong> OnRotation;
  private void ParseQuaternion(BS_SensorType sensorType, in byte[] data, in ulong timestamp, in float scalar)
  {
    Logger.Log($"parsing {sensorType} quaternion");
    var x = BS_ByteUtils.ParseNumber<short>(data, 0, true) * scalar;
    var y = BS_ByteUtils.ParseNumber<short>(data, 2, true) * scalar;
    var z = BS_ByteUtils.ParseNumber<short>(data, 4, true) * scalar;
    var w = BS_ByteUtils.ParseNumber<short>(data, 6, true) * scalar;
    Logger.Log($"raw quaternion: [{x}, {y}, {z}, {w}]");

    Quaternion quaternion = new(-x, -y, z, w);
    Logger.Log($"{sensorType}: {quaternion}");

    switch (sensorType)
    {
      case GameRotation:
        OnGameRotation?.Invoke(quaternion, timestamp);
        break;
      case Rotation:
        OnRotation?.Invoke(quaternion, timestamp);
        break;
      default:
        throw new ArgumentException($"uncaught sensorType {sensorType}");
    }
  }

  public Action<Vector3, ulong> OnOrientation;
  private void ParseOrientation(in byte[] data, in ulong timestamp, in float scalar)
  {
    Logger.Log($"parsing orientation");
    var yaw = BS_ByteUtils.ParseNumber<short>(data, 0, true);
    var pitch = BS_ByteUtils.ParseNumber<short>(data, 2, true);
    var roll = BS_ByteUtils.ParseNumber<short>(data, 4, true);
    Logger.Log($"orientation: yaw: {yaw}, pitch: {pitch}, roll: {roll}");

    Vector3 vector3 = new(pitch, yaw, roll);
    vector3 *= scalar;

    OnOrientation?.Invoke(vector3, timestamp);
  }

  public Action<HashSet<BS_Activity>, ulong> OnActivity;

  private void ParseActivity(in byte[] data, in ulong timestamp)
  {
    var activityBitfield = data[0];
    HashSet<BS_Activity> Activity = new();
    foreach (BS_Activity activityType in Enum.GetValues(typeof(BS_Activity)))
    {
      if ((activityBitfield & (byte)activityType) != 0)
      {
        Logger.Log($"detected activityType {activityType}");
        Activity.Add(activityType);
      }
    }
    OnActivity?.Invoke(Activity, timestamp);
  }

  public Action<ulong> OnStepDetection;
  private void ParseStepDetection(in byte[] data, in ulong timestamp)
  {
    Logger.Log("Step Detected");
    OnStepDetection?.Invoke(timestamp);
  }

  public Action<uint, ulong> OnStepCount;
  private void ParseStepCount(in byte[] data, in ulong timestamp)
  {
    var stepCount = BS_ByteUtils.ParseNumber<uint>(data, isLittleEndian: true);
    Logger.Log($"stepCount: {stepCount}");
    OnStepCount?.Invoke(stepCount, timestamp);
  }

  public Action<BS_DeviceOrientation, ulong> OnDeviceOrientation;
  private void ParseDeviceOrientation(in byte[] data, in ulong timestamp)
  {
    var deviceOrientation = (BS_DeviceOrientation)data[0];
    Logger.Log($"deviceOrientation: {deviceOrientation}");
    OnDeviceOrientation?.Invoke(deviceOrientation, timestamp);
  }
}

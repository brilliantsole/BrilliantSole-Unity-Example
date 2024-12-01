using System.Collections.Generic;
using static BS_SensorType;

public class BS_MotionSensorDataManager : BS_BaseSensorDataManager
{
  private static readonly BS_Logger Logger = BS_Logger.GetLogger("BS_MotionSensorDataManager", BS_Logger.LogLevel.Log);

  private readonly HashSet<BS_SensorType> sensorTypes = new(){
        Acceleration,
        Gravity,
        LinearAcceleration,
        Gyroscope,
        Magnetometer,
        GameRotation,
        Rotation,

        Orientation,
        Activity,
        StepCount,
        StepDetection,
        DeviceOrientation,
      };
  protected override HashSet<BS_SensorType> SensorTypes => sensorTypes;

  public override void ParseSensorDataMessage(BS_SensorType sensorType, in byte[] data, in ulong timestamp, in float scalar)
  {
    base.ParseSensorDataMessage(sensorType, data, timestamp, scalar);
    switch (sensorType)
    {
      // FILL
    }
  }
}

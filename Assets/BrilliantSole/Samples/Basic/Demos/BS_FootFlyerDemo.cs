using System;
using UnityEngine;

public class BS_FootFlyerDemo : BS_BaseDemo
{
    public BS_InsoleSide InsoleSide = BS_InsoleSide.Right;
    public BS_SensorRate SensorRate = BS_SensorRate._20ms;


    public Vector3 Size = new(2f, 1f, 0f);

    protected override void Start()
    {
        base.Start();
        Player.transform.localPosition -= Size / 2;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Device?.SetSensorRate(BS_SensorType.GameRotation, SensorRate);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        if (!gameObject.scene.isLoaded) return;
        Device?.ClearSensorRate(BS_SensorType.GameRotation);
    }
    protected override void MoveObstacles()
    {
        foreach (var obstacle in Obstacles)
        {
            obstacle.transform.Translate(Speed * Time.deltaTime * Vector3.left);
        }
    }

    protected override void CheckObstaclePositions()
    {
        base.CheckObstaclePositions();
        foreach (var obstacle in Obstacles.ToArray())
        {
            if (obstacle.transform.localPosition.x < -((Size.x / 2) + 0.1))
            {
                RemoveObstacle(obstacle);
            }
        }
    }
    private float lastTimeObstacleSpawned = 0;
    [Range(0.5f, 2.0f)]
    public float TimeBetweenObstacleSpawn = 1.0f;
    protected override void CheckObstacleSpawn()
    {
        base.CheckObstacleSpawn();
        if (runtime - lastTimeObstacleSpawned >= TimeBetweenObstacleSpawn)
        {
            SpawnObstacle();
        }
    }
    [Range(0.0f, 1.0f)]
    public float EnemyObstacleProbability = 0.5f;
    private void SpawnObstacle()
    {
        //Debug.Log("Spawning obstacle...");

        var isObstacleEnemy = UnityEngine.Random.value < EnemyObstacleProbability;
        var obstaclePrefab = isObstacleEnemy ? EnemyPrefab : CollectablePrefab;

        //Debug.Log($"spawning {(isObstacleEnemy ? "enemy" : "collectable")}");

        var obstacle = Instantiate(obstaclePrefab, Scene.transform.position, Quaternion.identity, Scene.transform);

        Vector3 position = new(Size.x / 2, Size.y * (UnityEngine.Random.value - 0.5f), 0);
        obstacle.transform.localPosition += position;

        Obstacles.Add(obstacle);

        lastTimeObstacleSpawned = runtime;
    }

    private void CheckMouse()
    {
        if (!Input.mousePresent) { return; }
        var scroll = Input.mouseScrollDelta;
        SetPlayerHeightOffset(scroll.y);
    }

    private void SetPlayerHeight(float newHeight)
    {
        var position = Player.transform.localPosition;
        position.y = Math.Clamp(newHeight, -Size.y / 2, Size.y / 2);
        //Debug.Log($"updating Player height to {position.y}");
        Player.transform.localPosition = position;
    }
    private void SetPlayerHeightOffset(float heightOffset)
    {
        var position = Player.transform.localPosition;
        SetPlayerHeight(position.y + heightOffset);
    }
    private void SetPlayerHeightNormalized(float normalizedHeight)
    {
        SetPlayerHeight(Size.y * (normalizedHeight - 0.5f));
    }

    private readonly BS_Range PitchRange = new();
    public bool InvertPitch = false;
    protected override void OnDeviceQuaternion(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, Quaternion quaternion, ulong timestamp)
    {
        base.OnDeviceQuaternion(devicePair, insoleSide, device, quaternion, timestamp);
        if (insoleSide != InsoleSide) { return; }
        var pitch = quaternion.GetPitch();
        pitch += 2.0f * Mathf.PI;
        //Debug.Log($"pitch: {pitch}");
        var normalizedHeight = PitchRange.UpdateAndGetNormalization(pitch, false);
        if (InvertPitch)
        {
            normalizedHeight = 1 - normalizedHeight;
        }
        //Debug.Log($"normalizedHeight: {normalizedHeight}");
        SetPlayerHeightNormalized(normalizedHeight);
    }
    public override void Calibrate()
    {
        base.Calibrate();
        PitchRange.Reset();
    }
    private BS_Device Device => DevicePair.Devices[InsoleSide];
    private bool IsInsoleConnected => Device?.IsConnected == true;
    protected override void Update()
    {
        if (!IsInsoleConnected) { CheckMouse(); }
        base.Update();
    }

    protected override void OnEnemyCollision(GameObject obstacle)
    {
        base.OnEnemyCollision(obstacle);
        Device?.TriggerVibration(EnemyVibrationConfigurations);
    }
    protected override void OnCollectableCollision(GameObject obstacle)
    {
        base.OnCollectableCollision(obstacle);
        Device?.TriggerVibration(CollectableVibrationConfigurations);
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BS_FootFlyerDemo : BS_BaseDemo
{
    public BS_InsoleSide InsoleSide = BS_InsoleSide.Right;
    public BS_SensorRate SensorRate = BS_SensorRate._20ms;

    private GameObject Player;
    public Vector3 Size = new(2f, 1f, 0f);

    protected override void Start()
    {
        base.Start();
        Player = Instantiate(PlayerPrefab, Scene.transform.position, Quaternion.identity, Scene.transform);
        Player.transform.localPosition -= Size / 2;
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        DevicePair.Devices[InsoleSide]?.SetSensorRate(BS_SensorType.GameRotation, SensorRate);
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        if (!gameObject.scene.isLoaded) return;
        DevicePair.Devices[InsoleSide]?.ClearSensorRate(BS_SensorType.GameRotation);
    }

    private readonly List<GameObject> Obstacles = new();
    private void ClearObstacles()
    {
        foreach (var obstacle in Obstacles.ToList())
        {
            RemoveObstacle(obstacle);
        }
    }
    [Range(0.1f, 10.0f)]
    public float Speed = 1.0f;
    private void MoveObstacles()
    {
        foreach (var obstacle in Obstacles)
        {
            obstacle.transform.Translate(Speed * Time.deltaTime * Vector3.left);

        }
    }
    public LayerMask CollisionLayer;
    private readonly Collider[] colliders = new Collider[10];
    private void CheckObstacleCollisions()
    {
        var colliderCount = Physics.OverlapSphereNonAlloc(Player.transform.position, 0.15f, colliders, CollisionLayer);
        if (colliderCount > 0)
        {
            //Debug.Log($"collided with {colliderCount} obstacles");
            for (int i = 0; i < colliderCount; i++)
            {
                var obstacle = colliders[i].gameObject.transform.parent.gameObject;
                OnObstacleCollision(obstacle);
            }
        }
    }
    private void OnObstacleCollision(GameObject obstacle)
    {
        //Debug.Log($"collided with {obstacle.name}");
        if (obstacle.name.Contains(CollectablePrefab.name))
        {
            //Debug.Log("collided with collectable");
            OnCollectableCollision(obstacle);
        }
        else if (obstacle.name.Contains(EnemyPrefab.name))
        {
            //Debug.Log("collided with enemy");
            OnEnemyCollision(obstacle);
        }
        else
        {
            Debug.LogError($"uncaught collided obstacle {obstacle.name}");
        }
        RemoveObstacle(obstacle);
    }
    public int CollectableScore = 100;
    private void OnCollectableCollision(GameObject obstacle)
    {
        Score += CollectableScore;
    }
    public float EnemyDamage = 20.0f;
    private void OnEnemyCollision(GameObject obstacle)
    {
        Health -= EnemyDamage;
    }
    private void CheckObstaclePositions()
    {
        foreach (var obstacle in Obstacles.ToArray())
        {
            if (obstacle.transform.localPosition.x < -((Size.x / 2) + 0.1))
            {
                RemoveObstacle(obstacle);
            }
        }
    }
    private void RemoveObstacle(GameObject obstacle)
    {
        //Debug.Log("removing obstacle");
        Obstacles.Remove(obstacle);
        Destroy(obstacle);
    }
    private float lastTimeObstacleSpawned = 0;
    private float runtime = 0;
    [Range(0.5f, 2.0f)]
    public float TimeBetweenObstacleSpawn = 1.0f;
    private void CheckObstacleSpawn()
    {
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
    private bool IsInsoleConnected => DevicePair.Devices[InsoleSide]?.IsConnected == true;
    void Update()
    {
        if (!IsInsoleConnected) { CheckMouse(); }
        if (!IsRunning) { return; }
        MoveObstacles();
    }
    void FixedUpdate()
    {
        if (!IsRunning) { return; }
        runtime += Time.deltaTime;
        CheckObstacleSpawn();
        CheckObstacleCollisions();
        CheckObstaclePositions();
    }

    public override void Reset()
    {
        base.Reset();
        ClearObstacles();
    }
}

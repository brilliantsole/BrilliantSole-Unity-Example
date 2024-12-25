using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static BS_InsoleSide;

public class BS_TapTapRevolutionDemo : BS_BaseDemo
{
    public GameObject TapTargetPrefab;
    private readonly Dictionary<BS_InsoleSide, GameObject> TapTargets = new();

    private readonly float xSpacing = 3.0f;

    protected override void Start()
    {
        base.Start();
        Player.SetActive(false);

        foreach (BS_InsoleSide insoleSide in Enum.GetValues(typeof(BS_InsoleSide)))
        {
            var TapTarget = Instantiate(TapTargetPrefab, Scene.transform.position, Quaternion.identity, Scene.transform);
            var localPosition = TapTarget.transform.localPosition;
            var xOffset = Size.x / xSpacing;
            if (insoleSide == Left) { xOffset *= -1.0f; }
            localPosition.x = xOffset;
            TapTarget.transform.localPosition = localPosition;
            TapTargets[insoleSide] = TapTarget;
        }
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        DevicePair.SetTfliteInferencingEnabled(true);
        DevicePair.OnDeviceTfliteClassification += OnDeviceTfliteClassification;
    }
    protected override void OnDisable()
    {
        base.OnDisable();
        if (!gameObject.scene.isLoaded) return;
        DevicePair.SetTfliteInferencingEnabled(false);
        DevicePair.OnDeviceTfliteClassification -= OnDeviceTfliteClassification;
    }

    private void OnDeviceTfliteClassification(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, string classification, float value, ulong timestamp)
    {
        Debug.Log($"{insoleSide} {classification}");
        if (classification == "tap")
        {
            OnTap(insoleSide);
        }
    }

    private void OnTap(BS_InsoleSide insoleSide)
    {
        Debug.Log($"{insoleSide} tap");
        if (PendingObstacles.ContainsKey(insoleSide))
        {
            RemoveObstacle(PendingObstacles[insoleSide]);
            Score += TapScore;
        }
        else
        {
            Health -= MisTapDamage;
        }
    }
    public float MisTapDamage = 10.0f;
    public float TapScore = 100.0f;

    protected override void MoveObstacles()
    {
        foreach (var obstacle in Obstacles)
        {
            obstacle.transform.Translate(Speed * Time.deltaTime * Vector3.down);
        }
    }
    protected override void CheckObstaclePositions()
    {
        base.CheckObstaclePositions();
        foreach (var obstacle in Obstacles.ToArray())
        {
            if (obstacle.transform.localPosition.y < -0.1)
            {
                RemoveObstacle(obstacle);
                Health -= EnemyDamage;
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
    private void SpawnObstacle()
    {
        //Debug.Log("Spawning obstacle...");

        var obstaclePrefab = EnemyPrefab;

        List<BS_InsoleSide> sides = new();
        if (DevicePair.IsHalfConnected)
        {
            sides.Add(UnityEngine.Random.value < 0.5f ? Left : Right);
        }
        else
        {
            var useBoth = UnityEngine.Random.value < 0.33f;
            if (useBoth)
            {
                sides.Add(Left);
                sides.Add(Right);
            }
            else
            {
                sides.Add(UnityEngine.Random.value < 0.5f ? Left : Right);
            }
        }

        foreach (var side in sides)
        {
            Debug.Log($"spawning {side}");

            var obstacle = Instantiate(obstaclePrefab, Scene.transform.position, Quaternion.identity, Scene.transform);

            Vector3 position = new(0, Size.y, 0);
            var xOffset = Size.x / xSpacing;
            if (side == Left) { xOffset *= -1; }
            position.x += xOffset;
            obstacle.transform.localPosition += position;

            AddObstacle(obstacle);
        }

        lastTimeObstacleSpawned = runtime;
    }

    protected override void Update()
    {
        base.Update();
        CheckKeyboardInput();
    }

    private void CheckKeyboardInput()
    {
        if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            OnTap(Left);
        }
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            OnTap(Right);
        }
    }

    private readonly Dictionary<BS_InsoleSide, GameObject> PendingObstacles = new();
    protected override void OnObstacleCollision(GameObject obstacle)
    {
        if (IsObstacleEnemy(obstacle))
        {
            var side = GetObstacleSide(obstacle);
            PendingObstacles[side] = obstacle;
            // FILL - change color
        }
        else
        {
            base.OnObstacleCollision(obstacle);
        }
    }
    private BS_InsoleSide GetObstacleSide(GameObject obstacle) => obstacle.transform.localPosition.x < 0 ? Left : Right;
    protected override void RemoveObstacle(GameObject obstacle)
    {
        if (IsObstacleEnemy(obstacle))
        {
            if (PendingObstacles.ContainsValue(obstacle))
            {
                PendingObstacles.Remove(GetObstacleSide(obstacle));
            }
        }
        base.RemoveObstacle(obstacle);
    }
}

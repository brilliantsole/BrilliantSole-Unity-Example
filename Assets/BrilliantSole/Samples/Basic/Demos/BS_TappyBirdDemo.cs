using System;
using UnityEngine;

public class BS_TappyBirdDemo : BS_BaseDemo
{
    public BS_InsoleSide InsoleSide = BS_InsoleSide.Right;
    private BS_Device Device => DevicePair.Devices.ContainsKey(InsoleSide) ? DevicePair.Devices[InsoleSide] : null;
    private bool IsInsoleConnected => Device?.IsConnected == true;

    protected override void Start()
    {
        base.Start();
        ResetPlayerPosition();
    }

    private void ResetPlayerPosition()
    {
        var playerPosition = Player.transform.localPosition;
        playerPosition.y = Size.y / 2;
        playerPosition.x = -Size.x / 3;
        playerPosition.z = -Size.z / 3;
        Player.transform.localPosition = playerPosition;
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

    protected override void OnIsRunningUpdate()
    {
        base.OnIsRunningUpdate();
        UpdatePlayerRigidBody();
    }
    protected override void OnGameOver()
    {
        base.OnGameOver();
        UpdatePlayerRigidBody();
    }

    public override void Reset()
    {
        base.Reset();
        ResetPlayerPosition();
        HasJumpedAtLeastOnce = false;
        Pitch = 0.0f;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (!IsRunning) { return; }
        CheckPlayerPosition();
    }

    protected override void Update()
    {
        base.Update();
        CheckJumpInput();
        if (IsRunning && HasJumpedAtLeastOnce)
        {
            UpdateScore();
        }
    }

    public float ScoreTimeMultiplier = 10.0f;
    private void UpdateScore()
    {
        Score += Time.deltaTime * ScoreTimeMultiplier;
    }

    private void CheckPlayerPosition()
    {
        if (Player.transform.localPosition.y <= -0.5)
        {
            Debug.Log("Reached bottom");
            Health = 0;
        }
    }

    private bool HasJumpedAtLeastOnce = false;
    [Range(1.0f, 10.0f)]
    public float JumpVelocity = 3.5f;

    private void Jump()
    {
        if (!IsRunning) { return; }

        if (!HasJumpedAtLeastOnce)
        {
            HasJumpedAtLeastOnce = true;
            UpdatePlayerRigidBody();
        }

        var linearVelocity = PlayerRigidBody.linearVelocity;
        linearVelocity.y = JumpVelocity;
        PlayerRigidBody.linearVelocity = linearVelocity;
    }
    private void CheckJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsRunning)
            {
                Jump();
            }
            else if (IsGameOver)
            {
                ToggleIsRunning();
            }
        }
    }

    public float PitchThreshold = 0.0f;
    public bool InvertPitch = false;
    private float Pitch = 0.0f;
    protected override void OnDeviceQuaternion(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, Quaternion quaternion, ulong timestamp)
    {
        base.OnDeviceQuaternion(devicePair, insoleSide, device, quaternion, timestamp);
        if (insoleSide != InsoleSide) { return; }

        var latestPitch = quaternion.GetPitch();

        var didLatestPitchExceedThreshold = DoesPitchExceedThreshold(latestPitch);
        var shouldJump = didLatestPitchExceedThreshold && !DidPitchExceedThreshold;
        if (shouldJump)
        {
            Jump();
        }
        DidPitchExceedThreshold = didLatestPitchExceedThreshold;
        Pitch = latestPitch;
    }
    private bool DoesPitchExceedThreshold(float pitch)
    {
        var offset = DidPitchExceedThreshold ? -20.0f : 0.0f;
        return InvertPitch ? pitch > (PitchThreshold + offset) : pitch < (PitchThreshold + offset);
    }
    private bool DidPitchExceedThreshold = false;
    public override void Calibrate()
    {
        base.Calibrate();
        PitchThreshold = Pitch;
    }

    private void UpdatePlayerRigidBody()
    {
        if (IsRunning && !IsGameOver && HasJumpedAtLeastOnce)
        {
            PlayerRigidBody.useGravity = true;
            PlayerRigidBody.isKinematic = false;
        }
        else
        {
            PlayerRigidBody.useGravity = false;
            PlayerRigidBody.isKinematic = true;
        }
    }

    private float lastTimeObstacleSpawned = 0;
    [Range(0.5f, 5.0f)]
    public float TimeBetweenObstacleSpawn = 1.0f;
    protected override void CheckObstacleSpawn()
    {
        if (!HasJumpedAtLeastOnce) { return; }
        base.CheckObstacleSpawn();
        if (runtime - lastTimeObstacleSpawned >= TimeBetweenObstacleSpawn)
        {
            SpawnObstacle();
        }
    }
    private void SpawnObstacle()
    {
        var obstacle = Instantiate(PipePrefab, Scene.transform.position, Quaternion.identity, Scene.transform);

        Vector3 position = new(Size.x / 2, Size.y * UnityEngine.Random.value, 0);
        obstacle.transform.localPosition += position;

        lastTimeObstacleSpawned = runtime;

        AddObstacle(obstacle);
    }
}


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
    private void Jump()
    {
        if (!HasJumpedAtLeastOnce)
        {
            HasJumpedAtLeastOnce = true;
            UpdatePlayerRigidBody();
        }
    }
    private void CheckJumpInput()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (IsRunning)
            {
                Jump();
            }
            else
            {
                ToggleIsRunning();
            }
        }
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
}


using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Collections.Generic;
using System.Linq;

public class BS_BaseDemo : MonoBehaviour
{
    public GameObject PlayerPrefab;
    public GameObject EnemyPrefab;
    public GameObject CollectablePrefab;
    public GameObject PipePrefab;

    public GameObject Scene;

    private GameObject Controls;
    private Button ToggleButton;
    private TextMeshProUGUI ScoreText;
    private TextMeshProUGUI HealthText;
    private Button CalibrateButton;

    private TextMeshProUGUI GameOverText;

    public List<BS_VibrationConfiguration> EnemyVibrationConfigurations = new();
    public List<BS_VibrationConfiguration> CollectableVibrationConfigurations = new();

    protected BS_DevicePair DevicePair => BS_DevicePair.Instance;

    protected GameObject Player;
    protected Rigidbody PlayerRigidBody;
    public float PlayerLinearDamping = 0.0f;

    public BS_SensorRate SensorRate = BS_SensorRate._20ms;

    public Vector3 Size = new(2f, 1f, 0f);

    protected virtual void Start()
    {
        Player = Instantiate(PlayerPrefab, Scene.transform.position, Quaternion.identity, Scene.transform);
        PlayerRigidBody = Player.GetComponent<Rigidbody>();
        UpdateLinearDamping();

        Controls = transform.Find("Controls").gameObject;
        ToggleButton = Controls.transform.Find("Toggle").GetComponent<Button>();
        ScoreText = Controls.transform.Find("Score").GetComponentInChildren<TextMeshProUGUI>();
        CalibrateButton = Controls.transform.Find("Calibrate").GetComponent<Button>();
        HealthText = Controls.transform.Find("Health").GetComponentInChildren<TextMeshProUGUI>();
        GameOverText = transform.Find("Game Over").GetComponent<TextMeshProUGUI>();

        ToggleButton.onClick.AddListener(ToggleIsRunning);
        CalibrateButton.onClick.AddListener(Calibrate);
    }
    private void UpdateLinearDamping()
    {
        if (PlayerRigidBody != null)
        {
            PlayerRigidBody.linearDamping = PlayerLinearDamping;
            Debug.Log($"updated linearDamping to {PlayerRigidBody.linearDamping}");
        }
        else
        {
            Debug.Log("mo PlayerRigidBody found");
        }
    }

    protected virtual void OnEnable()
    {
        UpdateLinearDamping();
        Scene.SetActive(true);

        DevicePair.OnDeviceGameRotation += OnDeviceQuaternion;
        DevicePair.OnDeviceRotation += OnDeviceQuaternion;
    }
    protected virtual void OnDisable()
    {
        if (!gameObject.scene.isLoaded) return;
        Scene.SetActive(false);
        Reset();
        IsRunning = false;

        DevicePair.OnDeviceGameRotation -= OnDeviceQuaternion;
        DevicePair.OnDeviceRotation -= OnDeviceQuaternion;
    }

    protected virtual void OnDeviceQuaternion(BS_DevicePair devicePair, BS_InsoleSide insoleSide, BS_Device device, Quaternion quaternion, ulong timestamp) { }

    [SerializeField]
    private bool _isRunning = false;
    public bool IsRunning
    {
        get => _isRunning;
        protected set
        {
            if (_isRunning != value)
            {
                _isRunning = value;
                OnIsRunningUpdate();
            }
        }
    }
    protected virtual void OnIsRunningUpdate()
    {
        Debug.Log($"IsRunning: {IsRunning}");

        var toggleButtonText = ToggleButton.transform.GetChild(0).GetComponentInChildren<TextMeshProUGUI>();
        if (IsGameOver)
        {
            toggleButtonText.text = "Restart";
        }
        else
        {
            toggleButtonText.text = IsRunning ? "Stop" : "Play";
        }
    }
    protected void ToggleIsRunning()
    {
        if (IsGameOver)
        {
            Reset();
        }
        IsRunning = !IsRunning;
    }

    [SerializeField]
    private float _score = 0;
    public float Score
    {
        get => _score;
        protected set
        {
            if (_score != value)
            {
                _score = value;
                OnScoreUpdate();
            }
        }
    }

    protected virtual void OnScoreUpdate()
    {
        //Debug.Log($"Score: {Score}");
        ScoreText.text = $"Score: {Math.Floor(Score)}";
    }

    [SerializeField]
    private float _health = 100;
    public float Health
    {
        get => _health;
        protected set
        {
            if (_health != value)
            {
                _health = value;
                OnHealthUpdate();
            }
        }
    }
    public bool IsGameOver => Health <= 0;

    protected virtual void OnHealthUpdate()
    {
        Debug.Log($"Health: {Health}%");
        HealthText.text = $"Health: {Math.Floor(Health)}%";
        if (IsGameOver)
        {
            OnGameOver();
        }
    }
    protected virtual void OnGameOver()
    {
        Debug.Log("Game Over");
        IsRunning = false;
        GameOverText.gameObject.SetActive(true);
    }
    public virtual void Calibrate()
    {
        Debug.Log("Calibrating...");
    }

    public virtual void Reset()
    {
        Debug.Log("Reset");
        Health = 100;
        Score = 0;
        GameOverText.gameObject.SetActive(false);
        ClearObstacles();
    }

    protected readonly List<GameObject> Obstacles = new();
    protected void ClearObstacles()
    {
        foreach (var obstacle in Obstacles.ToList())
        {
            RemoveObstacle(obstacle);
        }
    }
    [Range(0.1f, 10.0f)]
    public float Speed = 1.0f;
    protected virtual void MoveObstacles() { }

    protected bool IsObstacleAPrefabOf(GameObject obstacle, GameObject prefab) => obstacle.name.Contains(prefab.name);
    protected bool IsObstacleCollectable(GameObject obstacle) => IsObstacleAPrefabOf(obstacle, CollectablePrefab);
    protected bool IsObstacleEnemy(GameObject obstacle) => IsObstacleAPrefabOf(obstacle, EnemyPrefab);
    protected bool IsObstaclePipe(GameObject obstacle) => IsObstacleAPrefabOf(obstacle, PipePrefab);

    protected virtual void OnObstacleCollision(GameObject obstacle)
    {
        Debug.Log($"collided with {obstacle.name}");
        if (IsObstacleCollectable(obstacle))
        {
            //Debug.Log("collided with collectable");
            OnCollectableCollision(obstacle);
        }
        else if (IsObstacleCollectable(obstacle))
        {
            //Debug.Log("collided with enemy");
            OnEnemyCollision(obstacle);
        }
        else if (IsObstaclePipe(obstacle))
        {
            //Debug.Log("collided with pipe");
            OnEnemyCollision(obstacle);
        }
        else
        {
            Debug.LogError($"uncaught collided obstacle {obstacle.name}");
        }
        RemoveObstacle(obstacle);
    }
    protected virtual void CheckObstaclePositions() { }
    protected virtual void RemoveObstacle(GameObject obstacle)
    {
        if (obstacle.TryGetComponent<BS_ColliderBroadcaster>(out var colliderBroadcaster))
        {
            colliderBroadcaster.OnCollider -= OnObstacleCollider;
        }
        //Debug.Log("removing obstacle");
        Obstacles.Remove(obstacle);
        Destroy(obstacle);
    }
    protected void AddObstacle(GameObject obstacle)
    {
        if (obstacle.TryGetComponent<BS_ColliderBroadcaster>(out var colliderBroadcaster))
        {
            colliderBroadcaster.OnCollider += OnObstacleCollider;
        }
        Obstacles.Add(obstacle);
    }

    protected void OnObstacleCollider(GameObject obstacle, Collider collider)
    {
        OnObstacleCollision(obstacle);
    }

    public int CollectableScore = 100;
    protected virtual void OnCollectableCollision(GameObject obstacle)
    {
        Score += CollectableScore;
    }
    public float EnemyDamage = 20.0f;
    protected virtual void OnEnemyCollision(GameObject obstacle)
    {
        Health -= EnemyDamage;
    }

    protected float runtime = 0;
    protected virtual void Update()
    {
        if (!IsRunning) { return; }
        MoveObstacles();
    }
    protected virtual void FixedUpdate()
    {
        if (!IsRunning) { return; }
        runtime += Time.deltaTime;
        CheckObstacleSpawn();
        CheckObstaclePositions();
    }

    protected virtual void CheckObstacleSpawn() { }

    protected void TriggerVibration(BS_InsoleSide insoleSide, List<BS_VibrationConfiguration> vibrationConfigurations)
    {
        DevicePair.GetDevice(insoleSide)?.TriggerVibration(vibrationConfigurations);
    }
}

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

    public GameObject Scene;

    private GameObject Controls;
    private Button ToggleButton;
    private TextMeshProUGUI ScoreText;
    private TextMeshProUGUI HealthText;
    private Button CalibrateButton;

    private TextMeshProUGUI GameOverText;

    protected BS_DevicePair DevicePair => BS_DevicePair.Instance;

    protected GameObject Player;

    protected virtual void Start()
    {
        Player = Instantiate(PlayerPrefab, Scene.transform.position, Quaternion.identity, Scene.transform);

        Controls = transform.Find("Controls").gameObject;
        ToggleButton = Controls.transform.Find("Toggle").GetComponent<Button>();
        ScoreText = Controls.transform.Find("Score").GetComponentInChildren<TextMeshProUGUI>();
        CalibrateButton = Controls.transform.Find("Calibrate").GetComponent<Button>();
        HealthText = Controls.transform.Find("Health").GetComponentInChildren<TextMeshProUGUI>();
        GameOverText = transform.Find("Game Over").GetComponent<TextMeshProUGUI>();

        ToggleButton.onClick.AddListener(ToggleIsRunning);
        CalibrateButton.onClick.AddListener(Calibrate);
    }

    protected virtual void OnEnable()
    {
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

        var toggleButtonText = ToggleButton.transform.Find("Text").GetComponentInChildren<TextMeshProUGUI>();
        if (IsGameOver)
        {
            toggleButtonText.text = "Restart";
        }
        else
        {
            toggleButtonText.text = IsRunning ? "Stop" : "Play";
        }
    }
    private void ToggleIsRunning()
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
        Debug.Log($"Score: {Score}");
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

    public LayerMask CollisionLayer;
    private readonly Collider[] colliders = new Collider[10];
    protected virtual void CheckObstacleCollisions()
    {
        var colliderCount = Physics.OverlapSphereNonAlloc(Player.transform.position, 0.1f, colliders, CollisionLayer);
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
    protected virtual void CheckObstaclePositions() { }
    protected void RemoveObstacle(GameObject obstacle)
    {
        //Debug.Log("removing obstacle");
        Obstacles.Remove(obstacle);
        Destroy(obstacle);
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
        CheckObstacleCollisions();
        CheckObstaclePositions();
    }

    protected virtual void CheckObstacleSpawn() { }
}

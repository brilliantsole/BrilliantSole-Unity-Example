using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

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

    protected virtual void Start()
    {
        Controls = transform.Find("Controls").gameObject;
        ToggleButton = Controls.transform.Find("Toggle").GetComponent<Button>();
        ScoreText = Controls.transform.Find("Score").GetComponentInChildren<TextMeshProUGUI>();
        CalibrateButton = Controls.transform.Find("Calibrate").GetComponent<Button>();
        HealthText = Controls.transform.Find("Health").GetComponentInChildren<TextMeshProUGUI>();

        ToggleButton.onClick.AddListener(ToggleIsRunning);
        CalibrateButton.onClick.AddListener(Calibrate);
    }

    protected virtual void OnEnable()
    {
        Scene.SetActive(true);
    }
    protected virtual void OnDisable()
    {
        Scene.SetActive(false);
        Reset();
        IsRunning = false;
    }

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
        toggleButtonText.text = IsRunning ? "Stop" : "Play";
    }
    private void ToggleIsRunning()
    {
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

    protected virtual void OnHealthUpdate()
    {
        Debug.Log($"Health: {Health}%");
        HealthText.text = $"Health: {Math.Floor(Health)}%";
        if (Health <= 0)
        {
            OnGameOver();
        }
    }
    protected virtual void OnGameOver()
    {
        Debug.Log("Game Over");
        IsRunning = false;
    }

    public virtual void Calibrate()
    {
        Debug.Log("Calibrating");
    }

    public virtual void Reset()
    {
        Debug.Log("Reset");
        Health = 100;
        Score = 0;
    }
}

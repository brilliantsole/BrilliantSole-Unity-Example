using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BS_FootFlyerDemo : BS_BaseDemo
{
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
    }
    protected override void OnDisable()
    {
        base.OnDisable();
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
            Debug.Log("collided with collectable");
            OnCollectableCollision(obstacle);
        }
        else if (obstacle.name.Contains(EnemyPrefab.name))
        {
            Debug.Log("collided with enemy");
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

        var isObstacleEnemy = Random.value > EnemyObstacleProbability;
        var obstaclePrefab = isObstacleEnemy ? EnemyPrefab : CollectablePrefab;

        Debug.Log($"spawning {(isObstacleEnemy ? "enemy" : "collectable")}");

        var obstacle = Instantiate(obstaclePrefab, Scene.transform.position, Quaternion.identity, Scene.transform);

        Vector3 position = new(Size.x / 2, Size.y * Random.value, 0);
        obstacle.transform.localPosition += position;

        Obstacles.Add(obstacle);

        lastTimeObstacleSpawned = runtime;
    }

    private void MoveMouse()
    {
        if (!Input.mousePresent) { return; }
        var scroll = Input.mouseScrollDelta;
        var position = Player.transform.localPosition;
        position.y += scroll.y;
        Player.transform.localPosition = position;
    }

    void Update()
    {
        if (!IsRunning) { return; }
        MoveObstacles();
        MoveMouse();
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

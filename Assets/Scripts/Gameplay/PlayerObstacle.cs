using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObstacle : MonoBehaviour
{
    [SerializeField]
    PlayerObstacleObject obstaclePrefab;

    [SerializeField]
    Vector2 obstacleSpeed = Vector2.zero;

    [SerializeField]
    float obstacleGap = 4f;


    GameCamera gameCamera;

    List<PlayerObstacleObject> obstaclesSpawned;
    // FOR NOW, obstacles only spawn to the left and go right

    void Awake()
    {
        gameCamera = FindObjectOfType<GameCamera>();
        obstaclesSpawned = new List<PlayerObstacleObject>();
    }

    void Update()
    {
        if(ShouldSpawnObstacle())
        {
            SpawnObstacle();
        }

        if(IsOutsideFrame())
        {
            DestroySelf();
        }
    }

    void SpawnObstacle()
    {
        // Spawn outside the camera's view on the left side
        float xPos = GameplayConstants.platformSpawnPoint.x - obstaclePrefab.transform.lossyScale.x / 2 - gameCamera.CameraWidth() / 2;
        float zPos = GameplayConstants.obstacleDepth;
        float yPos = transform.position.y;

        Vector3 spawnPosition = new Vector3(xPos, yPos, zPos);
        PlayerObstacleObject obstacleSpawned = Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity);
        obstacleSpawned.speed = obstacleSpeed;

        obstaclesSpawned.Add(obstacleSpawned);
    }

    void DestroySelf()
    {
        foreach(var obstacle in obstaclesSpawned) Destroy(obstacle.gameObject);
        Destroy(gameObject);
    }

    bool ShouldSpawnObstacle()
    {
        if(obstaclesSpawned.Count == 0)
            return true;

        PlayerObstacleObject latestSpawn = obstaclesSpawned[obstaclesSpawned.Count - 1];
        float xPositionRequiredToSpawnMore = -(gameCamera.CameraWidth() / 2) + latestSpawn.transform.lossyScale.x / 2 + obstacleGap;
        
        return latestSpawn.transform.position.x >= xPositionRequiredToSpawnMore;
    }

    bool IsOutsideFrame()
    {
        if(obstaclesSpawned.Count == 0) return false;

        float heightMin = gameCamera.CalculateVerticalEdgeOfScreen(GameplayConstants.obstacleDepth, VerticalDirection.below) -
            obstaclePrefab.transform.lossyScale.y;

        return obstaclesSpawned[0].transform.position.y < heightMin;
    }
}

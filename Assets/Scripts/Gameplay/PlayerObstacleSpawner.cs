using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObstacleSpawner : MonoBehaviour
{
    [SerializeField]
    PlayerObstacle obstaclePrefab;

    [SerializeField]
    int startingScoreForSpawns = 10; // Upon the score reaching this much, the obstacles will start spawning


    ScoreManager scoreManager;
    BlockTower blockTower;
    GameCamera gameCamera;

    Coroutine spawnCoroutine;
    List<PlayerObstacle> obstaclesSpawned;

    public void StartSpawning()
    {
        spawnCoroutine = StartCoroutine(SpawnObstacles());
    }

    public void StopSpawning()
    {
        if(spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }


    void Start()
    {
        obstaclesSpawned = new List<PlayerObstacle>();

        scoreManager = FindObjectOfType<ScoreManager>();
        blockTower = FindObjectOfType<BlockTower>();
        gameCamera = FindObjectOfType<GameCamera>();

        StartSpawning();
    }

    IEnumerator SpawnObstacles()
    {
        int scoreForNextSpawn = startingScoreForSpawns;
        int scoreIncreasePerSpawn = 4;

        Debug.Log(gameCamera.CameraWidth());

        while(true)
        {
            // Stall until we reach the required score
            while(scoreForNextSpawn > scoreManager.Score)
            {
                yield return null;
            }

            SpawnObstacle();

            scoreForNextSpawn += scoreIncreasePerSpawn;
        }
    }

    void SpawnObstacle()
    {
        // Spawn outside the camera's view on the left side
        float xPos = GameplayConstants.platformSpawnPoint.x - obstaclePrefab.transform.lossyScale.x / 2 - gameCamera.CameraWidth() / 2;
        float zPos = GameplayConstants.obstacleDepth;

        // For now, Choose a random range of a y position which spawns it anywhere within the visible screen
        float yPosMax = gameCamera.CalculateVerticalEdgeOfScreen(zPos, VerticalDirection.above) - 
            obstaclePrefab.transform.lossyScale.y;
        float yPosMin = gameCamera.CalculateVerticalEdgeOfScreen(zPos, VerticalDirection.below) + 
            obstaclePrefab.transform.lossyScale.y * 2;
        float yPos = Random.Range(yPosMin, yPosMax);

        Vector3 spawnPosition = new Vector3(xPos, yPos, zPos);
        obstaclesSpawned.Add(Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity));
    }
}

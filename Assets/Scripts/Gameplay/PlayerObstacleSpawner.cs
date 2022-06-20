using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerObstacleSpawner : MonoBehaviour
{
    [SerializeField]
    PlayerObstacle obstaclePrefab;

    [SerializeField, Min(0)]
    int startingScoreForSpawns = 10; // Upon the score reaching this much, the obstacles will start spawning

    [SerializeField, Min(0)]
    int scoreIncreaseForSpawn = 3;


    ScoreManager scoreManager;
    GameCamera gameCamera;

    Coroutine spawnCoroutine;
    List<PlayerObstacle> obstaclesSpawned;


    void Awake()
    {
        obstaclesSpawned = new List<PlayerObstacle>();

        scoreManager = FindObjectOfType<ScoreManager>();
        gameCamera = FindObjectOfType<GameCamera>();
    }

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

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Blocker Bug", "S2190:Recursion should not be infinite", 
        Justification = "The StopSpawning method stops the infinite loop")]
    IEnumerator SpawnObstacles()
    {
        int scoreForNextSpawn = startingScoreForSpawns;

        while(true)
        {
            // Stall until we reach the required score
            while(scoreForNextSpawn > scoreManager.Score)
            {
                yield return null;
            }

            SpawnObstacle();

            scoreForNextSpawn += scoreIncreaseForSpawn;
        }
    }

    void SpawnObstacle()
    {
        // Remove all obstacles which have deleted themselves
        obstaclesSpawned.RemoveAll(obstacle => obstacle == null);

        // Spawn outside the camera's view on the left side
        float xPos = GameplayConstants.platformSpawnPoint.x - obstaclePrefab.transform.lossyScale.x / 2 - gameCamera.CameraWidth() / 2;
        float zPos = GameplayConstants.playerObstacleDepth;

        // For now, Choose a random range of a y position which spawns it anywhere within the visible screen
        // Also, make it so no obstacle spawn one on top of another by selecting only regions which are not already occupied
        float yPos;
        float yMargin = 1.4f;

        int iteration = 0; 
        int maxIterations = 8; // Stop trying to find a position if no position is found within this count

        bool isPosAvailable = true;
        do {
            float yPosMax = gameCamera.CalculateVerticalEdgeOfScreen(zPos, VerticalDirection.above) - 
                obstaclePrefab.transform.lossyScale.y / 2;
            float yPosMin = gameCamera.CalculateVerticalEdgeOfScreen(zPos, VerticalDirection.below) + 
                obstaclePrefab.transform.lossyScale.y / 2;
            yPos = Random.Range(yPosMin, yPosMax);

            foreach(PlayerObstacle obstacle in obstaclesSpawned)
            {
                float heightDifference = Mathf.Abs(obstacle.transform.position.y - yPos);
                if(heightDifference < yMargin)
                {
                    isPosAvailable = false;
                    break;
                }
            }
            
            if(iteration++ >= maxIterations) return;
        } while(!isPosAvailable);

        Vector3 spawnPosition = new(xPos, yPos, zPos);
        obstaclesSpawned.Add(Instantiate(obstaclePrefab, spawnPosition, Quaternion.identity));
    }

}

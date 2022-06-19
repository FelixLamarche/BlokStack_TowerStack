using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    [SerializeField]
    float maxSpawnAreaWidth = 7f;

    BlockSpawner blockSpawner;
    PlayerObstacleSpawner obstacleSpawner;
    SpikeSpawner spikeSpawner;
    GameCamera gameCamera;

    List<float> testFloats = new();
    bool isSpawning = false;

    void Awake()
    {
        blockSpawner = FindObjectOfType<BlockSpawner>();
        obstacleSpawner = FindObjectOfType<PlayerObstacleSpawner>();
        spikeSpawner = FindObjectOfType<SpikeSpawner>();
        gameCamera = FindObjectOfType<GameCamera>();
    }


    public void StartSpawning()
    {
        if (isSpawning) return;

        blockSpawner?.RemoveAllBlocksSpawned();
        blockSpawner?.StartSpawning();
        obstacleSpawner?.StartSpawning();
        spikeSpawner?.StartSpawning();
        isSpawning = true;
    }

    public void StopSpawning()
    {
        isSpawning = false;

        blockSpawner?.StopSpawning();
        obstacleSpawner?.StopSpawning();
        spikeSpawner?.StopSpawning();
    }

    // Get a random unused spawn position for a block
    // If no spawn point is available, it returns float.PositiveInfinity
    public float GetRandomBlockSpawnPosition(float blockWidth)
    {
        // Gets all visible objects, then get all the ranges in which the new block may spawn
        // Then choose one random x position for it to spawn at
        List<GameObject> blocksOnScreen = new();
        
        if(blockSpawner != null)
            blocksOnScreen.AddRange(blockSpawner.GetBlocksOnScreen());
        if(spikeSpawner != null)
            blocksOnScreen.AddRange(spikeSpawner.GetSpikesOnScreen());

        float blockSpawnZoneWidth = blockWidth + 0.04f; // Small margin to avoid blocks spawning stuck to each other

        List<Vector2> blockedSpawnRanges = new(); // x -> lower-bound x value, y-> higher-bound x value
        for(int i = 0; i < blocksOnScreen.Count; i++)
        {
            Vector2 blockedZone = new Vector2(blocksOnScreen[i].transform.position.x - blocksOnScreen[i].transform.lossyScale.x / 2,
                                            blocksOnScreen[i].transform.position.x + blocksOnScreen[i].transform.lossyScale.x / 2);
            blockedZone.x -= blockSpawnZoneWidth / 2;
            blockedZone.y += blockSpawnZoneWidth / 2;
            blockedSpawnRanges.Add(blockedZone);
        }
        blockedSpawnRanges.Sort(new Vector2XCompComparer());


        List<float> availableSpawnRanges = new List<float>();

        float spawnWidth = GetSpawnWidth();

        float lowerBoundX = -spawnWidth/ 2 + blockSpawnZoneWidth / 2;
        float upperBoundX = spawnWidth/ 2 - blockSpawnZoneWidth / 2;

        availableSpawnRanges.Add(lowerBoundX);
        availableSpawnRanges.Add(upperBoundX);

        bool replacedCoordinate;

        for (int i = 0; i < blockedSpawnRanges.Count; i++)
        {
            if (blockedSpawnRanges[i].x > upperBoundX || blockedSpawnRanges[i].y < lowerBoundX) continue;

            replacedCoordinate = false;

            for (int j = 0; j < availableSpawnRanges.Count; j+=2) // j is left-index, j+1 is the corresponding right index
            {
                // The block overlaps a complete available region
                if(blockedSpawnRanges[i].x <= availableSpawnRanges[j] && blockedSpawnRanges[i].y >= availableSpawnRanges[j + 1])
                {
                    availableSpawnRanges[j] = blockedSpawnRanges[i].x;
                    availableSpawnRanges[j + 1] = blockedSpawnRanges[i].y;

                    replacedCoordinate = true;
                    // Do not break as multiple zones may be overlapped
                }
                // Block overlaps a left-side border, but does not overlap the zone completely
                else if (blockedSpawnRanges[i].x <= availableSpawnRanges[j] && blockedSpawnRanges[i].y > availableSpawnRanges[j])
                {
                    availableSpawnRanges[j] = blockedSpawnRanges[i].y;
                    replacedCoordinate = true;
                    break;
                }
                // Block overlaps a right-side border, but does not overlap the zone completely
                else if(blockedSpawnRanges[i].x < availableSpawnRanges[j + 1] && blockedSpawnRanges[i].y >= availableSpawnRanges[j + 1])
                {
                    availableSpawnRanges[j + 1] = blockedSpawnRanges[i].x;
                    replacedCoordinate = true;
                    break;
                } 
            }

            if (!replacedCoordinate)
            {
                if (blockedSpawnRanges[i].x > lowerBoundX && blockedSpawnRanges[i].y < upperBoundX)
                {
                    availableSpawnRanges.Add(blockedSpawnRanges[i].x);
                    availableSpawnRanges.Add(blockedSpawnRanges[i].y);
                }
            }
            availableSpawnRanges.Sort();

            // If we completely block a zone, we will overshoot and have to remove a zone
            if (availableSpawnRanges.Count > 0 && availableSpawnRanges[availableSpawnRanges.Count - 1] > upperBoundX)
            {
                availableSpawnRanges.RemoveAt(availableSpawnRanges.Count - 2);
                availableSpawnRanges.RemoveAt(availableSpawnRanges.Count - 1);
            }
            if (availableSpawnRanges.Count > 0 && availableSpawnRanges[0] < lowerBoundX)
            {
                availableSpawnRanges.RemoveAt(0);
                availableSpawnRanges.RemoveAt(1);
            }
        }

        testFloats = availableSpawnRanges;

        // Get an a bottom-range index, the range being this randomIndex and randomIndex + 1
        if (availableSpawnRanges.Count == 0)
            return float.PositiveInfinity;

        int randomIndex = Random.Range(0, availableSpawnRanges.Count - 2);

        return Random.Range(availableSpawnRanges[randomIndex], availableSpawnRanges[randomIndex + 1]);
    }

    float GetSpawnWidth()
    {
        float screenEdgeMargin = 1f;
        float width = maxSpawnAreaWidth;

        if(gameCamera != null)
        {
            float cameraWidth = gameCamera.CameraWidth() - screenEdgeMargin;
            if (cameraWidth < width) width = cameraWidth;
        }

        return width;
    }

    class Vector2XCompComparer : IComparer<Vector2>
    {   
        public int Compare(Vector2 a, Vector2 b)
        {
            if (a.x > b.x)
                return 1;
            else if(a.x < b.x)
                return -1;

            return 0;
        }
    }
}

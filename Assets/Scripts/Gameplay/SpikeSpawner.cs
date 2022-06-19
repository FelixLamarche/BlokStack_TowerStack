using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikeSpawner : MonoBehaviour
{
    public float CurTimeBetweenSpawns {get {return curTimeBetweenSpawns;}}

    [SerializeField]
    Spike spikePrefab;
    [SerializeField, Min(0f)]
    float startingTimeBetweenSpawns = 1f;
    [SerializeField, Min(0f)]
    float minimumTimeBetweenSpawns = 0.5f;
    [SerializeField, Tooltip("Amount of time decreased by each further spawn"), Min(0f)]
    float spawnTimeDifferencePerSecond = 0.02f;


    float curTimeBetweenSpawns;

    List<Spike> spikesSpawned;
    Coroutine spawnCoroutine;

    GameCamera gameCamera;
    SpawnManager spawnManager;

    void Awake()
    {
        spikesSpawned = new List<Spike>();
        curTimeBetweenSpawns = startingTimeBetweenSpawns;

        gameCamera = FindObjectOfType<GameCamera>();
        spawnManager = FindObjectOfType<SpawnManager>();
    }

    public List<GameObject> GetSpikesOnScreen()
    {
        List<GameObject> spawnedBlocks = new List<GameObject>();
        float lowerBoundScreen = gameCamera.CalculateVerticalEdgeOfScreen(GameplayConstants.blockDepth, VerticalDirection.below);

        for(int i = 0; i < spikesSpawned.Count; i++)
        {
            if(spikesSpawned[i].transform.position.y > lowerBoundScreen)
                spawnedBlocks.Add(spikesSpawned[i].gameObject);
        }

        return spawnedBlocks;
    }

    public void StartSpawning()
    {
        if(spawnCoroutine is null)
            spawnCoroutine = StartCoroutine(StartSpawningBlocks());
    }

    public void StopSpawning()
    {
        if(spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            spawnCoroutine = null;
        }
    }

    public void RemoveAllSpikesSpawned()
    {
        foreach(Spike spike in spikesSpawned)
        {
            Destroy(spike.gameObject);
        }
        spikesSpawned.Clear();
    }


    IEnumerator StartSpawningBlocks()
    {
        float deltaTimeSinceSpawnTimeChanged = 0f;
        float timeBetweenDifferences = 1f;

        while(true)
        {
            // DONT CHANGE ORDERING
            yield return new WaitForSeconds(curTimeBetweenSpawns);
            SpawnSpike();

            deltaTimeSinceSpawnTimeChanged += curTimeBetweenSpawns;
            if(deltaTimeSinceSpawnTimeChanged >= timeBetweenDifferences)
            {
                deltaTimeSinceSpawnTimeChanged -= timeBetweenDifferences;
                curTimeBetweenSpawns = Mathf.Max(curTimeBetweenSpawns - spawnTimeDifferencePerSecond, minimumTimeBetweenSpawns);
            }
        }
    }

    public void SpawnSpike()
    {
        // To spawn within the boundaries of the blockSpawner
        float randomXPos = spawnManager.GetRandomBlockSpawnPosition(spikePrefab.transform.lossyScale.x);
        float depth = GameplayConstants.blockDepth;
        float height = gameCamera.CalculateVerticalEdgeOfScreen(depth, VerticalDirection.above);

        Vector3 spawnPoint = new Vector3 (randomXPos, height + spikePrefab.transform.lossyScale.y / 2, depth);
        
        Spike spikeSpawned = Instantiate(spikePrefab, spawnPoint, Quaternion.identity);
        spikesSpawned.Add(spikeSpawned);
    }
}

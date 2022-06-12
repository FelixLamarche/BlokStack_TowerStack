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

    public void Start()
    {
        spikesSpawned = new List<Spike>();
        curTimeBetweenSpawns = startingTimeBetweenSpawns;

        gameCamera = FindObjectOfType<GameCamera>();

        StartSpawning();
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

    public void RemoveAllBlocksSpawned()
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
        float maxXDisplacement = spikePrefab.transform.localScale.x / 2;

        float width = gameCamera.CameraWidth();
        float depth = GameplayConstants.blockDepth;
        float height = gameCamera.CalculateVerticalEdgeOfScreen(depth, VerticalDirection.above);

        Vector3 spawnPoint = new Vector3 (Random.Range(-width / 2 + maxXDisplacement, width / 2 - maxXDisplacement),
                                        height + spikePrefab.transform.lossyScale.y / 2,
                                        depth);
        
        Spike spikeSpawned = Instantiate(spikePrefab, spawnPoint, Quaternion.identity);
        spikesSpawned.Add(spikeSpawned);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    public float CurTimeBetweenSpawns {get {return curTimeBetweenSpawns;}}

    [SerializeField]
    BlockTowerElement blockPrefab;

    [SerializeField, Min(0f)]
    float startingTimeBetweenSpawns = 1f;
    [SerializeField, Min(0f)]
    float minimumTimeBetweenSpawns = 0.5f;
    [SerializeField, Tooltip("Amount of time decreased by each further spawn"), Min(0f)]
    float spawnTimeDifferencePerSecond = 0.02f;

    float curTimeBetweenSpawns;


    GameCamera gameCamera;
    SpawnManager spawnManager;

    Coroutine spawnCoroutine;
    readonly List<BlockTowerElement> blockObjectsSpawned = new List<BlockTowerElement>();

    void Awake()
    {
        gameCamera = FindObjectOfType<GameCamera>();
        spawnManager = FindObjectOfType<SpawnManager>();

        curTimeBetweenSpawns = startingTimeBetweenSpawns;
    }

    void LateUpdate()
    {
        MoveBlockSpawnerOutsideOfFrame();
    }

    // Returns a list of the blocks spawned whom are not in a tower
    public List<GameObject> GetBlocksOnScreen()
    {
        List<GameObject> spawnedBlocks = new List<GameObject>();
        float lowerBoundScreen = gameCamera.CalculateVerticalEdgeOfScreen(GameplayConstants.blockDepth, VerticalDirection.below);

        for(int i = 0; i < blockObjectsSpawned.Count; i++)
        {
            if(blockObjectsSpawned[i].transform.position.y > lowerBoundScreen && 
            !blockObjectsSpawned[i].IsAttachedToTower)
                spawnedBlocks.Add(blockObjectsSpawned[i].gameObject);
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

    public void RemoveAllBlocksSpawned()
    {
        foreach(BlockTowerElement blockSpawned in blockObjectsSpawned)
        {
            Destroy(blockSpawned.gameObject);
        }
        blockObjectsSpawned.Clear();
    }

    [System.Diagnostics.CodeAnalysis.SuppressMessage("Blocker Bug", "S2190:Recursion should not be infinite", 
        Justification = "Gets stopped by the StopSpawning method")]
    IEnumerator StartSpawningBlocks()
    {
        float deltaTimeSinceSpawnTimeChanged = 0f;
        float timeBetweenDifferences = 1f;

        while(true)
        {
            // DONT CHANGE ORDERING, otherwise 2 blocks spawn at the same time after a retry
            yield return new WaitForSeconds(curTimeBetweenSpawns);
            SpawnBlock();

            deltaTimeSinceSpawnTimeChanged += curTimeBetweenSpawns;
            if(deltaTimeSinceSpawnTimeChanged >= timeBetweenDifferences)
            {
                deltaTimeSinceSpawnTimeChanged -= timeBetweenDifferences;
                curTimeBetweenSpawns = Mathf.Max(curTimeBetweenSpawns - spawnTimeDifferencePerSecond, minimumTimeBetweenSpawns);
            }
        }
    }

    void SpawnBlock()
    {
        // To spawn within the boundaries of the blockSpawner
        float randomXPos = spawnManager.GetRandomBlockSpawnPosition(blockPrefab.transform.lossyScale.x);
        
        // If no spawn position is available, we return
        if (float.IsInfinity(randomXPos)) return;

        Vector3 worldSpawnPoint = transform.position + new Vector3(randomXPos, blockPrefab.transform.lossyScale.y / 2, 0);
        
        BlockTowerElement blockSpawned = Instantiate(blockPrefab, worldSpawnPoint, Quaternion.identity);
        blockObjectsSpawned.Add(blockSpawned);
    }

    private void MoveBlockSpawnerOutsideOfFrame()
    {
        // Always keep height at just outside of the camera view + block's size to spawn outside of the view
        float newY = gameCamera.CalculateVerticalEdgeOfScreen(transform.position.z + 0.5f, VerticalDirection.above) +
            blockPrefab.transform.lossyScale.y / 2;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    // Draw bounding box of the spawn space
    void OnDrawGizmos()
    {
        Vector3 zDisplacement = Vector3.forward * blockPrefab.transform.lossyScale.z / 2;
        Vector3 xDisplacement = Vector3.right * blockPrefab.transform.lossyScale.x / 2;
        Gizmos.DrawLine(transform.position + zDisplacement - xDisplacement, transform.position + zDisplacement + xDisplacement); // Back line
        Gizmos.DrawLine(transform.position - xDisplacement + zDisplacement, transform.position - xDisplacement - zDisplacement); // Left line
        Gizmos.DrawLine(transform.position + xDisplacement + zDisplacement, transform.position + xDisplacement - zDisplacement); // Right line
        Gizmos.DrawLine(transform.position - zDisplacement - xDisplacement, transform.position - zDisplacement + xDisplacement); // Forward line
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockSpawner : MonoBehaviour
{
    [SerializeField]
    GameObject block;

    [SerializeField]
    float timeBetweenSpawns = 2f;

    [SerializeField]
    float width = 3f;

    [SerializeField]
    float depth = 1f;

    GameCamera gameCamera;

    Coroutine spawnCoroutine;
    List<GameObject> blockObjectsSpawned = new List<GameObject>();

    void Awake()
    {
        gameCamera = FindObjectOfType<GameCamera>();
    }

    void LateUpdate()
    {
        MoveBlockSpawnerOutsideOfFrame();
    }

    public void StartSpawning()
    {
        if(spawnCoroutine is null)
            spawnCoroutine = StartCoroutine(SpawnBlocks());
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
        foreach(GameObject blockSpawned in blockObjectsSpawned)
        {
            Destroy(blockSpawned);
        }
        blockObjectsSpawned.Clear();
    }


    IEnumerator SpawnBlocks()
    {
        while(true)
        {
            // DONT CHANGE ORDERING, otherwise 2 blocks spawn at the same time after a retry
            yield return new WaitForSeconds(timeBetweenSpawns);
            SpawnBlock();
        }
    }

    void SpawnBlock()
    {
        // To spawn within the boundaries of the blockSpawner
        float xDisplacement = block.transform.localScale.x / 2;
        float zDisplacement = block.transform.localScale.z / 2;

        Vector3 spawnPoint = transform.position + 
            Vector3.right * Random.Range(-width / 2 + xDisplacement, width / 2 - xDisplacement) +
            Vector3.forward * Random.Range(-depth / 2 + zDisplacement, depth / 2 - zDisplacement)
            + Vector3.up * block.transform.localScale.y / 2;
        
        GameObject blockSpawned = Instantiate(block, spawnPoint, Quaternion.identity);
        blockObjectsSpawned.Add(blockSpawned);
    }

    private void MoveBlockSpawnerOutsideOfFrame()
    {
        // Always keep height at just outside of the camera view + block's size to spawn outside of the view
        float newY = gameCamera.CalculateEdgeOfScreenHeight(transform.position.z + 0.5f, VerticalDirection.above) +
            block.transform.lossyScale.y / 2;
        transform.position = new Vector3(transform.position.x, newY, transform.position.z);
    }

    // Draw bounding box of the spawn space
    void OnDrawGizmos()
    {
        Vector3 zDisplacement = Vector3.forward * depth / 2;
        Vector3 xDisplacement = Vector3.right * width / 2;
        Gizmos.DrawLine(transform.position + zDisplacement - xDisplacement, transform.position + zDisplacement + xDisplacement); // Back line
        Gizmos.DrawLine(transform.position - xDisplacement + zDisplacement, transform.position - xDisplacement - zDisplacement); // Left line
        Gizmos.DrawLine(transform.position + xDisplacement + zDisplacement, transform.position + xDisplacement - zDisplacement); // Right line
        Gizmos.DrawLine(transform.position - zDisplacement - xDisplacement, transform.position - zDisplacement + xDisplacement); // Forward line
    }

}

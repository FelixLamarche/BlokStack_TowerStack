using UnityEngine;

public class BlockPlatform : BlockTowerElement
{
    BlockTower blockTower;

    void Awake()
    {
        blockTower = GetComponent<BlockTower>();
        Height = GetComponent<BoxCollider2D>().size.y;
        Score = 0;
        TowerIn = blockTower;
    }

    void Start()
    {
        ResetPlatform();
    }

    public void ResetPlatform()
    {
        transform.position = new Vector3(0f, transform.position.y, transform.position.z);
    }
}

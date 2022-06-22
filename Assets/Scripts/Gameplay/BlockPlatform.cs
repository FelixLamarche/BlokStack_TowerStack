using UnityEngine;

public class BlockPlatform : BlockTowerElement
{
    void Awake()
    {
        TowerIn = GetComponent<BlockTower>();
        Score = 0;
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

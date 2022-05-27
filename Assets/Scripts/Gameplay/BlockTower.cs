using System.Collections.Generic;
using UnityEngine;

public class BlockTower : MonoBehaviour
{
    public List<BlockTowerElement> blocksStacked = new List<BlockTowerElement>();

    TowerMovement towerMovement;
    GameCamera gameCamera;

    PlatformTower platformTower;
    public PlatformTower PlatformTower 
    {
        get {return platformTower;}
        private set {platformTower = value;}
    }

    int scoreBlocksStacked = 0;

    void Awake()
    {
        PlatformTower = GetComponent<PlatformTower>();
        towerMovement = FindObjectOfType<TowerMovement>();
        gameCamera = FindObjectOfType<GameCamera>();
    }

    public BlockTowerElement GetTopBlockElement()
    {
        if(blocksStacked.Count > 0 )
            return blocksStacked[blocksStacked.Count - 1].GetComponent<BlockTowerElement>();
        else
            return null;
    }

    public void ResetTower()
    {
        scoreBlocksStacked = 0;
        blocksStacked.Clear();
        PlatformTower.ResetPlatform();
        blocksStacked.Add(PlatformTower);
    }

    public void AddBlockElement(BlockTowerElement blockToAdd)
    {
        blocksStacked.Add(blockToAdd);
        scoreBlocksStacked += blockToAdd.Score;
        GameManager.instance.ScoreManager.SetScore(scoreBlocksStacked);
        UpdateTowerMovementTarget();
    }

    public bool IsTopOfTower(BlockTowerElement blockElement)
    {
        return blocksStacked.Count == 0 ? false : blockElement.Equals(blocksStacked[blocksStacked.Count - 1]);
    }
    
    public float GetTowerHeight()
    {
        BlockTowerElement blockElement = GetTopBlockElement();
        return blockElement is null ? 0f : blockElement.transform.position.y + blockElement.Height / 2;
    }

    void UpdateTowerMovementTarget()
    {
        if(blocksStacked.Count == 0)
            return;

        float yHeightMin = gameCamera.CalculateEdgeOfScreenHeight(transform.position.z + 0.5f, VerticalDirection.below);
        // Remove 1 from height to avoid dealing with resizing camera and to give a little bit of leeway
        yHeightMin -= 1;
        int index = blocksStacked.Count - 1;
        BlockTowerElement block = blocksStacked[index];
        while(index >= 0)
        {
            if(block.transform.position.y - block.Height < yHeightMin)
                break;
            block = blocksStacked[index];
            index--;
        }

        towerMovement.SetMovingBlock(block);
    }
}

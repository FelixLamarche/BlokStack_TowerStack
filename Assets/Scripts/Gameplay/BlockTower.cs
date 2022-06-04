using System.Collections.Generic;
using UnityEngine;

public class BlockTower : MonoBehaviour
{
    public List<BlockTowerElement> blocksStacked = new List<BlockTowerElement>();
    public BlockPlatform PlatformTower 
    {
        get {return platformTower;}
        private set {platformTower = value;}
    }

    GameCamera gameCamera;
    ScoreManager scoreManager;

    TowerMovement towerMovement;
    BlockPlatform platformTower;

    int scoreBlocksStacked = 0;

    void Awake()
    {
        PlatformTower = GetComponent<BlockPlatform>();
    }

    void Start()
    {
        towerMovement = FindObjectOfType<TowerMovement>();
        gameCamera = FindObjectOfType<GameCamera>();
        scoreManager = FindObjectOfType<ScoreManager>();
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
        scoreManager.SetScore(scoreBlocksStacked);
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

        float yHeightMin = gameCamera.CalculateEdgeOfScreenPosition(transform.position.z + 0.5f, VerticalDirection.below);
        // Remove an additional constant to give a bit more leeway to the tower
        const float yHeightLeeway = 1;
        yHeightMin -= yHeightLeeway;
        
        int index = blocksStacked.Count - 1;
        BlockTowerElement lowestBlockOnScreen = blocksStacked[index];
        while(index >= 0)
        {
            if(lowestBlockOnScreen.transform.position.y - lowestBlockOnScreen.Height < yHeightMin)
                break;
            lowestBlockOnScreen = blocksStacked[index];
            index--;
        }

        towerMovement.SetMovingBlock(lowestBlockOnScreen);
    }
}

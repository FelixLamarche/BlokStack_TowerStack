using System.Collections.Generic;
using UnityEngine;

public class BlockTower : MonoBehaviour
{
    public BlockPlatform PlatformTower 
    {
        get {return platformTower;}
        private set {platformTower = value;}
    }

    GameCamera gameCamera;
    ScoreManager scoreManager;

    TowerMovement towerMovement;
    BlockPlatform platformTower;

    readonly List<BlockTowerElement> blocksStacked = new List<BlockTowerElement>();
    int scoreBlocksStacked = 0;

    void Awake()
    {
        PlatformTower = GetComponent<BlockPlatform>();

        towerMovement = FindObjectOfType<TowerMovement>();
        gameCamera = FindObjectOfType<GameCamera>();
        scoreManager = FindObjectOfType<ScoreManager>();
    }

    void Start()
    {
        AddBlockElement(platformTower);
    }

    public BlockTowerElement GetTopBlockElement()
    {
        if (blocksStacked.Count > 0)
            return blocksStacked[^1];
        else
            return null;
    }

    public void ResetTower()
    {
        scoreBlocksStacked = 0;
        blocksStacked.Clear();
        PlatformTower.ResetPlatform();
        AddBlockElement(platformTower);
    }

    public void AddBlockElement(BlockTowerElement blockToAdd)
    {
        blocksStacked.Add(blockToAdd);
        blockToAdd.IsAttachedToTower = true;
        scoreBlocksStacked += blockToAdd.Score;
        scoreManager.SetScore(scoreBlocksStacked);
        UpdateTowerMovementTarget();
    }

    public bool IsTopOfTower(BlockTowerElement blockElement)
    {
        return blocksStacked.Count != 0 && blockElement.Equals(blocksStacked[^1]);
    }
    
    public float GetTowerHeight()
    {
        BlockTowerElement blockElement = GetTopBlockElement();
        return blockElement is null ? 0f : blockElement.transform.position.y + blockElement.Height / 2;
    }

    public void CollisionWithSpike(Spike spike)
    {
        FindObjectOfType<GameplayManager>().StopGame();
    }

    void UpdateTowerMovementTarget()
    {
        if(blocksStacked.Count == 0)
            return;

        float yHeightMin = gameCamera.CalculateVerticalEdgeOfScreen(transform.position.z + 0.5f, VerticalDirection.below);
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

        // blocks who've gone out of frame are no longer attached to the tower
        if(index > 0) blocksStacked[index - 1].IsAttachedToTower = false;

        towerMovement.SetMovingBlock(lowestBlockOnScreen);
    }
}

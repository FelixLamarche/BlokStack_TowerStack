using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public bool GameRunning {get; private set;}
    public ScoreManager ScoreManager { get {return scoreManager;} }

    [SerializeField]
    BlockSpawner blockSpawner;
    [SerializeField]
    BlockTower blockTower;

    [SerializeField]
    GameObject mainMenuButton;

    GameCamera gameCamera;
    ScoreManager scoreManager;
    SpawnManager spawnManager;

    void Awake()
    {
        GameRunning = false;
        scoreManager = GetComponent<ScoreManager>();
        spawnManager = FindObjectOfType<SpawnManager>();
        gameCamera = FindObjectOfType<GameCamera>();
    }

    void Start()
    {
        StartGame();
    }

    public void StartGame()
    {
        if(GameRunning) return;
        GameRunning = true;

        gameCamera.ResetCamera();
        GameInput.ResetInput();
        GameInput.AcceptInputs = true;

        spawnManager.StartSpawning();

        blockTower.ResetTower();

        scoreManager.ResetScore();
        scoreManager.IsCounting = true;
    }

    public void StopGame()
    {
        if(!GameRunning) return;

        scoreManager.IsCounting = false;
        GameRunning = false;

        spawnManager.StopSpawning();

        mainMenuButton.SetActive(true);
    }
}

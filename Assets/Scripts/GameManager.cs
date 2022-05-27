using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance {get; private set;}

    [SerializeField]
    BlockSpawner blockSpawner;
    [SerializeField]
    BlockTower blockTower;

    MainMenuManager mainMenuManager;
    ScoreManager scoreManager;
    GameCamera gameCamera;

    public bool GameRunning {get; private set;}
    public ScoreManager ScoreManager {get {return scoreManager;}}

    // TODO make a GAMEMANAGER as a STATIC CLASS not inheriting from monobehaviour
    void Awake()
    {
        if(instance is null)
            instance = this;
        else
            Destroy(gameObject);

        GameRunning = false;

        scoreManager = GetComponent<ScoreManager>();
        mainMenuManager = GetComponent<MainMenuManager>();
        gameCamera = FindObjectOfType<GameCamera>();

        // TODO change this to be native 
        Screen.SetResolution(Screen.height * 9 / 16, Screen.height, true);


        // TODO REMOVE FOLLOWING LINES OF TEST
        GameInput.AcceptInputs = true;
    }


    public void StartGame()
    {
        if(GameRunning)
            return;

        blockSpawner.RemoveAllBlocksSpawned();
        blockTower.ResetTower();
        

        mainMenuManager.ShowGameUI();
        scoreManager.ResetScore();
        
        gameCamera.ResetCamera();

        blockSpawner.StartSpawning();

        GameInput.AcceptInputs = true;
        scoreManager.IsCounting = true;
        GameRunning = true;
    }

    public void StopGame()
    {
        if(!GameRunning)
            return;

        scoreManager.IsCounting = false;
        GameRunning = false;

        mainMenuManager.ShowLoseUI();
        blockSpawner.StopSpawning();
    }

    public DeviceInput AddDeviceInput()
    {
        DeviceInput deviceInput;
        #if UNITY_IOS || UNITY_ANDROID
        deviceInput = gameObject.AddComponent<MobileInput>();
        #else
        deviceInput = gameObject.AddComponent<DesktopInput>();
        #endif

        return deviceInput;
    }
}

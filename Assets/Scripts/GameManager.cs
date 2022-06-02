using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance {get; private set;}

    void Awake()
    {
        if(instance is null)
            instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this.gameObject);

        // TODO change this to be native 
        Screen.SetResolution(Screen.height * 9 / 16, Screen.height, true);
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

    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene("MainMenuScene");
    }

    public void LoadGameplayScene()
    {
        SceneManager.LoadScene("GameplayScene");
    }
}

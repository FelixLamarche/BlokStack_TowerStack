using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] mainMenuElements;

    [SerializeField]
    GameObject[] gameOptionsElements;

    public void Awake()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        ChangeStateElements(mainMenuElements, true);
        ChangeStateElements(gameOptionsElements, false);
    }

    public void ShowGameOptions()
    {
        ChangeStateElements(mainMenuElements, false);
        ChangeStateElements(gameOptionsElements, true);
    }


    void ChangeStateElements(GameObject[] elements, bool state)
    {
        for(int i = 0; i < elements.Length; i++)
        {
            elements[i].SetActive(state);
        }
    }
}

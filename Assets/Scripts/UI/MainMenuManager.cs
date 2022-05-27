using UnityEngine;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField]
    GameObject[] mainMenuElements;

    [SerializeField]
    GameObject[] gameElements;

    [SerializeField]
    GameObject[] onLosingElements;


    public void Awake()
    {
        ShowMainMenu();
    }

    public void ShowMainMenu()
    {
        ChangeStateElements(gameElements, false);
        ChangeStateElements(onLosingElements, false);
        ChangeStateElements(mainMenuElements, true);
    }

    public void ShowGameUI()
    {
        ChangeStateElements(onLosingElements, false);
        ChangeStateElements(mainMenuElements, false);
        ChangeStateElements(gameElements, true);
    }

    public void ShowLoseUI()
    {
        ChangeStateElements(mainMenuElements, false);
        ChangeStateElements(gameElements, false);
        ChangeStateElements(onLosingElements, true);
    }

    void ChangeStateElements(GameObject[] elements, bool state)
    {
        for(int i = 0; i < elements.Length; i++)
        {
            elements[i].SetActive(state);
        }
    }

    // TODO fade in and out the UI Elements
    void FadeIn()
    {

    }
}

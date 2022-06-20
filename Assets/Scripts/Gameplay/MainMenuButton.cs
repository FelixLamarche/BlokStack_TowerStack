using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenuButton : MonoBehaviour
{
    public void GoToMainMenu()
    {
        GameManager.Instance.LoadMainMenuScene();
    }
}

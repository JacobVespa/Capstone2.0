using UnityEngine;
using UnityEngine.UI;

public class MainMenu_UI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;

    [SerializeField] SoundManager soundManager;

    public void StartButtonClicked()
    {
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager != null)
        {
            gameflowManager.StartScrollerLevel();
            soundManager.StopBGM();
        }
        else
        {
            Debug.LogError("GameflowManager not found in the scene.");
        }
    }

    public void OptionButtonClicked()
    {
        mainMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void BackButtonClicked()
    {
        mainMenu.SetActive(true);
        optionsMenu.SetActive(false);
    }
}

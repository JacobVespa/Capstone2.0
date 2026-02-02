using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.UI;

public class MainMenu_UI : MonoBehaviour
{
    [SerializeField] private GameObject mainMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject startButton;
    [SerializeField] private GameObject optionsTabButton;

    [SerializeField] SoundManager soundManager;

    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(startButton);
    }

    public void StartButtonClicked()
    {
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager != null)
        {
            gameflowManager.StartScrollerLevel();
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

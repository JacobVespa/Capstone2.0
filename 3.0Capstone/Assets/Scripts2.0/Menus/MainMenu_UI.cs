using UnityEngine;
using UnityEngine.UI;

public class MainMenu_UI : MonoBehaviour
{
    [SerializeField] private GameObject options;
    [SerializeField] private GameObject menu;

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
        menu.SetActive(false);
        options.SetActive(true);
    }

    public void BackButtonClicked()
    {
        menu.SetActive(true);
        options.SetActive(false);
    }

}

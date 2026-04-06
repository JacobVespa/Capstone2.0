using UnityEngine;
using UnityEngine.EventSystems;

public class BoardToMainMenu : MonoBehaviour
{
    public void Start()
    {
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(this.gameObject);
        }
    }

    public void OnQuitButton()
    {
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager != null)
        {
            GameManager.Instance.ResetStats();
            gameflowManager.EndLevel();
            LevelManager.Instance.HideEndScreen(9);
        }
    }
}

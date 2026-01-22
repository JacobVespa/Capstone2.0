using UnityEngine;

public class NavigationMenu_UI : MonoBehaviour
{
    public void OnContinueButton()
    {
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager != null)
        {
            gameflowManager.StartDefenseLevel();
            LevelManager.Instance.HideEndScreen(GameManager.Instance.NavigationScreenIndex);
        }
    }
}

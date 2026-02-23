using UnityEngine;
using UnityEngine.EventSystems;

public class NavigationMenu_UI : MonoBehaviour
{
    //[SerializeField] private GameObject button;
    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        //EventSystem.current.SetSelectedGameObject(button);
    }

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

using UnityEngine;
using UnityEngine.EventSystems;

public class VictoryMenu_UI : MonoBehaviour
{
    [SerializeField] private GameObject returnButton;


    private void OnEnable()
    {
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(returnButton);
    }

    public void OnQuitButton()
    {
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager != null)
        {
            GameManager.Instance.ResetStats();
            gameflowManager.EndLevel();
            LevelManager.Instance.HideEndScreen(7);
        }
    }

}

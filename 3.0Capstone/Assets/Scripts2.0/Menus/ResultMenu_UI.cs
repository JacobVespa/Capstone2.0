using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResultMenu_UI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text resultText;
    [SerializeField] private TMP_Text shardsText;
    [SerializeField] private TMP_Text killsText;
    [SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject mainMenuButton;

    [Header("Messages")]
    private string winMessage = "Cave Complete!";
    private string loseMessage = "Cave CRASHED!";

    private void OnEnable()
    {
        UpdateResultUI();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(restartButton);
    }

    private void UpdateResultUI()
    {
        if (GameManager.Instance.GameOverStatus)
        {
            resultText.text = loseMessage;
        }
        else
        {
            resultText.text = winMessage;
        }

        shardsText.text = "Shards Collected: " + GameManager.Instance.Shards.ToString();
        killsText.text = "Enemies Defeated: " + GameManager.Instance.Kills.ToString();
    }

    public void OnRestartButton()
    {
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager != null)
        {
            gameflowManager.RestartLevel();
            LevelManager.Instance.HideEndScreen(GameManager.Instance.ResultScreenIndex);
        }
    }

    public void OnQuitButton()
    {
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager != null)
        {
            gameflowManager.EndLevel();
            LevelManager.Instance.HideEndScreen(GameManager.Instance.ResultScreenIndex);
        }
    }
}

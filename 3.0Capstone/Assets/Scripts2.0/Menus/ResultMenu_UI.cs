using System.Collections;
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

    private int totalShards = 0;
    private int totalKills = 0;

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

        //Count from zero slowly and ramp up speed

        totalShards = GameManager.Instance.Shards;
        totalKills = GameManager.Instance.Kills;
        CountUpScore();
    }

    private void CountUpScore()
    {
        StartCoroutine(Counter());
    }

    private float multiplier = 1.0f;
    private int tempShards = 0;
    private int tempKills = 0;

    private IEnumerator Counter()
    {
        while (tempShards < totalShards || tempKills < totalKills)
        {
            if (tempShards < totalShards)
            {
                shardsText.text = "Shards: " + (tempShards++).ToString();
            }
            if (tempKills < totalKills)
            {
                killsText.text = "Kills: " + (tempKills++).ToString();
            }

            yield return new WaitForSecondsRealtime(0.4f * multiplier);
            multiplier -= 0.05f;
        }
    }

    public void OnRestartButton()
    {
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager != null)
        {
            GameManager.Instance.ResetStats();
            gameflowManager.RestartLevel();
            LevelManager.Instance.HideEndScreen(GameManager.Instance.ResultScreenIndex);
        }
    }

    public void OnQuitButton()
    {
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager != null)
        {
            GameManager.Instance.ResetStats();
            gameflowManager.EndLevel();
            LevelManager.Instance.HideEndScreen(GameManager.Instance.ResultScreenIndex);
        }
    }
}

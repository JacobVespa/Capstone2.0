using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

public class ResultMenu_UI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private TMP_Text shardsText;
    [SerializeField] private TMP_Text killsText;
    //[SerializeField] private GameObject restartButton;
    [SerializeField] private GameObject mainMenuButton;

    private int totalShards = 0;
    private int totalKills = 0;

    private void Start()
    {
        UpdateResultUI();
        if (EventSystem.current != null)
        {
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(mainMenuButton);
        }
    }

    private void UpdateResultUI()
    {
        if (GameManager.Instance == null) return;

        totalShards = GameManager.Instance.Shards;
        totalKills = GameManager.Instance.Kills;
        StartCoroutine(DelayedCountUp());
    }

    private float multiplier = 1.0f;
    private int tempShards = 0;
    private int tempKills = 0;

    private IEnumerator DelayedCountUp()
    {
        yield return null; // wait one frame for scene to settle
        CountUpScore();
    }

    private void CountUpScore()
    {
        StopAllCoroutines();
        tempShards = 0;
        tempKills = 0;
        multiplier = 1.0f;
        StartCoroutine(Counter());
    }

    private IEnumerator Counter()
    {
        while (tempShards < totalShards || tempKills < totalKills)
        {
            if (tempShards < totalShards)
            {
                tempShards++;
                shardsText.text = "Shards: " + tempShards.ToString();
            }
            if (tempKills < totalKills)
            {
                tempKills++;
                killsText.text = "Kills: " + tempKills.ToString();
            }

            yield return new WaitForSecondsRealtime(0.4f * multiplier);
            multiplier = Mathf.Max(0.05f, multiplier - 0.05f);
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
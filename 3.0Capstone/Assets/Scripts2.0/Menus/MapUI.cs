using UnityEngine;
using UnityEngine.UI;

public class MapUI : MonoBehaviour
{
    [Header("Progress Bar")]
    [SerializeField] private Slider progressBar;

    [Header("Defense Checkpoints")]
    [SerializeField] private RectTransform checkpointContainer;
    [SerializeField] private Sprite checkpointImage;
    [SerializeField] private Vector2 checkpointSize = new Vector2(16f, 16f);

    private void Update()
    {
        if (GameManager.Instance == null) return;

        LevelProgress();

        if (progressBar.value >= progressBar.maxValue)
            progressBar.enabled = false;
    }

    private void LevelProgress()
    {
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager == null || gameflowManager.CurrentLevel == null) return;

        if (gameflowManager.CurrentLevel is ScrollerLevel)
        {
            ScrollerProgress(gameflowManager);
            progressBar.value = GameManager.Instance.GameTime;
        }
        else if (gameflowManager.CurrentLevel is DefenceLevel)
        {
            DefenseProgress(gameflowManager);
        }
    }

    private void ScrollerProgress(GameflowManager gameflowManager)
    {
        float levelDuration = gameflowManager.CurrentLevel.Duration;
        if (levelDuration == 0 || progressBar.maxValue == levelDuration) return;

        progressBar.maxValue = levelDuration;
    }

    private void DefenseProgress(GameflowManager gameflowManager)
    {
        DefenceLevel defenceLevel = gameflowManager.CurrentLevel as DefenceLevel;
        if (defenceLevel == null) return;

        int finalWave = defenceLevel.FinalWave;
        if (finalWave == 0) return;

        // Set up slider + checkpoints once
        if (progressBar.maxValue != finalWave)
        {
            progressBar.maxValue = finalWave;
            SpawnDefenseCheckpoints(finalWave);
        }

        progressBar.value = defenceLevel.WavesCompleted;
    }

    private void SpawnDefenseCheckpoints(int finalWave)
    {
        // Clear existing checkpoints
        foreach (Transform child in checkpointContainer)
            Destroy(child.gameObject);

        float width = checkpointContainer.rect.width;

        for (int i = 1; i <= finalWave; i++)
        {
            // Create UI object
            GameObject checkpoint = new GameObject($"Checkpoint_{i}", typeof(RectTransform), typeof(Image));
            checkpoint.transform.SetParent(checkpointContainer, false);

            Image image = checkpoint.GetComponent<Image>();
            image.sprite = checkpointImage;
            image.raycastTarget = false;

            RectTransform rect = checkpoint.GetComponent<RectTransform>();
            rect.sizeDelta = checkpointSize;
            rect.anchorMin = new Vector2(0f, 0.5f);
            rect.anchorMax = new Vector2(0f, 0.5f);
            rect.pivot = new Vector2(0.5f, 0.5f);

            float normalized = (float)i / finalWave;
            float xPos = normalized * width;

            rect.anchoredPosition = new Vector2(xPos, 0f);
        }
    }
}
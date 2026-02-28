using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MapButton : MonoBehaviour
{
    Image locationImage;
    TMP_Text locationText;
    private int levelIndex;
    public int LevelIndex
    {
        set => levelIndex = value;
    }

    private bool visited = false;
    public bool Visited => visited;

    void Awake()
    {
        locationImage = GetComponent<Image>();
        locationText = GetComponentInChildren<TMP_Text>();

        if (locationImage != null)
            locationImage.color = Color.white;

        if (locationText != null)
            locationText.text = "Unknown";
    }

    public void SetLocation(Sprite sprite, string title)
    {
        if (locationImage != null)
            locationImage.sprite = sprite;

        if (locationText != null)
            locationText.text = title;
    }

    public void VisitLocation()
    {
        if (visited) return;

        visited = true;

        if (locationText != null)
            locationText.text = "Complete";

        if (CaveMap.Instance != null)
            CaveMap.Instance.OnNodeVisited(GetComponent<Button>());
    }

    /// <summary>
    /// Restores visited appearance without triggering map logic.
    /// Used when restoring state after the scene reloads.
    /// </summary>
    public void ForceVisited()
    {
        visited = true;

        if (locationText != null)
            locationText.text = "Complete";
    }

    // Only wire this in the inspector OnClick
    public void StartLevel()
    {
        VisitLocation(); // Save state before the scene changes

        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager != null)
        {
            if (levelIndex == 0)
                gameflowManager.StartScrollerLevel();
            else if (levelIndex == 1)
                gameflowManager.StartDefenseLevel();
            else if (levelIndex == 2)
                gameflowManager.StartGoldRushLevel();
            else
                Debug.LogWarning("No level assigned to this button.");
        }
        else
        {
            Debug.LogError("GameflowManager not found in the scene.");
        }
    }
}
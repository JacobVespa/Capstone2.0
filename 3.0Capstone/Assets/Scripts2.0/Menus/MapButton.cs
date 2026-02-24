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

    private static CaveMap caveMap;

    void Awake()
    {
        locationImage = GetComponent<Image>();
        locationText = GetComponentInChildren<TMP_Text>();

        if (locationImage != null)
            locationImage.color = Color.white;

        if (locationText != null)
            locationText.text = "Unknown";

        if (caveMap == null)
            caveMap = FindFirstObjectByType<CaveMap>();
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

        if (caveMap != null)
        {
            caveMap.UpdateActiveLines(GetComponent<Button>());
            caveMap.ProgressMap();
        }
    }

    public void StartLevel()
    {
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager != null)
        {
            if (levelIndex == 0)
                gameflowManager.StartScrollerLevel();
            else if (levelIndex == 1)
                gameflowManager.StartDefenseLevel();
            else
                Debug.LogWarning("No level assigned to this button.");
        }
        else
        {
            Debug.LogError("GameflowManager not found in the scene.");
        }
    }
}
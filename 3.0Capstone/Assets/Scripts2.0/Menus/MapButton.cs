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
        locationImage.sprite = sprite;

        if (locationText != null)
            locationText.text = title;
    }

    public void VisitLocation()
    {
        visited = true;

        if (locationText != null)
            locationText.text = "Complete";
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
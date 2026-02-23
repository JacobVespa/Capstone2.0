using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CaveMap : MonoBehaviour
{
    [Header("Map Settings")]
    [SerializeField] GameObject prefab;
    [SerializeField] RectTransform mapContainer;
    [SerializeField] private float spacingX = 150f;
    [SerializeField] private float spacingY = 150f;

    [Header("Images")]
    [SerializeField] Sprite searchSprite;
    [SerializeField] Sprite extractSprite;

    private List<List<Button>> levelTree;

    private int levels = 3;
    private int depth = 3;

    void Start()
    {
        CreateMap();
    }

    private void CreateMap()
    {
        levelTree = new List<List<Button>>();

        // Create depth + 1 rows so the visited row is always visible at the bottom
        for (int i = 0; i < depth + 1; i++)
        {
            List<Button> levelButtons = new List<Button>();

            for (int j = 0; j < levels; j++)
            {
                GameObject levelButton = CreateLevel(col: j, row: i);
                levelButtons.Add(levelButton.GetComponent<Button>());
            }

            levelTree.Add(levelButtons);
        }

        UpdateButtonAccess();
    }

    private void ProgressMap()
    {
        // Drop the oldest row (now scrolled out of view)
        foreach (Button button in levelTree[0])
        {
            Destroy(button.gameObject);
        }

        levelTree.RemoveAt(0);

        // Shift all remaining rows down by one step
        foreach (List<Button> row in levelTree)
        {
            foreach (Button button in row)
            {
                button.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, spacingY);
            }
        }

        // Add a new row at the top
        List<Button> newRow = new List<Button>();

        for (int j = 0; j < levels; j++)
        {
            GameObject levelButton = CreateLevel(col: j, row: depth);
            newRow.Add(levelButton.GetComponent<Button>());
        }

        levelTree.Add(newRow);
        UpdateButtonAccess();
    }

    private GameObject CreateLevel(int col, int row)
    {
        GameObject levelButton = Instantiate(prefab, mapContainer);
        RectTransform rectTransform = levelButton.GetComponent<RectTransform>();

        float totalWidth = (levels - 1) * spacingX;
        float x = (col * spacingX) - (totalWidth / 2f);

        float totalHeight = (depth - 1) * spacingY;
        float y = (row * spacingY) - (totalHeight / 2f);

        rectTransform.anchoredPosition = new Vector2(x, y);

        MapButton mapButton = levelButton.GetComponent<MapButton>();

        int rng = Random.Range(1, 3); // 1 or 2

        if (rng == 1)
        {
            mapButton.SetLocation(searchSprite, "Search");
            mapButton.LevelIndex = 0;
        }
        else if (rng == 2)
        {
            mapButton.SetLocation(extractSprite, "Extract");
            mapButton.LevelIndex = 1;
        }

        return levelButton;
    }

    private void UpdateButtonAccess()
    {
        foreach (List<Button> row in levelTree)
        {
            foreach (Button button in row)
            {
                button.interactable = false;
            }
        }

        // Row 0 is the last visited row (non-interactable, just visible)
        // Row 1 is the current active row the player chooses from
        bool visitedRowExists = levelTree[0].Exists(b => b.GetComponent<MapButton>().Visited);

        if (visitedRowExists)
        {
            foreach (Button button in levelTree[1])
            {
                button.interactable = true;
            }
        }
        else
        {
            // Nothing visited yet, let the player pick from row 0
            foreach (Button button in levelTree[0])
            {
                button.interactable = true;
            }
        }
    }
}
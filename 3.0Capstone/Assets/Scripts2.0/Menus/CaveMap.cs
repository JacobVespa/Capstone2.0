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

    private static CaveMap instance;

    void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        CreateMap();
    }

    private void CreateMap()
    {
        levelTree = new List<List<Button>>();

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

    public void ProgressMap()
    {
        foreach (Button button in levelTree[0])
            Destroy(button.gameObject);

        levelTree.RemoveAt(0);

        foreach (List<Button> row in levelTree)
        {
            foreach (Button button in row)
            {
                button.GetComponent<RectTransform>().anchoredPosition -= new Vector2(0, spacingY);
            }
        }

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

        Sprite[] sprites = { searchSprite, extractSprite };
        string[] names = { "Search", "Extract" };
        int rng = Random.Range(0, sprites.Length);
        mapButton.SetLocation(sprites[rng], names[rng]);
        mapButton.LevelIndex = rng;

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
            foreach (Button button in levelTree[0])
            {
                button.interactable = true;
            }
        }
    }
}
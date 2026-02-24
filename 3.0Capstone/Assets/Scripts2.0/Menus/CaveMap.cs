using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

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

    [Header("Lines")]
    [SerializeField] private Color defaultLineColor = Color.gray;
    [SerializeField] private Color activeLineColor = Color.green;
    [SerializeField] private float lineThickness = 4f;

    private List<List<Button>> levelTree;
    private List<int> rowWidths;
    private List<List<List<Image>>> connectionLines;

    public Button firstSelectedButton;
    private List<int> rowWidths;
    private List<List<List<Image>>> connectionLines;

    private int levels = 3;
    private int depth = 3;

    // Tracks the global row index so alternating pattern is consistent across ProgressMap calls
    private int globalRowIndex = 0;

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

        firstSelectedButton = levelTree[0].First(); //Might need to change this later to accomodate for progression?

        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
       
    }

    private int GetNodeCountForRow(int globalRow)
    {
        // Row 0 always equals base level count, then alternates +0 and +1
        return globalRow == 0 ? levels : levels + (globalRow % 2 == 0 ? 0 : 1);
    }

    private int GetNodeCountForRow(int globalRow)
    {
        // Row 0 always equals base level count, then alternates +0 and +1
        return globalRow == 0 ? levels : levels + (globalRow % 2 == 0 ? 0 : 1);
    }

    private void CreateMap()
    {
        levelTree = new List<List<Button>>();
        rowWidths = new List<int>();
        connectionLines = new List<List<List<Image>>>();
        globalRowIndex = 0;

        for (int i = 0; i < depth + 1; i++)
        {
            int nodeCount = GetNodeCountForRow(globalRowIndex);
            rowWidths.Add(nodeCount);
            globalRowIndex++;

            List<Button> levelButtons = new List<Button>();

            for (int j = 0; j < nodeCount; j++)
            {
                GameObject levelButton = CreateLevel(col: j, row: i, rowWidth: nodeCount);
                levelButtons.Add(levelButton.GetComponent<Button>());
            }

            levelTree.Add(levelButtons);
        }

        DrawAllConnections();
        UpdateButtonAccess();
    }

    public void ProgressMap()
    {
        // Destroy lines for the oldest row
        if (connectionLines.Count > 0)
        {
            foreach (List<Image> lineGroup in connectionLines[0])
                foreach (Image line in lineGroup)
                    if (line != null) Destroy(line.gameObject);

            connectionLines.RemoveAt(0);
        }

        // Destroy oldest row of buttons
        foreach (Button button in levelTree[0])
            Destroy(button.gameObject);

        levelTree.RemoveAt(0);
        rowWidths.RemoveAt(0);

        // Recalculate positions for remaining rows
        for (int i = 0; i < levelTree.Count; i++)
        {
            int nodeCount = rowWidths[i];
            for (int j = 0; j < levelTree[i].Count; j++)
            {
                levelTree[i][j].GetComponent<RectTransform>().anchoredPosition = GetNodePosition(col: j, row: i, rowWidth: nodeCount);
            }
        }

        // Destroy remaining lines to redraw
        foreach (List<List<Image>> rowLines in connectionLines)
            foreach (List<Image> lineGroup in rowLines)
                foreach (Image line in lineGroup)
                    if (line != null) Destroy(line.gameObject);

        connectionLines.Clear();

        // Add new row at the top using the global index to maintain alternating pattern
        int newNodeCount = GetNodeCountForRow(globalRowIndex);
        rowWidths.Add(newNodeCount);
        globalRowIndex++;

        List<Button> newRow = new List<Button>();
        for (int j = 0; j < newNodeCount; j++)
        {
            GameObject levelButton = CreateLevel(col: j, row: levelTree.Count, rowWidth: newNodeCount);
            newRow.Add(levelButton.GetComponent<Button>());
        }

        levelTree.Add(newRow);

        DrawAllConnections();
        UpdateButtonAccess();
    }

    private Vector2 GetNodePosition(int col, int row, int rowWidth)
    {
        float totalWidth = (rowWidth - 1) * spacingX;
        float x = (col * spacingX) - (totalWidth / 2f);

        float totalHeight = (depth - 1) * spacingY;
        float y = (row * spacingY) - (totalHeight / 2f);

        return new Vector2(x, y);
    }

    private GameObject CreateLevel(int col, int row, int rowWidth)
    {
        GameObject levelButton = Instantiate(prefab, mapContainer);
        RectTransform rectTransform = levelButton.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = GetNodePosition(col, row, rowWidth);

        MapButton mapButton = levelButton.GetComponent<MapButton>();

        Sprite[] sprites = { searchSprite, extractSprite };
        string[] names = { "Search", "Extract" };
        int rng = Random.Range(0, sprites.Length);
        mapButton.SetLocation(sprites[rng], names[rng]);
        mapButton.LevelIndex = rng;

        return levelButton;
    }

    private void DrawAllConnections()
    {
        connectionLines.Clear();

        for (int i = 0; i < levelTree.Count - 1; i++)
        {
            int currentWidth = rowWidths[i];
            int nextWidth = rowWidths[i + 1];
            List<List<Image>> rowLineGroups = new List<List<Image>>();

            for (int j = 0; j < levelTree[i].Count; j++)
            {
                List<Image> linesFromNode = new List<Image>();
                List<int> neighbours = GetNeighbourIndices(col: j, currentRow: i, currentRowWidth: currentWidth, nextRowWidth: nextWidth);

                foreach (int n in neighbours)
                {
                    if (n >= 0 && n < levelTree[i + 1].Count)
                    {
                        Vector2 from = levelTree[i][j].GetComponent<RectTransform>().anchoredPosition;
                        Vector2 to = levelTree[i + 1][n].GetComponent<RectTransform>().anchoredPosition;
                        Image line = DrawLine(from, to, defaultLineColor);
                        linesFromNode.Add(line);
                    }
                }

                rowLineGroups.Add(linesFromNode);
            }

            connectionLines.Add(rowLineGroups);
        }
    }

    private List<int> GetNeighbourIndices(int col, int currentRow, int currentRowWidth, int nextRowWidth)
    {
        List<int> neighbours = new List<int>();

        if (currentRowWidth > nextRowWidth)
        {
            // Wide to narrow: each node connects to the one at same col and col-1 in narrow row
            neighbours.Add(col - 1);
            neighbours.Add(col);
        }
        else
        {
            // Narrow to wide: each node connects to same col and col+1 in wide row
            neighbours.Add(col);
            neighbours.Add(col + 1);
        }

        return neighbours;
    }

    private Image DrawLine(Vector2 from, Vector2 to, Color color)
    {
        GameObject lineObj = new GameObject("Line", typeof(RectTransform), typeof(Image));
        lineObj.transform.SetParent(mapContainer, false);
        lineObj.transform.SetAsFirstSibling();

        Image lineImage = lineObj.GetComponent<Image>();
        lineImage.color = color;
        lineImage.raycastTarget = false; // <-- Add this line

        RectTransform rt = lineObj.GetComponent<RectTransform>();
        Vector2 direction = to - from;
        float distance = direction.magnitude;

        rt.sizeDelta = new Vector2(distance, lineThickness);
        rt.anchoredPosition = from + direction * 0.5f;
        rt.localRotation = Quaternion.FromToRotation(Vector3.right, new Vector3(direction.x, direction.y, 0));

        return lineImage;
    }

    public void UpdateActiveLines(Button visitedButton)
    {
        for (int i = 0; i < levelTree.Count - 1; i++)
        {
            for (int j = 0; j < levelTree[i].Count; j++)
            {
                if (levelTree[i][j] == visitedButton && i < connectionLines.Count)
                {
                    foreach (Image line in connectionLines[i][j])
                        line.color = activeLineColor;
                }
            }
        }
    }

    private void UpdateButtonAccess()
    {
        foreach (List<Button> row in levelTree)
            foreach (Button button in row)
                button.interactable = false;

        bool visitedRowExists = levelTree[0].Exists(b => b.GetComponent<MapButton>().Visited);

        if (visitedRowExists && levelTree.Count > 1)
        {
            for (int j = 0; j < levelTree[0].Count; j++)
            {
                if (levelTree[0][j].GetComponent<MapButton>().Visited)
                {
                    List<int> neighbours = GetNeighbourIndices(col: j, currentRow: 0, currentRowWidth: rowWidths[0], nextRowWidth: rowWidths[1]);
                    foreach (int n in neighbours)
                    {
                        if (n >= 0 && n < levelTree[1].Count)
                            levelTree[1][n].interactable = true;
                    }
                    break;
                }
            }
        }
        else if (!visitedRowExists)
        {
            // All nodes on the first layer are accessible at the start
            foreach (Button button in levelTree[0])
                button.interactable = true;
        }
    }
}
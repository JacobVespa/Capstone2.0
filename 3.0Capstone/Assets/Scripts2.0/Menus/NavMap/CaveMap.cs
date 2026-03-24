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

    [Header("City Layout Jitter")]
    [Tooltip("Max random X offset applied per node (city-block variance)")]
    [SerializeField] private float jitterX = 45f;
    [Tooltip("Max random Y offset applied per node (city-block variance)")]
    [SerializeField] private float jitterY = 30f;
    [Tooltip("Chance (0-1) a node gets an extra nudge to simulate irregular blocks")]
    [SerializeField] private float extraNudgeChance = 0.3f;
    [SerializeField] private float extraNudgeStrength = 25f;

    [Header("Images")]
    [SerializeField] Sprite searchSprite;
    [SerializeField] Sprite extractSprite;
    [SerializeField] Sprite bonusSprite;
    [SerializeField] Sprite mysterySprite;

    [Header("Lines")]
    [SerializeField] private Color defaultLineColor = Color.gray;
    [SerializeField] private Color activeLineColor = Color.green;
    [SerializeField] private float lineThickness = 4f;

    private List<List<Button>> levelTree;
    private List<int> rowWidths;
    private List<List<List<Image>>> connectionLines;
    private List<List<Vector2>> nodePositions;

    public Button firstSelectedButton;

    private int levels = 3;
    private int depth = 3;

    public static CaveMap Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        CaveMapState state = CaveMapState.Instance;

        if (state != null && state.MapSeed == -1)
            state.GenerateNewSeed();

        CreateMap();

        if (GameManager.Instance != null)
            GameManager.Instance.MaxDepth = depth;

        if (state != null && state.HasVisitedNode)
        {
            RestoreState(state);
        }
        else
        {
            UpdateButtonAccess();

            firstSelectedButton = levelTree[0].First();
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);

            PlaceRigAtStart();
        }
    }

    public Vector2 GetButtonPosition(Button button)
    {
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        RectTransform rigParent = SpriteRig.Instance.GetRigParent();

        Vector2 screenPoint = RectTransformUtility.WorldToScreenPoint(null, buttonRect.position);
        RectTransformUtility.ScreenPointToLocalPointInRectangle(rigParent, screenPoint, null, out Vector2 localPoint);
        return localPoint;
    }

    private void PlaceRigAtStart()
    {
        if (SpriteRig.Instance == null || levelTree.Count == 0) return;

        float centreX = 0f;
        float bottomY = nodePositions[0].Average(p => p.y) - spacingY * 0.8f;

        SpriteRig.Instance.SnapTo(new Vector2(centreX, bottomY + (bottomY * 0.5f)));
    }

    private void RestoreState(CaveMapState state)
    {
        Vector2Int last = state.LastVisited;

        foreach (Vector2Int node in state.VisitedPath)
        {
            int row = node.y;
            int col = node.x;

            if (row < 0 || row >= levelTree.Count || col < 0 || col >= levelTree[row].Count)
                continue;

            MapButton mb = levelTree[row][col].GetComponent<MapButton>();

            // Last visited node shows "Onward!", all previous nodes show "Complete"
            if (node == last)
                mb.ForceVisitedLast();
            else
                mb.ForceVisited();
        }

        RestoreVisitedLines(state.VisitedPath);
        UpdateButtonAccess();

        if (last.y >= 0 && last.y < levelTree.Count && last.x >= 0 && last.x < levelTree[last.y].Count)
        {
            Vector2 lastPos = GetButtonPosition(levelTree[last.y][last.x]);
            SpriteRig.Instance?.SnapTo(lastPos);
        }

        int nextRow = last.y + 1;
        if (nextRow < levelTree.Count)
        {
            Button firstUnlocked = levelTree[nextRow].FirstOrDefault(b => b.interactable);
            if (firstUnlocked != null)
            {
                firstSelectedButton = firstUnlocked;
                EventSystem.current.SetSelectedGameObject(null);
                EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
            }
        }
    }

    private void RestoreVisitedLines(List<Vector2Int> path)
    {
        for (int i = 0; i < path.Count - 1; i++)
        {
            int fromRow = path[i].y;
            int fromCol = path[i].x;

            if (fromRow < 0 || fromRow >= levelTree.Count || fromCol < 0 || fromCol >= levelTree[fromRow].Count)
                continue;
            if (fromRow >= connectionLines.Count)
                continue;

            ColourLinesFrom(fromRow, fromCol);
        }

        if (path.Count > 0)
        {
            Vector2Int last = path[path.Count - 1];
            ColourLinesFrom(last.y, last.x);
        }
    }

    private void ColourLinesFrom(int row, int col)
    {
        if (row < 0 || row >= connectionLines.Count) return;
        if (col < 0 || col >= connectionLines[row].Count) return;

        List<int> neighbours = GetNeighbourIndices(
            col: col,
            currentRow: row,
            currentRowWidth: rowWidths[row],
            nextRowWidth: rowWidths[row + 1]
        );

        for (int lineIdx = 0; lineIdx < connectionLines[row][col].Count; lineIdx++)
        {
            if (lineIdx >= neighbours.Count) break;
            int neighbourCol = neighbours[lineIdx];

            if (neighbourCol < 0 || neighbourCol >= levelTree[row + 1].Count) continue;

            bool neighbourVisited = levelTree[row + 1][neighbourCol].GetComponent<MapButton>().Visited;
            if (neighbourVisited)
                connectionLines[row][col][lineIdx].color = activeLineColor;
        }
    }

    private int GetNodeCountForRow(int globalRow)
    {
        return globalRow == 0 ? levels : levels + (globalRow % 2 == 0 ? 0 : 1);
    }

    private void GenerateCityPositions()
    {
        nodePositions = new List<List<Vector2>>();

        float totalHeight = (depth - 1) * spacingY;

        for (int row = 0; row < rowWidths.Count; row++)
        {
            int nodeCount = rowWidths[row];
            List<Vector2> rowPos = new List<Vector2>();

            float totalWidth = (nodeCount - 1) * spacingX;
            float baseY = (row * spacingY) - (totalHeight / 2f);

            for (int col = 0; col < nodeCount; col++)
            {
                float baseX = (col * spacingX) - (totalWidth / 2f);

                float offsetX = Random.Range(-jitterX, jitterX);
                float offsetY = Random.Range(-jitterY, jitterY);

                if (Random.value < extraNudgeChance)
                {
                    offsetX += Random.Range(-extraNudgeStrength, extraNudgeStrength);
                    offsetY += Random.Range(-extraNudgeStrength * 0.5f, extraNudgeStrength * 0.5f);
                }

                rowPos.Add(new Vector2(baseX + offsetX, baseY + offsetY));
            }

            rowPos.Sort((a, b) => a.x.CompareTo(b.x));
            nodePositions.Add(rowPos);
        }
    }

    private void CreateMap()
    {
        levelTree = new List<List<Button>>();
        rowWidths = new List<int>();
        connectionLines = new List<List<List<Image>>>();

        int localGlobalRow = CaveMapState.Instance != null ? CaveMapState.Instance.GlobalRowIndex : 0;
        int seed = CaveMapState.Instance != null ? CaveMapState.Instance.MapSeed : 0;
        Random.InitState(seed);

        for (int i = 0; i < depth + 1; i++)
        {
            int nodeCount = GetNodeCountForRow(localGlobalRow);
            rowWidths.Add(nodeCount);
            localGlobalRow++;
        }

        GenerateCityPositions();

        for (int i = 0; i < depth + 1; i++)
        {
            bonusCount = 0;
            List<Button> levelButtons = new List<Button>();

            for (int j = 0; j < rowWidths[i]; j++)
            {
                GameObject levelButton = CreateLevel(col: j, row: i);
                levelButtons.Add(levelButton.GetComponent<Button>());
            }

            levelTree.Add(levelButtons);
        }

        DrawAllConnections();
    }

    public void OnNodeVisited(Button visitedButton)
    {
        int visitedRow = -1;
        int visitedCol = -1;

        for (int i = 0; i < levelTree.Count; i++)
        {
            for (int j = 0; j < levelTree[i].Count; j++)
            {
                if (levelTree[i][j] == visitedButton)
                {
                    visitedRow = i;
                    visitedCol = j;
                    break;
                }
            }
            if (visitedRow != -1) break;
        }

        if (visitedRow == -1) return;

        if (CaveMapState.Instance != null)
            CaveMapState.Instance.AddVisitedNode(visitedRow, visitedCol);

        ColourLineToNode(visitedRow, visitedCol);

        foreach (List<Button> row in levelTree)
            foreach (Button button in row)
                button.interactable = false;
    }

    // Called by MenuCursor when a tiebreak forces a selection,
    // bypassing the normal interactable check
    public void ForceNodeVisited(Button button)
    {
        if (button == null) return;
        OnNodeVisited(button);
    }

    private void ColourLineToNode(int toRow, int toCol)
    {
        if (toRow == 0) return;

        int fromRow = toRow - 1;
        int fromCol = -1;

        for (int j = 0; j < levelTree[fromRow].Count; j++)
        {
            if (levelTree[fromRow][j].GetComponent<MapButton>().Visited)
            {
                fromCol = j;
                break;
            }
        }

        if (fromCol == -1 || fromRow >= connectionLines.Count) return;
        if (fromCol >= connectionLines[fromRow].Count) return;

        List<int> neighbours = GetNeighbourIndices(
            col: fromCol,
            currentRow: fromRow,
            currentRowWidth: rowWidths[fromRow],
            nextRowWidth: rowWidths[toRow]
        );

        for (int lineIdx = 0; lineIdx < neighbours.Count; lineIdx++)
        {
            if (neighbours[lineIdx] == toCol && lineIdx < connectionLines[fromRow][fromCol].Count)
            {
                connectionLines[fromRow][fromCol][lineIdx].color = activeLineColor;
                break;
            }
        }
    }

    private void ResetMap()
    {
        foreach (List<List<Image>> rowLines in connectionLines)
            foreach (List<Image> lineGroup in rowLines)
                foreach (Image line in lineGroup)
                    if (line != null) Destroy(line.gameObject);

        connectionLines.Clear();

        foreach (List<Button> row in levelTree)
            foreach (Button button in row)
                Destroy(button.gameObject);

        levelTree.Clear();
        rowWidths.Clear();
        nodePositions = null;

        if (CaveMapState.Instance != null)
            CaveMapState.Instance.GenerateNewSeed();

        CreateMap();
        UpdateButtonAccess();
        PlaceRigAtStart();

        firstSelectedButton = levelTree[0].First();
        EventSystem.current.SetSelectedGameObject(null);
        EventSystem.current.SetSelectedGameObject(firstSelectedButton.gameObject);
    }

    int bonusCount = 0;

    private GameObject CreateLevel(int col, int row)
    {
        GameObject levelButton = Instantiate(prefab, mapContainer);
        RectTransform rectTransform = levelButton.GetComponent<RectTransform>();
        rectTransform.anchoredPosition = nodePositions[row][col];

        MapButton mapButton = levelButton.GetComponent<MapButton>();

        Sprite[] sprites = { searchSprite, extractSprite, mysterySprite, bonusSprite };
        string[] names = { "Search", "Extract", "Mystery", "Bonus" };

        int rng = PickLevelType(row);

        mapButton.SetLocation(sprites[rng], names[rng]);
        mapButton.LevelIndex = rng;

        return levelButton;
    }

    private int PickLevelType(int row)
    {
        bool bonusAllowed = (row % 2 != 0) && (bonusCount <= 1);

        int searchWeight  = 30;
        int extractWeight = 30;
        int mysteryWeight = 20;
        int bonusWeight   = bonusAllowed ? 20 : 0;

        int total = searchWeight + extractWeight + mysteryWeight + bonusWeight;
        int roll  = Random.Range(0, total);

        if (roll < searchWeight)                                     return 0;
        if (roll < searchWeight + extractWeight)                     return 1;
        if (roll < searchWeight + extractWeight + mysteryWeight)     return 2;

        bonusCount++;
        return 3;
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
                        Vector2 from = nodePositions[i][j];
                        Vector2 to = nodePositions[i + 1][n];
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
            neighbours.Add(col - 1);
            neighbours.Add(col);
        }
        else
        {
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
        lineImage.raycastTarget = false;

        RectTransform rt = lineObj.GetComponent<RectTransform>();
        Vector2 direction = to - from;
        float distance = direction.magnitude;

        rt.sizeDelta = new Vector2(distance, lineThickness);
        rt.anchoredPosition = from + direction * 0.5f;
        rt.localRotation = Quaternion.FromToRotation(Vector3.right, new Vector3(direction.x, direction.y, 0));

        return lineImage;
    }

    public void UpdateActiveLines(Button visitedButton) { }

    private void UpdateButtonAccess()
    {
        foreach (List<Button> row in levelTree)
            foreach (Button button in row)
                button.interactable = false;

        int lastVisitedRow = -1;
        int lastVisitedCol = -1;

        for (int i = levelTree.Count - 1; i >= 0; i--)
        {
            for (int j = 0; j < levelTree[i].Count; j++)
            {
                if (levelTree[i][j].GetComponent<MapButton>().Visited)
                {
                    lastVisitedRow = i;
                    lastVisitedCol = j;
                    break;
                }
            }
            if (lastVisitedRow != -1) break;
        }

        if (lastVisitedRow == -1)
        {
            foreach (Button button in levelTree[0])
                button.interactable = true;
        }
        else if (lastVisitedRow < levelTree.Count - 1)
        {
            int nextRow = lastVisitedRow + 1;
            List<int> neighbours = GetNeighbourIndices(
                col: lastVisitedCol,
                currentRow: lastVisitedRow,
                currentRowWidth: rowWidths[lastVisitedRow],
                nextRowWidth: rowWidths[nextRow]
            );

            foreach (int n in neighbours)
            {
                if (n >= 0 && n < levelTree[nextRow].Count)
                    levelTree[nextRow][n].interactable = true;
            }
        }
    }
}
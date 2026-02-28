using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Persistent singleton that survives scene loads.
/// Stores the full visited path and map seed so CaveMap
/// can restore itself identically after an additive scene reload.
/// Attach to the same persistent GameObject as GameManager.
/// </summary>
public class CaveMapState : MonoBehaviour
{
    public static CaveMapState Instance { get; private set; }

    // Full ordered list of (row, col) pairs the player has visited
    public List<Vector2Int> VisitedPath { get; private set; } = new List<Vector2Int>();

    // Seed used to generate the current map — ensures identical layout on reload
    public int MapSeed { get; private set; } = -1;

    // Tracks the global alternating row pattern across map resets
    public int GlobalRowIndex { get; set; } = 0;

    public bool HasVisitedNode => VisitedPath.Count > 0;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void GenerateNewSeed()
    {
        MapSeed = Random.Range(0, 999999);
    }

    public void AddVisitedNode(int row, int col)
    {
        VisitedPath.Add(new Vector2Int(col, row)); // x = col, y = row
    }

    public Vector2Int LastVisited => VisitedPath.Count > 0
        ? VisitedPath[VisitedPath.Count - 1]
        : new Vector2Int(-1, -1);

    /// <summary>
    /// Called when the player completes the last row.
    /// Generates a new seed and clears the path for the next map.
    /// </summary>
    public void ResetForNewMap(int depthPlusOne)
    {
        GlobalRowIndex += depthPlusOne;
        VisitedPath.Clear();
        GenerateNewSeed();
    }

    /// <summary>
    /// Full reset when starting a new game run.
    /// </summary>
    public void FullReset()
    {
        GlobalRowIndex = 0;
        VisitedPath.Clear();
        MapSeed = -1;
    }
}
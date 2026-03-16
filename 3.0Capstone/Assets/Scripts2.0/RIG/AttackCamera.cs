using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackCamera : MonoBehaviour
{
    private CameraCinematic cam;
    private WaveSpawner spawner;

    [Header("Camera Points")] //— index 0-2 = Left (Top/Mid/Bot), 3-5 = Right (Top/Mid/Bot), 6 = Centre
    [SerializeField] private Transform[] cameraPoints;

    [Header("Spawn Points")] //— index 0-2 = Left (Top/Mid/Bot), 3-5 = Right (Top/Mid/Bot)
    [SerializeField] private Transform[] spawnPoints;

    [Header("Timing")]
    [SerializeField] private float initialDelay = 3f;
    [SerializeField] private float resetHoldTime = 1f;
    [SerializeField] private float moveHoldTime = 1f;

    private const int SideSize = 3;
    private int lastCamIndex = -1;
    private int currentSide = -1;
    private bool transitioning = false;
    private bool waveWasComplete = false;

    void Start()
    {
        cam = FindFirstObjectByType<CameraCinematic>();
        if (cam == null) Debug.LogError("No CameraCinematic found.");

        spawner = FindFirstObjectByType<WaveSpawner>();
        if (spawner == null) Debug.LogError("No WaveSpawner found.");

        // Take full control — WaveSpawner.FiniteWaves must be true in the inspector
        spawner.CanSpawn = false;

        StartCoroutine(InitialSequence());
    }

    void Update()
    {
        if(spawner == null) { return; }
        if (!transitioning && spawner.IsWaveComplete && !waveWasComplete)
        {
            waveWasComplete = true;
            StartCoroutine(BetweenWaveSequence());
        }

        if (!spawner.IsWaveComplete)
            waveWasComplete = false;
    }

    private IEnumerator InitialSequence()
    {
        currentSide = Random.Range(0, 2);

        yield return new WaitForSeconds(initialDelay);

        MoveCamera();
        yield return new WaitForSeconds(moveHoldTime);

        spawner.StartNewWave();
    }

    private IEnumerator BetweenWaveSequence()
    {
        transitioning = true;

        // Step 1: return to centre
        //cam.MoveCameraToPosition(cameraPoints[6].position);
        //yield return new WaitForSeconds(resetHoldTime);

        // Step 2: flip side and move
        currentSide = currentSide == 0 ? 1 : 0;
        MoveCamera();
        yield return new WaitForSeconds(moveHoldTime);

        // Step 3: start next wave
        spawner.StartNewWave();

        transitioning = false;
    }

    private void MoveCamera()
    {
        int sideOffset = currentSide * SideSize;

        // Pick a random row on this side, excluding the last used index
        List<int> validIndices = new List<int>();
        for (int r = 0; r < SideSize; r++)
        {
            int index = sideOffset + r;
            if (index != lastCamIndex)
                validIndices.Add(index);
        }

        // Fallback in case all excluded (shouldn't happen with 3 rows)
        if (validIndices.Count == 0)
        {
            for (int r = 0; r < SideSize; r++)
                validIndices.Add(sideOffset + r);
        }

        int chosenIndex = validIndices[Random.Range(0, validIndices.Count)];
        lastCamIndex = chosenIndex;

        int row = chosenIndex % SideSize;

        cam.MoveCameraTo(cameraPoints[chosenIndex].position,1f);
        AssignSpawnPoints(sideOffset, row);
    }

    private void AssignSpawnPoints(int sideOffset, int excludeRow)
    {
        List<Transform> activePoints = new List<Transform>();

        for (int i = 0; i < spawnPoints.Length; i++)
        {
            int pointSideOffset = (i / SideSize) * SideSize;
            int pointRow = i % SideSize;

            if (pointSideOffset == sideOffset && pointRow != excludeRow)
                activePoints.Add(spawnPoints[i]);
        }

        spawner.SpawnPoints = activePoints;
    }

    public void ResetCamera()
    {
        StopAllCoroutines();
        transitioning = false;
        waveWasComplete = false;
        lastCamIndex = -1;
        currentSide = -1;

        spawner.CanSpawn = false;
        spawner.SpawnPoints = new List<Transform>(spawnPoints);
        cam.MoveCameraToPosition(cameraPoints[6].position);
    }
}
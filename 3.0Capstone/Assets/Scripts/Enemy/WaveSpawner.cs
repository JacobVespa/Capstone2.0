using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject rigTarget;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();

    [Header("Enemy Prefabs (choose one or randomize)")]
    [SerializeField] private List<GameObject> enemyPrefabs = new List<GameObject>();
    [SerializeField] private bool randomizeEnemyPrefab = true;

    [Header("Wave Settings")]
    [SerializeField] private int enemiesPerWave = 8;
    [SerializeField] private float spawnInterval = 1.0f;
    [SerializeField] private float timeBetweenWaves = 3.0f;

    [Header("Difficulty / Safety Caps")]
    [SerializeField] private int maxAliveEnemies = 10;

    [Header("Wave Rules")]
    [Tooltip("If true, next wave starts only after all spawned enemies are dead/inactive.")]
    [SerializeField] private bool requireWaveClearToAdvance = true;

    [Tooltip("If false, waves will run forever. If true, stop after totalWaves.")]
    [SerializeField] private bool finiteWaves = false;
    [SerializeField] private int totalWaves = 5;

    // Runtime
    private int currentWave = 0;
    private int spawnedThisWave = 0;

    private readonly List<GameObject> trackedEnemies = new List<GameObject>();

    private Coroutine waveRoutine;

    private void Start()
    {
        if (rigTarget == null)
            Debug.LogWarning("[WaveSpawner] rigTarget not assigned.");

        if (spawnPoints.Count == 0)
            Debug.LogWarning("[WaveSpawner] No spawn points assigned.");

        if (enemyPrefabs.Count == 0)
            Debug.LogWarning("[WaveSpawner] No enemy prefabs assigned.");

        waveRoutine = StartCoroutine(RunWaves());
    }


    private IEnumerator RunWaves()
    {
        while (true)
        {
            if (finiteWaves && currentWave >= totalWaves)
                yield break;

            currentWave++;
            spawnedThisWave = 0;

            // Spawn enemies for this wave
            while (spawnedThisWave < enemiesPerWave)
            {
                CleanupTrackedList();

                // Enforce alive cap
                if (GetAliveCount() >= maxAliveEnemies)
                {
                    yield return null;
                    continue;
                }

                SpawnOneEnemy();
                spawnedThisWave++;

                yield return new WaitForSeconds(spawnInterval);
            }

            // Wave spawned fully
            if (requireWaveClearToAdvance)
            {
                while (GetAliveCount() > 0)
                {
                    yield return null;
                }
            }
        }
    }

    private void SpawnOneEnemy()
    {
        if (enemyPrefabs.Count == 0 || spawnPoints.Count == 0) return;

        GameObject prefab = enemyPrefabs[0];
        if (randomizeEnemyPrefab && enemyPrefabs.Count > 1)
            prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

        Transform spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];

        GameObject enemy = Instantiate(prefab, spawnPoint.position, spawnPoint.rotation);
        trackedEnemies.Add(enemy);

        // Assign target if possible
        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        if (ai != null && rigTarget != null)
        {
            ai.SetTarget(rigTarget);
        }
    }
    private int GetAliveCount()
    {
        int alive = 0;
        for (int i = 0; i < trackedEnemies.Count; i++)
        {
            if (trackedEnemies[i] != null && trackedEnemies[i].activeSelf)
                alive++;
        }
        return alive;
    }

    private void CleanupTrackedList()
    {
        // Remove destroyed refs (not deactivated)
        for (int i = trackedEnemies.Count - 1; i >= 0; i--)
        {
            if (trackedEnemies[i] == null)
                trackedEnemies.RemoveAt(i);
        }
    }

    // Some stuff for debugging
    public int CurrentWave => currentWave;
    public int AliveEnemies => GetAliveCount();
}

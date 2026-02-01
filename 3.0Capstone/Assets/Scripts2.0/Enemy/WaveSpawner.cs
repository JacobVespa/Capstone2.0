using System.Collections.Generic;
using System.Collections;
using UnityEngine;



/*
 * WaveSpawner
 * --------------------------------------------------
 * Handles spawning enemies in configurable waves.
 *
 * HOW IT WORKS:
 * - Enemies spawn over time from defined spawn points
 * - Each wave spawns a fixed number of enemies
 * - A max-alive cap prevents overwhelming the player
 * - Waves can advance automatically or wait until cleared
 *
 * INSPECTOR SECTIONS:
 *
 * [Wave Settings]
 * - Enemies Per Wave:
 *   Total number of enemies spawned in a single wave.
 *
 * - Spawn Interval:
 *   Time (seconds) between individual enemy spawns.
 *
 * - Time Between Waves:
 *   Delay (seconds) before the next wave begins.
 *
 * [Difficulty / Safety Caps]
 * - Max Alive Enemies:
 *   Hard limit on how many enemies can exist at once.
 *   Spawning pauses until enemies are killed.
 *
 * [Wave Rules]
 * - Require Wave Clear To Advance:
 *   If enabled, the next wave will not start until all
 *   enemies from the current wave are dead/inactive.
 *
 * - Finite Waves:
 *   If enabled, the spawner will stop after Total Waves.
 *
 * - Total Waves:
 *   Number of waves when Finite Waves is enabled.
 *
 * NOTES:
 * - Enemies automatically target the RIG on spawn
 * - Uses activeSelf to track alive enemies
 * - Suitable for all rooms
 */


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
            if (WaveComplete())
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
        Vector2 spawnPos= spawnPoint.position;
        if (prefab.layer == 7)
        {
            EnemyBody body = prefab.GetComponent<EnemyBody>();
            body.Sprite.SetActive(false);

            spawnPos = rigTarget.transform.position;

            float yRange = Random.Range(-5,8); // change later if can since it was set up my eye balling shit
            float xRange = 0;

            if (yRange > 0) {  xRange = Random.Range(-8, 8); }
            else if(yRange <= 0) { xRange = Random.Range(-10, 10); }
            
            spawnPos.x += xRange;
            spawnPos.y += yRange;

            
        }
        
        

        GameObject enemy = Instantiate(prefab, spawnPos, spawnPoint.rotation);
        trackedEnemies.Add(enemy);

        GameObject attackPoint = rigTarget;

        if(spawnPoint.TryGetComponent<SpawnPoint>(out SpawnPoint sp))
        {
            if(sp.ReturnTargetPoint() != null) { attackPoint = sp.ReturnTargetPoint(); }
            
        }
        
        // Assign target if possible
        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        if (ai != null && rigTarget != null)
        {
            
            ai.SetTarget(rigTarget, attackPoint);
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
    public int CurrentWave { set { currentWave = value; } }
    public int AliveEnemies => GetAliveCount();

    public bool WaveComplete()
    {
        return finiteWaves && currentWave >= totalWaves;
    }
}

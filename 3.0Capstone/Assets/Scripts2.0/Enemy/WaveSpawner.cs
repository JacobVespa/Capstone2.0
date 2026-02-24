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
 * - Controlled externally by DefenceLevel for gameplay waves
 *
 * UPGRADE NOTES:
 * - Burst / formation spawning (spawn 3+ from one spawn point)
 * - Off-screen safety clamp (extra padding for large enemies)
 * - Defense anti-line: spawn spread + stagger inside bursts
 */

public class WaveSpawner : MonoBehaviour
{

    public enum FormationType
    {
        Single,
        Line,
        Arc,
        V
    }

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

    [Tooltip("Random offset applied around spawn point to reduce 'lines' (especially in Defense).")]
    [SerializeField] private float spawnPointSpreadRadius = 1.25f;

    [Header("Burst / Formation Spawning")]
    [SerializeField] private bool enableBurstSpawning = true;

    [Tooltip("Min enemies spawned per spawn event when burst is enabled.")]
    [SerializeField] private int burstMin = 3;

    [Tooltip("Max enemies spawned per spawn event when burst is enabled.")]
    [SerializeField] private int burstMax = 5;

    [SerializeField] private FormationType formationType = FormationType.Line;

    [Tooltip("Spacing between enemies in a formation.")]
    [SerializeField] private float formationSpacing = 1.5f;

    [Tooltip("Extra small random jitter to avoid perfect robot formations.")]
    [SerializeField] private float formationJitter = 0.35f;

    [Tooltip("If > 0, staggers enemies inside a burst to reduce 'single-file' behavior.")]
    [SerializeField] private float burstStaggerMin = 0.05f;

    [SerializeField] private float burstStaggerMax = 0.15f;

    [Header("Off-Screen Safety")]
    [SerializeField] private bool clampSpawnsOnScreen = true;

    [Tooltip("Extra viewport padding for normal enemies (0.02 = 2% screen).")]
    [SerializeField] private float viewportPaddingNormal = 0.03f;

    [Tooltip("Extra viewport padding for large/heavy enemies (0.06 = 6% screen).")]
    [SerializeField] private float viewportPaddingHeavy = 0.07f;

    [Tooltip("Assign large/heavy prefabs here so they never spawn near the edge/off-screen.")]
    [SerializeField] private List<GameObject> heavyEnemyPrefabs = new List<GameObject>();

    // Runtime
    private int currentWave = 0;
    private int spawnedThisWave = 0;
    private bool waveActive = false;
    private bool waveSpawningComplete = false;

    private readonly List<GameObject> trackedEnemies = new List<GameObject>();
    private Coroutine waveRoutine;

    private void Start()
    {
        if (rigTarget == null) Debug.LogWarning("[WaveSpawner] rigTarget not assigned.");
        if (spawnPoints.Count == 0) Debug.LogWarning("[WaveSpawner] No spawn points assigned.");
        if (enemyPrefabs.Count == 0) Debug.LogWarning("[WaveSpawner] No enemy prefabs assigned.");

        // Don't start automatically - wait for external trigger
        if (finiteWaves == false)
        {
            StartNewWave();
        }
    }

    private void Update()
    {
        if (!waveActive && !finiteWaves)
        {
            StartNewWave();
            return;
        }
    }

    public void StartNewWave()
    {
        if (waveActive)
        {
            Debug.LogWarning("[WaveSpawner] Trying to start a new wave while one is already active!");
            return;
        }

        currentWave++;
        spawnedThisWave = 0;
        waveActive = true;
        waveSpawningComplete = false;

        if (waveRoutine != null)
            StopCoroutine(waveRoutine);

        waveRoutine = StartCoroutine(RunSingleWave());
        Debug.Log($"[WaveSpawner] Started wave {currentWave}");
    }

    private IEnumerator RunSingleWave()
    {
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

            // Decide burst count
            int remaining = enemiesPerWave - spawnedThisWave;
            int burstCount = 1;

            if (enableBurstSpawning && remaining > 1)
            {
                burstCount = Random.Range(burstMin, burstMax + 1);
                burstCount = Mathf.Clamp(burstCount, 1, remaining);
            }

            // Also respect maxAlive cap when bursting
            int capacity = maxAliveEnemies - GetAliveCount();
            burstCount = Mathf.Clamp(burstCount, 1, capacity);

            // Spawn the burst
            if (enableBurstSpawning)
            {
                yield return StartCoroutine(SpawnBurst(burstCount));
                spawnedThisWave += burstCount;
            }
            else
            {
                SpawnSingleEnemy();
                spawnedThisWave += 1;
            }

            yield return new WaitForSeconds(spawnInterval);
        }

        waveSpawningComplete = true;
        Debug.Log($"[WaveSpawner] Wave {currentWave} spawning complete");

        // Wave spawned fully - wait for clear if required
        if (requireWaveClearToAdvance)
        {
            while (GetAliveCount() > 0)
            {
                yield return null;
            }
        }

        // Wave fully complete
        waveActive = false;
        Debug.Log($"[WaveSpawner] Wave {currentWave} fully complete");
    }

    private IEnumerator SpawnBurst(int count)
    {
        if (count <= 0) yield break;

        // Choose prefab once for the burst
        GameObject prefab = ChoosePrefab();
        if (prefab == null) yield break;

        // Choose an anchor position / spawnPoint for the burst
        Transform spawnPoint = null;
        Vector3 anchorPos = GetBaseSpawnPosition(prefab, out spawnPoint);
        Quaternion rot = (spawnPoint != null) ? spawnPoint.rotation : Quaternion.identity;

        // Formation offsets
        Vector3[] offsets = GetFormationOffsets(count, formationType, formationSpacing);

        for (int i = 0; i < count; i++)
        {
            Vector3 pos = anchorPos + offsets[i];

            // Add spread to reduce "lines"
            Vector2 spread2D = Random.insideUnitCircle * spawnPointSpreadRadius;
            pos += new Vector3(spread2D.x, spread2D.y, 0f);

            pos.z = anchorPos.z;
            pos += new Vector3(Random.Range(-formationJitter, formationJitter),Random.Range(-formationJitter, formationJitter),0f);

            // Clamp on-screen
            pos = ApplyOnScreenClampIfNeeded(pos, prefab);

            SpawnEnemyInstance(prefab, pos, rot, spawnPoint);

            if (count > 1 && burstStaggerMax > 0f)
                yield return new WaitForSeconds(Random.Range(burstStaggerMin, burstStaggerMax));
        }
    }

    private void SpawnSingleEnemy()
    {
        GameObject prefab = ChoosePrefab();
        if (prefab == null) return;

        Transform spawnPoint = null;
        Vector3 spawnPos = GetBaseSpawnPosition(prefab, out spawnPoint);
        Quaternion rot = (spawnPoint != null) ? spawnPoint.rotation : Quaternion.identity;

        spawnPos = ApplyOnScreenClampIfNeeded(spawnPos, prefab);

        SpawnEnemyInstance(prefab, spawnPos, rot, spawnPoint);
    }

    private GameObject ChoosePrefab()
    {
        if (enemyPrefabs.Count == 0) return null;

        GameObject prefab = enemyPrefabs[0];
        if (randomizeEnemyPrefab && enemyPrefabs.Count > 1)
            prefab = enemyPrefabs[Random.Range(0, enemyPrefabs.Count)];

        return prefab;
    }

    private Vector3 GetBaseSpawnPosition(GameObject prefab, out Transform spawnPoint)
    {
        spawnPoint = null;

        if (prefab.layer == 7 && rigTarget != null)
        {
            Vector3 pos = rigTarget.transform.position;

            float zRange = Random.Range(-5f, 8f);
            float xRange;
            if (zRange > 0) xRange = Random.Range(-8f, 8f);
            else xRange = Random.Range(-10f, 10f);

            pos.x += xRange;
            pos.y += zRange;

            return pos;
        }

        if (spawnPoints.Count > 0)
        {
            spawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            return spawnPoint.position;
        }

        // Fallback: rig position
        return (rigTarget != null) ? rigTarget.transform.position : Vector3.zero;
    }

    private void SpawnEnemyInstance(GameObject prefab, Vector3 spawnPos, Quaternion rotation, Transform spawnPoint)
    {
        GameObject enemy = Instantiate(prefab, spawnPos, rotation);
        trackedEnemies.Add(enemy);

        if (prefab.layer == 7)
        {
            EnemyBody body = enemy.GetComponent<EnemyBody>();
            if (body != null && body.Sprite != null)
                body.Sprite.SetActive(false);
        }

        GameObject attackPoint = rigTarget;

        if (spawnPoint != null && spawnPoint.TryGetComponent<SpawnPoint>(out SpawnPoint sp))
        {
            if (sp.ReturnTargetPoint() != null)
                attackPoint = sp.ReturnTargetPoint();
        }

        EnemyAI ai = enemy.GetComponent<EnemyAI>();
        if (ai != null && rigTarget != null)
        {
            ai.SetTarget(rigTarget, attackPoint);
        }
    }

    private Vector3[] GetFormationOffsets(int count, FormationType formation, float spacing)
    {
        Vector3[] offsets = new Vector3[count];

        if (formation == FormationType.Single || count == 1)
        {
            offsets[0] = Vector3.zero;
            return offsets;
        }

        switch (formation)
        {
            case FormationType.Line:
                for (int i = 0; i < count; i++)
                {
                    float x = (i - (count - 1) * 0.5f) * spacing;
                    offsets[i] = new Vector3(x, 0f, 0f);
                }
                break;

            case FormationType.Arc:
                float radius = spacing * Mathf.Max(1f, count / 2f);
                float start = -60f;
                float end = 60f;

                for (int i = 0; i < count; i++)
                {
                    float t = (count == 1) ? 0.5f : i / (float)(count - 1);
                    float ang = Mathf.Lerp(start, end, t) * Mathf.Deg2Rad;

                    offsets[i] = new Vector3(Mathf.Sin(ang) * radius, Mathf.Cos(ang) * radius, 0f);
                }
                break;

            case FormationType.V:
                offsets[0] = Vector3.zero;
                for (int i = 1; i < count; i++)
                {
                    int step = (i + 1) / 2;
                    int side = (i % 2 == 1) ? -1 : 1;
                    offsets[i] = new Vector3(side * step * spacing, step * spacing * 0.6f, 0f);
                }
                break;
        }

        return offsets;
    }

    private Vector3 ApplyOnScreenClampIfNeeded(Vector3 pos, GameObject prefab)
    {
        if (!clampSpawnsOnScreen) return pos;
        Camera cam = Camera.main;
        if (cam == null) return pos;

        float padding = IsHeavyPrefab(prefab) ? viewportPaddingHeavy : viewportPaddingNormal;

        Vector3 screenPoint = cam.WorldToScreenPoint(pos);

        if (screenPoint.z < 0f) return pos;

        Vector3 vp = cam.WorldToViewportPoint(pos);

        vp.x = Mathf.Clamp(vp.x, padding, 1f - padding);
        vp.y = Mathf.Clamp(vp.y, padding, 1f - padding);

        Vector3 clampedWorld = cam.ViewportToWorldPoint(new Vector3(vp.x, vp.y, screenPoint.z));

        clampedWorld.z = pos.z;
        clampedWorld.y = pos.y;

        return clampedWorld;
    }

    private bool IsHeavyPrefab(GameObject prefab)
    {
        if (heavyEnemyPrefabs != null && heavyEnemyPrefabs.Contains(prefab))
            return true;

        return false;
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
        for (int i = trackedEnemies.Count - 1; i >= 0; i--)
        {
            if (trackedEnemies[i] == null)
                trackedEnemies.RemoveAt(i);
        }
    }

    public void ResetSpawner()
    {
        if (waveRoutine != null)
            StopCoroutine(waveRoutine);

        currentWave = 0;
        spawnedThisWave = 0;
        waveActive = false;
        waveSpawningComplete = false;

        foreach (GameObject enemy in trackedEnemies)
        {
            if (enemy != null)
                Destroy(enemy);
        }
        trackedEnemies.Clear();
    }

    // Public accessors for DefenceLevel
    public bool IsWaveActive => waveActive;
    public bool IsWaveComplete => !waveActive && waveSpawningComplete;
    public int CurrentWave => currentWave;
    public int AliveEnemies => GetAliveCount();
}
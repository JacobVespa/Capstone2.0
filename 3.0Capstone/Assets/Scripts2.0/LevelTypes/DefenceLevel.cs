using System.Collections;
using UnityEngine;

public class DefenceLevel : Level
{
    public DefenceLevel(int sceneIndex, int waves)
        : base(LevelType.DEFENCE, sceneIndex, waves)
    {
    }

    private int wavesCompleted = 0;
    public int WavesCompleted { get { return wavesCompleted; } }

    private WaveSpawner[] spawners;
    DefenseGem gem;
    private bool hasStarted = false;
    private bool initialized = false;
    private float startTime = 10f;

    public override void StartLevel()
    {
        Debug.Log("Starting Defence Level");
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayBGM("DefenceLevel");

        base.StartLevel();
    }

    public override void MonoStart()
    {
        // Called automatically after the scene loads
        cinematic = GameObject.FindObjectsByType<CameraCinematic>(FindObjectsSortMode.None)[0];

        spawners = GameObject.FindObjectsByType<WaveSpawner>(FindObjectsSortMode.None);

        gem = GameObject.FindFirstObjectByType<DefenseGem>();

        // Spawners start disabled, waiting for the level to trigger them
        initialized = true;
        Debug.Log($"Defence Level MonoStart completed. Found {spawners.Length} spawners.");
    }

    public override void RestartLevel()
    {
        Debug.Log("Restarting Defence Level");

        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayBGM("DefenceLevel");

        hasStarted = false;
        wavesCompleted = 0;
        initialized = false;

        // Reset all spawners
        if (spawners != null)
        {
            foreach (WaveSpawner spawner in spawners)
            {
                spawner.ResetSpawner();
            }
        }

        base.RestartLevel();
    }

    public override void WindDownLevel(bool goNext)
    {
        Debug.Log("Defence Level Complete!");

        if(goNext) DifficultyManager.Instance.GemCollected();

        base.WindDownLevel(goNext);
    }

    public override void EndLevel()
    {
        Debug.Log("Ending Defence Level");
        base.EndLevel();
    }

    public void UpdateLevel()
    {
        if (!initialized || spawners == null)
            return;

        // Start cinematic + first wave
        if (!hasStarted &&
            cinematic != null &&
            GameManager.Instance.GameTime >= startTime)
        {
            cinematic.MoveCamera();
            WallMoving[] walls = GameObject.FindObjectsByType<WallMoving>(FindObjectsSortMode.None);
            foreach (WallMoving wall in walls)            
            {
                wall.Pause();
            }
            hasStarted = true;

            StartNextWave();
            Debug.Log("Starting Wave 1");
            return;
        }

        // Don't check wave completion until waves have started
        if (!hasStarted)
            return;

        // Check if all spawners completed their current wave
        bool allSpawnersComplete = true;
        foreach (WaveSpawner spawner in spawners)
        {
            if (!spawner.IsWaveComplete)
            {
                allSpawnersComplete = false;
                break;
            }
        }

        // If all spawners are done with their current wave, advance to next level wave
        if (allSpawnersComplete)
        {
            wavesCompleted++;
            Debug.Log($"Wave {wavesCompleted} Complete!");

            if (wavesCompleted < FinalWave)
            {
                StartNextWave();

                if (gem != null)
                {
                    gem.ShakeCrystal(1.5f);
                }

                cinematic.PanOver(3f,4f, new Vector3(0,14,0));

                Debug.Log($"Starting Wave {wavesCompleted + 1}");
            }
            else
            {
                Debug.Log("All waves complete!");

                if (gem != null)
                {
                    gem.BreakCrystal(1.5f);
                }

                cinematic.PanOver(3f,4f, new Vector3(0,14,0));
                // Victory will be triggered by GameflowManager checking WavesCompleted > FinalWave
            }
        }
    }

    private void StartNextWave()
    {
        foreach (WaveSpawner spawner in spawners)
        {
            spawner.StartNewWave();
        }
    }
}
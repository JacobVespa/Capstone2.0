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
    private bool hasStarted = false;
    private bool initialized = false;
    private float startTime = 10f;

    public override void StartLevel()
    {
        Debug.Log("Starting Defence Level");
        if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM("CaveFight");
        base.StartLevel();
    }

    public override void MonoStart()
    {
        // This is called automatically after the scene loads
        cinematic = GameObject.FindObjectsByType<CameraCinematic>(FindObjectsSortMode.None)[0];
        spawners = GameObject.FindObjectsByType<WaveSpawner>(FindObjectsSortMode.None);

        foreach (WaveSpawner spawner in spawners)
        {
            spawner.CurrentWave = 0;
            spawner.enabled = false;
        }

        initialized = true;
        Debug.Log("Defence Level MonoStart completed");
    }

    public override void RestartLevel()
    {
        Debug.Log("Restarting Defence Level");
        if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM("CaveFight");
        
        hasStarted = false;
        wavesCompleted = 0;
        initialized = false;
        
        base.RestartLevel();
    }

    public override void WindDownLevel(bool goNext)
    {
        Debug.Log("Defence Level Complete!");
        base.WindDownLevel(goNext);
    }

    public override void EndLevel()
    {
        Debug.Log("Ending Defence Level");
        base.EndLevel();
    }

    public void UpdateLevel()
    {
        if (!initialized || spawners == null) return;

        if (!hasStarted && this.cinematic != null && GameManager.Instance.GameTime >= startTime)
        {
            this.cinematic.MoveCamera();
            hasStarted = true;
            Debug.Log("Starting Waves");
        }

        bool canStart = false;

        foreach (WaveSpawner spawner in spawners)
        {
            if (!spawner.WaveComplete())
            {
                return;
            }

            canStart = true;
        }

        if (canStart)
        {
            NextWave();
        }
    }

    private void NextWave()
    {
        wavesCompleted++;
        if (wavesCompleted > this.FinalWave)
        {
            Debug.Log("All waves complete!");
            return;
        }
        StartWave();
    }

    private void StartWave()
    {
        foreach (WaveSpawner spawner in spawners)
        {
            spawner.enabled = true;
            spawner.CurrentWave = 0;
        }
    }
}
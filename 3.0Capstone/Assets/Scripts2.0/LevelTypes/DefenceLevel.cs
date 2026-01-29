using UnityEngine;

public class DefenceLevel : Level
{
    public DefenceLevel(int sceneIndex, int waves) 
        : base(LevelType.DEFENCE, sceneIndex, waves)
    {
    }

    private int wavesCompleted = 0;
    public int WavesCompleted {get { return wavesCompleted; } }

    public override void StartLevel()
    {
        Debug.Log("Starting Defence Level");
        if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM("CaveFight");
        base.StartLevel();
    }

    public override void RestartLevel()
    {
        Debug.Log("Restarting Defence Level");
        if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM("CaveFight");
        base.RestartLevel();
    }

    public override void WindDownLevel(bool goNext)
    {
        Debug.Log("Defence Level Complete!");
        base.WindDownLevel(goNext);
    }

    public override void EndLevel()
    {
        Debug.Log("Ending Scroller Level");
        base.EndLevel();
    }

    private WaveSpawner[] spawnsers;
    private CameraCinematic cinematic;
    private bool hasStarted = false;

    void Start()
    {
        cinematic = GameObject.FindObjectsByType<CameraCinematic>(FindObjectsSortMode.None)[0];
        spawnsers = GameObject.FindObjectsByType<WaveSpawner>(FindObjectsSortMode.None);

        foreach (WaveSpawner spawner in spawnsers)
        {
            spawner.CurrentWave = 0;
            spawner.enabled = false;
        }
    }

    void Update()
    {
        if (spawnsers == null || spawnsers.Length == 0) return;

        if (!hasStarted && cinematic != null)
        {
            cinematic.MoveCamera();
            hasStarted = true;
        }

        bool canStart = false;

        foreach (WaveSpawner spawner in spawnsers)
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
        foreach (WaveSpawner spawner in spawnsers)
        {
            spawner.enabled = true;
            spawner.CurrentWave = 0;
        }
    }
}

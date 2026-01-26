using UnityEngine;

public class Level
{
    protected LevelManager levelManager => LevelManager.Instance;

    public enum LevelType // Temp boss types for now
    {
        SCROLLER = 1,
        DEFENCE = 2,
        BOSS = 3
    }

    protected LevelType levelType;
    protected int sceneIndex;
    protected float duration; // in seconds
    protected int finalWave;
    protected int waves;

    public float Duration => duration;
    public int FinalWave => finalWave;

    public Level(LevelType type, int index, float durationSeconds)
    {
        this.levelType = type;
        this.sceneIndex = index;
        this.duration = durationSeconds;
    }

    public Level(LevelType type, int index, int TotalWaves)
    {
        this.levelType = type;
        this.sceneIndex = index;
        this.finalWave = TotalWaves;
        //this.waves = 0;
    }

    public virtual void StartLevel() // Menu -> Level
    {
        if (levelManager == null) return;

        levelManager.LoadScene(sceneIndex);
    }

    public virtual void WindDownLevel(bool goNext) // Level -> Level Transition
    {
        if (levelManager == null) return;

        levelManager.StartWindDownLevel(goNext);
    }

    public virtual void EndLevel() // Level -> Menu  | OR | Level -> End Game
    {
        if (levelManager == null) return;

        levelManager.LoadScene(0); //Loops to menu for now, add logic to determine othe levels later
        SoundManager.Instance.PlayBGM("CaveFight");
    }

    public virtual void RestartLevel() // Level -> Level
    {
        if (levelManager == null) return;

        levelManager.LoadScene(sceneIndex);
    }

    public LevelType GetLevelType()
    {
        return levelType;
    }
}

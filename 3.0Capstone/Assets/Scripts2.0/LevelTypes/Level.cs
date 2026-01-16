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

    public float Duration => duration;

    public Level(LevelType type, int index, float durationSeconds)
    {
        this.levelType = type;
        this.sceneIndex = index;
        this.duration = durationSeconds;
    }

    public virtual void StartLevel() // Menu -> Level
    {
        levelManager.LoadScene(sceneIndex);
    }

    public virtual void WindDownLevel() // Level -> Level Transition
    {
        levelManager.StartWindDownLevel(this);
    }

    public virtual void EndLevel() // Level -> Menu  | OR | Level -> End Game
    {
        levelManager.LoadScene(0); //Loops to menu for now, add logic to determine othe levels later
    }

    public virtual void RestartLevel() // Level -> Level
    {
        levelManager.LoadScene(sceneIndex);
    }

    public LevelType GetLevelType()
    {
        return levelType;
    }
}

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

    public Level(LevelType type, int index)
    {
        this.levelType = type;
        this.sceneIndex = index;
    }

    public virtual void StartLevel() // Menu -> Level
    {
        levelManager.LoadScene(sceneIndex);
    }

    public virtual void EndLevel() // Level -> Menu  | OR | Level -> End Game
    {
        levelManager.LoadScene(0);
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

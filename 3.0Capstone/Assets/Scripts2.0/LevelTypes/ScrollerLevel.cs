using UnityEngine;

public class ScrollerLevel : Level
{
    public ScrollerLevel(int sceneIndex) 
        : base(LevelType.SCROLLER, sceneIndex)
    {
    }

    public override void StartLevel()
    {
        Debug.Log("Starting Scroller Level");
        base.StartLevel();
    }

    public override void EndLevel()
    {
        Debug.Log("Scroller Level Complete!");
        base.EndLevel();
    }
}


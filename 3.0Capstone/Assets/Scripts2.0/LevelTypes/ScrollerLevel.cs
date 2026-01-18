using UnityEngine;

public class ScrollerLevel : Level
{
    public ScrollerLevel(int sceneIndex, float duration) 
        : base(LevelType.SCROLLER, sceneIndex, duration)
    {
    }

    public override void StartLevel()
    {
        Debug.Log("Starting Scroller Level");
        SoundManager.Instance.PlayBGM("CaveFight");
        base.StartLevel();
    }

    public override void RestartLevel()
    {
        Debug.Log("Restarting Scroller Level");
        SoundManager.Instance.PlayBGM("CaveFight");
        base.RestartLevel();
    }

    public override void WindDownLevel()
    {
        Debug.Log("Scroller Level Complete!");
        base.WindDownLevel();
    }

    public override void EndLevel()
    {
        Debug.Log("Ending Scroller Level");
        base.EndLevel();
    }
}
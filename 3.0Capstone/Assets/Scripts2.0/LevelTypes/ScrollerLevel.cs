using UnityEngine;

public class ScrollerLevel : Level
{
    public ScrollerLevel(int sceneIndex, float duration) 
        : base(LevelType.SCROLLER, sceneIndex, duration)
    {
    }

    public override void StartLevel()
    {
        //Debug.Log("Starting Scroller Level");
        if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM("BattleTheme");
        base.StartLevel();
    }

    public override void RestartLevel()
    {
        //Debug.Log("Restarting Scroller Level");
        if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM("BattleTheme");
        base.RestartLevel();
    }

    public override void WindDownLevel(bool goNext)
    {
        //Debug.Log("Scroller Level Complete!");
        base.WindDownLevel(goNext);
    }

    public override void EndLevel()
    {
        //Debug.Log("Ending Scroller Level");
        base.EndLevel();
    }
}
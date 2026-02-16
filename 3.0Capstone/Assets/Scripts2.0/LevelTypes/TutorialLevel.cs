using UnityEngine;

public class TutorialLevel : Level
{
    public TutorialLevel(int sceneIndex) 
        : base(LevelType.TUTORIAL, sceneIndex, 0f)
    {
    }

    public override void StartLevel()
    {
        Debug.Log("Starting Scroller Level");
        if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM("BattleTheme");
        base.StartLevel();
    }

    public override void RestartLevel()
    {
        Debug.Log("Restarting Scroller Level");
        if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM("BattleTheme");
        base.RestartLevel();
    }

    public override void WindDownLevel(bool goNext)
    {
        Debug.Log("Scroller Level Complete!");
        base.WindDownLevel(goNext);
    }

    public override void EndLevel()
    {
        Debug.Log("Ending Scroller Level");
        base.EndLevel();
    }
}
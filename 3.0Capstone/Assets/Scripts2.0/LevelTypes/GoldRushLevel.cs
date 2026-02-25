using UnityEngine;

public class GoldRushLevel : Level
{
    public GoldRushLevel(int sceneIndex, float duration)
        : base(LevelType.GOLDRUSH, sceneIndex, duration)
    {
    }

    public override void StartLevel()
    {
        Debug.Log("Starting Gold Rush Level");
        if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM("BattleTheme");
        base.StartLevel();
    }

    public override void RestartLevel()
    {
        Debug.Log("Restarting Gold Rush Level");
        if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM("BattleTheme");
        base.RestartLevel();
    }

    public override void WindDownLevel(bool goNext)
    {
        Debug.Log("Gold Rush Level Complete!");
        base.WindDownLevel(goNext);
    }

    public override void EndLevel()
    {
        Debug.Log("Ending Gold Rush Level");
        base.EndLevel();
    }
}
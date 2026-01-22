using UnityEngine;

public class DefenceLevel : Level
{
    public DefenceLevel(int sceneIndex, int waves) 
        : base(LevelType.SCROLLER, sceneIndex, waves)
    {
    }

    public override void StartLevel()
    {
        Debug.Log("Starting Defence Level");
        SoundManager.Instance.PlayBGM("CaveFight");
        base.StartLevel();
    }

    public override void RestartLevel()
    {
        Debug.Log("Restarting Defence Level");
        SoundManager.Instance.PlayBGM("CaveFight");
        base.RestartLevel();
    }

    public override void WindDownLevel()
    {
        Debug.Log("Defence Level Complete!");
        base.WindDownLevel();
    }

    public override void EndLevel()
    {
        Debug.Log("Ending Scroller Level");
        base.EndLevel();
    }
}

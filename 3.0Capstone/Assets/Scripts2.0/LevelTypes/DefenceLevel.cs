using UnityEngine;

public class DefenceLevel : Level
{
    public DefenceLevel(int sceneIndex, int waves) 
        : base(LevelType.DEFENCE, sceneIndex, waves)
    {
    }

    private int wavesCompleted = 0;
    public int WavesCompleted {get { return wavesCompleted; } set { wavesCompleted = value; } }

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
}

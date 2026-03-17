using UnityEngine;

public class QVariant1Level : Level
{
    public QVariant1Level(int sceneIndex, int wasPressed) // 0 for no, 1 for yes
        : base(LevelType.QVARIANT1, sceneIndex, wasPressed)
    {
    }

    public override void StartLevel()
    {
        Debug.Log("Starting Question Mark Variant 1 Level");
        if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM("Navigation");
        base.StartLevel();
    }

    public override void RestartLevel()
    {
        Debug.Log("Restarting Question Mark Variant 1 Level");
        if (SoundManager.Instance != null) SoundManager.Instance.PlayBGM("Navigation");
        base.RestartLevel();
    }

    public override void WindDownLevel(bool goNext)
    {
        Debug.Log("Question Mark Variant 1 Level Complete");
        base.WindDownLevel(goNext);
    }

    public override void EndLevel()
    {
        Debug.Log("Ending Question Mark Variant 1 Level");
        base.EndLevel();
    }

}

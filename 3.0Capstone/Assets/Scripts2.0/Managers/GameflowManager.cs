using UnityEngine;

public class GameflowManager : MonoBehaviour
{
    public Level CurrentLevel { get; private set; }

    private bool levelRunning = false;

    private void Update()
    {
        if (!levelRunning || CurrentLevel == null || GameManager.Instance.GameOverStatus)
            return;

        if (CurrentLevel is ScrollerLevel scrollerLevel)
        {
            if (GameManager.Instance.GameTime >= scrollerLevel.Duration)
            {
                levelRunning = false;
                GameManager.Instance.Victory();
            }
        }
        else if (CurrentLevel is DefenceLevel defenceLevel)
        {
            defenceLevel.UpdateLevel();

            if (defenceLevel.WavesCompleted > defenceLevel.FinalWave)
            {
                levelRunning = false;
                GameManager.Instance.Victory();
            }
        }
    }

    // -------- Level Entry --------

    public void StartScrollerLevel()
    {
        int currentSceneIndex = 1;
        float currentDuration = 60f;
        
        CurrentLevel = new ScrollerLevel(currentSceneIndex, currentDuration);
        BeginLevel();
    }

    public void StartDefenseLevel()
    {
        int currentSceneIndex = 4;
        int finalWave = 3;
        
        CurrentLevel = new DefenceLevel(currentSceneIndex, finalWave);
        BeginLevel();
    }

    private void BeginLevel()
    {
        ResetValues();
        levelRunning = true;
        CurrentLevel?.StartLevel();
    }

    public void EndLevel()
    {
        if (CurrentLevel == null) return;

        GameManager.Instance.ResetGameTime();
        levelRunning = false;
        CurrentLevel?.EndLevel();
    }

    public void RestartLevel()
    {
        if (CurrentLevel == null) return;

        ResetValues();
        levelRunning = true;
        CurrentLevel?.RestartLevel();
    }

    public void WindDownLevel(bool goNext)
    {
        if (CurrentLevel == null) return;

        GameManager.Instance.PauseGameTime();
        GameManager.Instance.ResetGameTime();
        levelRunning = false;
        CurrentLevel?.WindDownLevel(goNext);
    }

    // -------- Utility --------

    public void ResetValues()
    {
        GameManager.Instance.GameOverStatus = false;
        GameManager.Instance.GameOverTriggered = false;
        GameManager.Instance.ResetGameTime();
        GameManager.Instance.StartGameTime();
    }
}
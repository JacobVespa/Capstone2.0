using UnityEngine;

public class GameflowManager : MonoBehaviour
{
    public Level CurrentLevel { get; private set; }

    private bool levelRunning = false;

    private void Update()
    {
        if (!levelRunning || CurrentLevel == null)
            return;

        if (GameManager.Instance.GameTime >= CurrentLevel.Duration && GameManager.Instance.GameOverStatus == false)
        {
            levelRunning = false;
            //WindDownLevel();
            GameManager.Instance.Victory();
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

    public void WindDownLevel()
    {
        if (CurrentLevel == null) return;

        GameManager.Instance.PauseGameTime();
        GameManager.Instance.ResetGameTime();
        levelRunning = false;
        CurrentLevel?.WindDownLevel();
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
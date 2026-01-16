using UnityEngine;

public class GameflowManager : MonoBehaviour
{
    public Level CurrentLevel { get; private set; }

    private bool levelRunning = false;

    private void Update()
    {
        if (!levelRunning || CurrentLevel == null)
            return;

        if (GameManager.Instance.GameTime >= CurrentLevel.Duration)
        {
            levelRunning = false;
            WindDownLevel();
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
        GameManager.Instance.ResetGameTime();
        GameManager.Instance.StartGameTime();
        levelRunning = true;
        CurrentLevel?.StartLevel();
    }

    public void WindDownLevel()
    {
        if (CurrentLevel == null) return;

        GameManager.Instance.PauseGameTime();
        GameManager.Instance.ResetGameTime();
        levelRunning = false;
        CurrentLevel?.WindDownLevel();
    }
}
using UnityEngine;
using System.Collections;

public class GameflowManager : MonoBehaviour
{
    public Level CurrentLevel { get; private set; }

    private bool levelRunning = false;
    public bool LevelRunning => levelRunning;

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
        else if (CurrentLevel is GoldRushLevel goldRushLevel)
        {
            if (GameManager.Instance.GameTime >= goldRushLevel.Duration)
            {
                levelRunning = false;
                GameManager.Instance.Victory();
            }
        }
    }

    // -------- Level Entry --------

    public void StartTutorialLevel()
    {
        int currentSceneIndex = 5;
        
        CurrentLevel = new TutorialLevel(currentSceneIndex);
        BeginLevel();
    }

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

    public void StartGoldRushLevel()
    {
        int currentSceneIndex = 6;
        float currentDuration = 30f;

        CurrentLevel = new GoldRushLevel(currentSceneIndex, currentDuration);
        BeginLevel();
    }

    private void BeginLevel()
    {
        ResetValues();
        levelRunning = true;
        CursorHidden(true);
        CurrentLevel?.StartLevel();
    }

    public void EndLevel()
    {
        if (CurrentLevel == null) return;

        GameManager.Instance.ResetGameTime();
        levelRunning = false;
        CursorHidden(false);
        CurrentLevel?.EndLevel();
    }

    public void RestartLevel()
    {
        if (CurrentLevel == null) return;

        ResetValues();
        levelRunning = true;
        CursorHidden(true);
        CurrentLevel?.RestartLevel();
    }

    public void WindDownLevel(bool goNext)
    {
        if (CurrentLevel == null) return;
        StartCoroutine(WindDownSequence(goNext));
    }

    private IEnumerator WindDownSequence(bool goNext)
    {
        // Pause time immediately
        GameManager.Instance.ResetGameTime();
        levelRunning = false;

        // Play transition and wait for it to finish
        CameraCinematic cam = GameObject.FindFirstObjectByType<CameraCinematic>();
        if (cam != null && goNext)
        {
            cam.PanOver(20f, 0.1f, new Vector3(0, -60, 0));
            yield return new WaitForSecondsRealtime(4f); // match the PanOver duration
        }

        CursorHidden(false);
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

    public void CursorHidden(bool isHidden)
    {
        Cursor.visible = !isHidden;
        Cursor.lockState = isHidden ? CursorLockMode.Confined : CursorLockMode.None;
    }
}
using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class GameflowManager : MonoBehaviour
{
    public Level CurrentLevel { get; private set; }

    private bool levelRunning = false;
    public bool LevelRunning
    {
        get {  return levelRunning; }
        set { levelRunning = value; }
    }

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
                GameManager.Instance.Gems++;

                FlashGems();

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

    // Randomize variants here
    public void Randomize()
    {
        int randQVariant = Random.Range(0, 2);
        if(randQVariant == 0)
        {
            StartQVariant1Level();
        }
        else
        {
            StartQVariant2Level();
        }
    }

    public void StartQVariant1Level()
    {
        int currentSceneIndex = 8;
        int wasPressed = 0;

        CurrentLevel = new QVariant1Level(currentSceneIndex, wasPressed);
        BeginLevel();
    }

    public void StartQVariant2Level()
    {
        int currentSceneIndex = 10;
        int wasPressed = 0;

        CurrentLevel = new QVariant2Level(currentSceneIndex, wasPressed);
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
        
        GameManager.Instance.ResetGameTime();
        levelRunning = false;
        CursorHidden(false);
        CurrentLevel?.WindDownLevel(goNext);
    }

    // -------- Utility --------

    private void FlashGems()
    {
        StartCoroutine(GemEffect(1f));
    }

    private IEnumerator GemEffect(float delay)
    {
        yield return new WaitForSeconds(delay);

        DefenseGemUI defenseGemUI = GameObject.FindFirstObjectByType<DefenseGemUI>();
        if (defenseGemUI != null)
            defenseGemUI.UpdateGemCount();
        else
            Debug.Log("gemUI not available");
    }

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
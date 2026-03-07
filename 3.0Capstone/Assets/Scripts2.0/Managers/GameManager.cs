using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private int resultScreenIndex = 2;
    public int ResultScreenIndex => resultScreenIndex;
    [SerializeField] private int navigationScreenIndex = 3;
    public int NavigationScreenIndex => navigationScreenIndex;


    private bool timeActive;
    private bool isPaused;

    private bool gameOverTriggered = false;
    public bool GameOverTriggered { set { gameOverTriggered = value; } }

    private bool gameOver = false;
    public bool GameOverStatus { get { return gameOver; } set { gameOver = value; } }

    private int shards;
    public int Shards => shards;

    private int kills;
    public int Kills => kills;

    [SerializeField] private float gameTime;
    public float GameTime => gameTime;

    public PlayerMovement[] playerMovement;
    [SerializeField] private Canvas pauseCanvas;
    private Button pauseButton;

    [SerializeField] private Transform vfxStorage;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        playerMovement = FindObjectsByType<PlayerMovement>(sortMode: FindObjectsSortMode.None);
        pauseButton = pauseCanvas.GetComponentInChildren<Button>();
    }

    private void Start()
    {
        shards = 0;
        timeActive = false;
        if (SoundManager.Instance != null)
            SoundManager.Instance.PlayBGM("CaveFight");
    }

    private void Update()
    {
        if (timeActive) gameTime += Time.deltaTime;

        if (gameOver && !gameOverTriggered)
        {
            gameOverTriggered = true;
            GameOver();
        }

        TogglePauseMenu();
    }

    public void AddShards(int amount)
    {
        shards += amount;
    }

    public void AddKills(int amount)
    {
        kills += amount;
    }

    public void ResetStats()
    {
        shards = 0;
        kills = 0;
    }

    public void StartGameTime() => timeActive = true;
    public void PauseGameTime() => timeActive = false;
    public void StopGameTime() => Time.timeScale = 0f;
    public void ResumeGameTime() => Time.timeScale = 1f;

    public void ResetGameTime()
    {
        gameTime = 0f;
    }

    public void TogglePauseMenu()
    {

        PlayerControls controls = FindFirstObjectByType<PlayerControls>();
        GameflowManager flow = FindFirstObjectByType<GameflowManager>();

        if (!flow.LevelRunning || controls == null || flow == null) return;

        if (controls.controlEvent.HasEscaped && !isPaused)
        {
            pauseCanvas.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(null);
            EventSystem.current.SetSelectedGameObject(pauseButton.gameObject);

            StopGameTime();
            
        }
        else if (controls.controlEvent.HasEscaped && isPaused)
        {
            pauseCanvas.gameObject.SetActive(false);

            ResumeGameTime();
        }
    }

    public void PauseButton()
    {
        pauseCanvas.gameObject.SetActive(false);
        ResumeGameTime();
    }

    public void GameOver()
    {
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager != null) gameflowManager.WindDownLevel(false);

        PauseGameTime();
    }

    public void Victory()
    {
        GameflowManager gameflowManager = FindFirstObjectByType<GameflowManager>();
        if (gameflowManager != null) gameflowManager.WindDownLevel(true);

        PauseGameTime();
    }

    public Transform GetStorage()
    {
        return vfxStorage;
    }

    public void ToggleStorageMovement(bool isMoving)
    {
        foreach (Transform emitter in vfxStorage)
        {
            if (isMoving)
                emitter.GetComponentInChildren<ParticleSystem>().Play();
            else
                emitter.GetComponentInChildren<ParticleSystem>().Pause();
        }
    }

    public void EmptyStorage()
    {
        foreach (Transform t in vfxStorage)
        {
            if (t != null)
                Destroy(t.gameObject);
        }
    }

}
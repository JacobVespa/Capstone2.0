using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    private int shards;
    private float gameTime;
    private bool timeActive;

    public int Shards => shards;
    public float GameTime => gameTime;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Start()
    {
        shards = 0;
        timeActive = false;
    }

    private void Update()
    {
        if (timeActive)
            gameTime += Time.deltaTime;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Start timer when a gameplay scene loads
        if (scene.name != "MainMenu")
        {
            ResetGameTime();
        }
    }

    public void AddShards(int amount)
    {
        shards += amount;
    }

    public void StartGameTime() => timeActive = true;
    public void PauseGameTime() => timeActive = false;

    public void ResetGameTime()
    {
        gameTime = 0f;
        timeActive = true;
    }
}
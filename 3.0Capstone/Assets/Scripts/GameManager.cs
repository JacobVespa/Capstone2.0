using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class GameManager : MonoBehaviour
{
    
    public static GameManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // Destroy duplicate instances
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Persist across scene loads
        }
    }

    private int shards;
    public int Shards { get { return shards; } }

    private float gameTime;
    public float GameTime { get { return gameTime; } }

    private bool timeActive;

    void Start()
    {
        shards = 0;
    }

    private void Update()
    {
        if (timeActive) gameTime = Time.time;
    }

    public void AddShards(int amount)
    {
        shards += amount;
    }

    public void StartGameTime() { timeActive = true; }

    public void PauseGameTime() { timeActive = false; }

    public void ResetGameTime()
    {
        timeActive = false;
        gameTime = 0;
        timeActive = true;
    }

}

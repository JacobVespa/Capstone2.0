using UnityEngine;
using UnityEngine.EventSystems;

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

    public EventSystem UIEvent;

    private int gems;
    public int Gems { get { return gems; } }
    private int shards;
    public int Shards { get { return shards; } }

    void Start()
    {
        gems = 0;
        shards = 0;

        UIEvent = FindAnyObjectByType<EventSystem>();
    }

    public void AddShards(int amount)
    {
        shards += amount;
    }

}

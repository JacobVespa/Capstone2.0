using NUnit.Framework;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputControlManager : MonoBehaviour
{
    public static InputControlManager Instance { get; private set; }
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

        Cursor.visible = false; //TODO move this to a menu/settings script later
        Cursor.lockState = CursorLockMode.Confined;
        playerInputManager = GetComponent<PlayerInputManager>();
    }

    private PlayerInputManager playerInputManager;

    [SerializeField] private int numberOfPlayers = 2;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    public List<Transform> SpawnPoints { get { return spawnPoints; } }

    private void Start()
    {
        if (numberOfPlayers < 1) numberOfPlayers = 1;

        for (int i = 0; i < numberOfPlayers; i++)
        {
            SpawnPlayer();
        }
    }

    public void SpawnPlayer(InputDevice device = null)
    {
        playerInputManager.JoinPlayer(playerInputManager.playerCount, -1,null, device);
    }

    public void HasSpawned()
    {
        if (spawnPoints.Count > 0) spawnPoints.RemoveAt(0);
    }


}

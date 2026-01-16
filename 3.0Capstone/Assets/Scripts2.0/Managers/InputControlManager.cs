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
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        playerInputManager = GetComponent<PlayerInputManager>();
    }

    private PlayerInputManager playerInputManager;

    [SerializeField] private int numberOfPlayers = 2;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    public List<Transform> SpawnPoints { get { return spawnPoints; } }

    private Mole[] player = new Mole[2];
    public Mole[] Player { get { return player; } }

    // Track how many players have spawned
    private int spawnedPlayerCount = 0;
    public int SpawnedPlayerCount { get { return spawnedPlayerCount; } }

    [Header("Player One Assets")]
    [SerializeField] private Sprite playerOneSprite;
    [SerializeField] private RuntimeAnimatorController playerOneAnimatorController;

    [Header("Player Two Assets")]
    [SerializeField] private Sprite playerTwoSprite;
    [SerializeField] private RuntimeAnimatorController playerTwoAnimatorController;

    private void Start()
    {
        SetPlayerAssets();

        if (numberOfPlayers < 1) numberOfPlayers = 1;
        if (numberOfPlayers > 2) numberOfPlayers = 2;

        // Get all connected input devices
        var devices = InputSystem.devices;
        
        int playersSpawned = 0;
        
        // Try to spawn a player for each connected device (up to numberOfPlayers)
        for (int i = 0; i < devices.Count && playersSpawned < numberOfPlayers; i++)
        {
            // Only use Gamepad or Keyboard devices
            if (devices[i] is Gamepad || devices[i] is Keyboard)
            {
                playerInputManager.JoinPlayer(playersSpawned, -1, null, devices[i]);
                playersSpawned++;
            }
        }

        // If we didn't spawn enough players, log a warning
        if (playersSpawned < numberOfPlayers)
        {
            Debug.LogWarning($"Only {playersSpawned} input devices found. Expected {numberOfPlayers} players.");
        }
    }

    // Called by PlayerMovement after it finishes spawning
    public void HasSpawned()
    {
        spawnedPlayerCount++;
    }

    public Transform GetCurrentSpawnPoint()
    {
        if (spawnedPlayerCount < spawnPoints.Count)
        {
            return spawnPoints[spawnedPlayerCount];
        }
        return spawnPoints[0]; // Fallback
    }

    // Get the player index for the currently spawning player
    public int GetCurrentPlayerIndex()
    {
        return spawnedPlayerCount;
    }

    private void SetPlayerAssets()
    {
        if (numberOfPlayers <= 0 || numberOfPlayers > 2) return;

        player[0] = new Mole(playerOneSprite, playerOneAnimatorController);
        player[1] = new Mole(playerTwoSprite, playerTwoAnimatorController);
    }

    public class Mole
    {
        private Sprite playerSprite;
        private RuntimeAnimatorController animatorController;

        public Mole(Sprite sprite, RuntimeAnimatorController controller)
        {
            this.playerSprite = sprite;
            this.animatorController = controller;
        }

        public Sprite PlayerSprite { get { return playerSprite; } }
        public RuntimeAnimatorController AnimatorController { get { return animatorController; } }
    }
}
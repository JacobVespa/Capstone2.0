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
        playerInputManager = GetComponent<PlayerInputManager>();
    }

    private PlayerInputManager playerInputManager;

    [SerializeField] private int numberOfPlayers = 2;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    public List<Transform> SpawnPoints { get { return spawnPoints; } }

    private Mole[] player = new Mole[2];
    public Mole[] Player { get { return player; } }

    private int spawnedPlayerCount = 0;
    public int SpawnedPlayerCount { get { return spawnedPlayerCount; } }

    [Header("Player One Assets")]
    [SerializeField] private GameObject playerOne;

    [Header("Player Two Assets")]
    [SerializeField] private GameObject playerTwo;

    private void Start()
    {
        SetPlayerAssets();
        TogglePlayerLimit(GameManager.Instance.increaseMaxPlayer);

        if (numberOfPlayers < 1) numberOfPlayers = 1;
        if (numberOfPlayers > 2) numberOfPlayers = 2;

        var devices = InputSystem.devices;
        int playersSpawned = 0;

        // Check if Steam is injecting a virtual XInput controller
        bool hasSteamVirtualController = false;
        foreach (var device in devices)
        {
            if (device is Gamepad && device.description.interfaceName == "XInput")
            {
                hasSteamVirtualController = true;
                break;
            }
        }

        // Deduplicate devices by product + serial to catch any remaining duplicates
        var seen = new HashSet<string>();

        for (int i = 0; i < devices.Count && playersSpawned < numberOfPlayers; i++)
        {
            if (devices[i] is Gamepad gamepad)
            {
                // Skip raw duplicate devices when Steam virtual controller is active.
                // Raw HID duplicates typically have no manufacturer string and are non-XInput.
                if (hasSteamVirtualController &&
                    gamepad.description.interfaceName != "XInput" &&
                    string.IsNullOrEmpty(gamepad.description.manufacturer))
                {
                    Debug.Log($"Skipping likely Steam raw duplicate: {gamepad.description.product}");
                    continue;
                }

                // Deduplicate by product + serial as a second safety net
                string deviceKey = gamepad.description.product + "|" + gamepad.description.serial;
                if (!seen.Add(deviceKey))
                {
                    Debug.Log($"Skipping duplicate device: {gamepad.description.product}");
                    continue;
                }

                playerInputManager.JoinPlayer(playersSpawned, -1, null, gamepad);
                playersSpawned++;
            }
        }

        if (playersSpawned < numberOfPlayers)
        {
            Debug.LogWarning($"Only {playersSpawned} input devices found. Expected {numberOfPlayers} players.");
        }
    }

    // Creates a toggle between only allowing 2 players or allowing infinite players
    // TODO: Despawn extra players when toggle is used after more than 2 have been detected
    public void TogglePlayerLimit(bool enabled)
    {
        if (enabled)
        {
            playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersWhenButtonIsPressed;
        }
        else
        {
            playerInputManager.joinBehavior = PlayerJoinBehavior.JoinPlayersManually;
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

        player[0] = new Mole(playerOne);
        player[1] = new Mole(playerTwo);
    }

    public class Mole
    {
        private GameObject playerObject;

        public Mole(GameObject playerObject)
        {
            this.playerObject = playerObject;
        }
        public Mole(GameObject playerObject, bool controls)
        {
            this.playerObject = playerObject;
        }

        public GameObject PlayerObject { get { return playerObject; } }
    }
}
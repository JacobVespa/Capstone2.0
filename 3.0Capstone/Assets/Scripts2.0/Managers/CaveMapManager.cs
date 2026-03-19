using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.EventSystems;

public class CaveMapInputManager : MonoBehaviour
{
    public static CaveMapInputManager Instance { get; private set; }

    [SerializeField] private int numberOfPlayers = 2;
    [SerializeField] private InputActionAsset uiActionsAsset;

    private GameObject[] playerSelections;
    private MultiplayerEventSystem[] playerEventSystems;
    private int registeredPlayerCount = 0;

    // Tracks which player index was assigned a keyboard so we can
    // poll EventSystem.current for their selection
    private int keyboardPlayerIndex = -1;
    public int KeyboardPlayerIndex => keyboardPlayerIndex;

    public int RegisteredPlayerCount => registeredPlayerCount;

    private void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); return; }
        Instance = this;

        numberOfPlayers = Mathf.Clamp(numberOfPlayers, 1, 2);
        playerSelections   = new GameObject[numberOfPlayers];
        playerEventSystems = new MultiplayerEventSystem[numberOfPlayers];
    }

    private void Start()
    {
        // Keep the shared EventSystem alive — it handles mouse hover and clicks
        var devices = InputSystem.devices;
        int spawned = 0;

        // Pass 1: gamepads get priority
        for (int i = 0; i < devices.Count && spawned < numberOfPlayers; i++)
        {
            if (devices[i] is Gamepad)
            {
                SetupPlayerEventSystem(spawned, devices[i]);
                spawned++;
            }
        }

        // Pass 2: keyboard fills remaining slots
        for (int i = 0; i < devices.Count && spawned < numberOfPlayers; i++)
        {
            if (devices[i] is Keyboard)
            {
                // Record which player index owns the keyboard
                keyboardPlayerIndex = spawned;
                SetupPlayerEventSystem(spawned, devices[i]);
                spawned++;
            }
        }

        if (spawned < numberOfPlayers)
            Debug.LogWarning($"CaveMapInputManager: only {spawned}/{numberOfPlayers} devices found.");

        StartCoroutine(SetInitialSelections());
    }

    private void SetupPlayerEventSystem(int playerIndex, InputDevice device)
    {
        GameObject esGO = new GameObject($"CaveMap_EventSystem_P{playerIndex + 1}");

        MultiplayerEventSystem mes = esGO.AddComponent<MultiplayerEventSystem>();
        InputSystemUIInputModule uiModule = esGO.AddComponent<InputSystemUIInputModule>();

        InputActionAsset clonedActions = Instantiate(uiActionsAsset);

        foreach (var actionMap in clonedActions.actionMaps)
            actionMap.devices = new InputDevice[] { device };

        clonedActions.Enable();
        uiModule.actionsAsset = clonedActions;

        playerEventSystems[playerIndex] = mes;
        registeredPlayerCount++;
    }

    private IEnumerator SetInitialSelections()
    {
        yield return null;

        GameObject first = CaveMap.Instance?.firstSelectedButton?.gameObject;
        if (first == null) yield break;

        for (int i = 0; i < registeredPlayerCount; i++)
            SetPlayerSelection(i, first);
    }

    public void SetPlayerSelection(int playerIndex, GameObject selected)
    {
        if ((uint)playerIndex >= (uint)playerSelections.Length) return;

        playerSelections[playerIndex] = selected;

        MultiplayerEventSystem mes = playerEventSystems[playerIndex];
        if (mes != null && mes.currentSelectedGameObject != selected)
        {
            mes.SetSelectedGameObject(null);
            mes.SetSelectedGameObject(selected);
        }
    }

    public GameObject GetPlayerSelection(int playerIndex)
    {
        if ((uint)playerIndex >= (uint)playerSelections.Length) return null;
        return playerSelections[playerIndex];
    }
}
using UnityEngine;
using UnityEngine.InputSystem;

public class InputControlManager : MonoBehaviour
{
    public GameObject playerPrefab;
    private PlayerInputManager playerInputManager;

    private void Awake()
    {
        playerInputManager = GetComponent<PlayerInputManager>();
    }

    private void Start()
    {
        SpawnPlayer();
        SpawnPlayer();
    }

    public void SpawnPlayer(InputDevice device = null)
    {
        playerInputManager.JoinPlayer(playerInputManager.playerCount, -1,null, device);
    }
}

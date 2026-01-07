using UnityEngine;
using UnityEngine.InputSystem;

public class InputControlManager : MonoBehaviour
{
    private PlayerInputManager playerInputManager;

    private void Awake()
    {
        Cursor.visible = false; //TODO move this to a menu/settings script later
        Cursor.lockState = CursorLockMode.Confined;
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

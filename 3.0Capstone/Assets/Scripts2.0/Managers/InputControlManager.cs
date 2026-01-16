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
        }

        Cursor.visible = false; //TODO move this to a menu/settings script later
        Cursor.lockState = CursorLockMode.Confined;
        playerInputManager = GetComponent<PlayerInputManager>();
    }

    private PlayerInputManager playerInputManager;

    [SerializeField] private int numberOfPlayers = 2;
    [SerializeField] private List<Transform> spawnPoints = new List<Transform>();
    public List<Transform> SpawnPoints { get { return spawnPoints; } }

    private Mole[] player = new Mole[2];
    public Mole[] Player { get { return player; } }

    [Header("Player One Assets")]
    [SerializeField] private Sprite playerOneSprite;

    [Header("Player Two Assets")]
    [SerializeField] private Sprite playerTwoSprite;

    [Header("Shared Player Assets")]
    [SerializeField] private Animator playerAnimator;

    private void Start()
    {
        SetplayerAssets();

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

    public void HasSpawned() // Replace this with a temp list instead of deleting spawnpoints perminantely!
    {
        if (spawnPoints.Count > 0) spawnPoints.RemoveAt(0);
    }

    public void SetplayerAssets()
    {
        if (numberOfPlayers <= 0 || numberOfPlayers > 2) return;

        player[0] = new Mole(playerOneSprite,playerAnimator);
        player[1] = new Mole(playerTwoSprite,playerAnimator);
    }

    public class Mole
    {
        Sprite playerSprite;
        Animator playerAnimator;

        public Mole(Sprite sprite, Animator animator)
        {
            this.playerSprite = sprite;
            this.playerAnimator = animator;
        }

        public Sprite PlayerSprite { get { return playerSprite; } }
        public Animator PlayerAnimator { get { return playerAnimator; } }
    }


}

using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using TMPro;

public class AppraisalStation : MonoBehaviour, IInteractable
{
    [Header("Canvas Elements")]
    [SerializeField] private Canvas appraisalCanvas;
    [SerializeField] private Button appraiseButton;
    [SerializeField] private Button dismantleButton;
    [SerializeField] private TMP_Text gemCountText;
    [SerializeField] private TMP_Text shardCountText;

    [Header("Temporary Upgrade Buttons")]
    [SerializeField] private Button speedUpgradeButton;
    [SerializeField] private Button rateOfFireUpgradeButton;

    [Header("RIG Upgrades")]
    [SerializeField] private RigMovement rigMovement;
    [SerializeField] private float speedUpgradeAmount = 5f;
    [SerializeField] private float maxRigSpeed = 40f;


    [SerializeField] private bool appraising = false;
    [SerializeField] private float toggleCooldown = 0.5f;

    private bool canToggle = true;   // cooldown gate

    private void Start()
    {
        dismantleButton.gameObject.SetActive(false);
        speedUpgradeButton.gameObject.SetActive(false);
        rateOfFireUpgradeButton.gameObject.SetActive(false);

        appraisalCanvas.gameObject.SetActive(false);
    }

    private void Update()
    {
        if (appraising)
        {
            UpdateValues();
        }
    }

    public void Interact(PlayerBody interactor)
    {
        
        if (!canToggle) return;      // block extra calls during cooldown
        StartCoroutine(ToggleWithCooldown(interactor));

    }

    private IEnumerator ToggleWithCooldown(PlayerBody interactor)
    {
        canToggle = false;

        if (!appraising)
            Enter(interactor);
        else
            Exit(interactor);

        yield return new WaitForSeconds(toggleCooldown); // cooldown length

        canToggle = true;
    }

    private void Enter(PlayerBody interactor)
    {
        appraisalCanvas.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        UpdateValues();

        appraising = true;
        interactor.Inputs.enabled = false; // Disable player movement while appraising
        interactor.EnterStation();

    }

    private void Exit(PlayerBody interactor)
    {
        appraisalCanvas.gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        appraising = false;
        interactor.Inputs.enabled = true; // Re-enable player movement
        interactor.ExitStation();
    }

    private void UpdateValues()
    {
        if (GameManager.Instance != null)
        {
            gemCountText.text = "Gems: " + GameManager.Instance.gems;
            shardCountText.text = "Shards: " + GameManager.Instance.shards;
        }
        else
        {
            gemCountText.text = "Gems: N/A";
            shardCountText.text = "Shards: N/A";
        }
    }

    public void AppraseGem()
    {
        if (GameManager.Instance == null) return;

        if (GameManager.Instance.gems > 0)
        {
            speedUpgradeButton.gameObject.SetActive(true);
            rateOfFireUpgradeButton.gameObject.SetActive(true);
            appraiseButton.gameObject.SetActive(false);
            dismantleButton.gameObject.SetActive(true);

            GameManager.Instance.gems -= 1;
        }
        else
        {
            Debug.Log("No gems to appraise.");
        }

    }

    public void DismantleForShards() // Remove hardcode values later
    {
        if (GameManager.Instance == null) return;

        GameManager.Instance.shards += 5000;

        appraiseButton.gameObject.SetActive(true);
        dismantleButton.gameObject.SetActive(false);
        speedUpgradeButton.gameObject.SetActive(false);
        rateOfFireUpgradeButton.gameObject.SetActive(false);
    }

    // HARD CODED UPGRADES FOR DEMONSTRATION PURPOSES ONLY
    public void UpgradeSpeed()
    {
        // TEMP: Rig speed upgrade
        if (rigMovement != null)
        {
            float oldSpeed = rigMovement.rigMoveSpeed;
            float newSpeed = Mathf.Min(oldSpeed + speedUpgradeAmount, maxRigSpeed);
            rigMovement.rigMoveSpeed = newSpeed;

            Debug.Log("Rig speed upgraded from " + oldSpeed + " to " + newSpeed);
        }
        else
        {
            Debug.LogWarning("RigMovement reference not set on AppraisalStation.");
        }

        speedUpgradeButton.gameObject.SetActive(false);
        rateOfFireUpgradeButton.gameObject.SetActive(false);
        appraiseButton.gameObject.SetActive(true);
    }

    public void UpgradeRateOfFire()
    {
        // TEMP: Stub for later weapon integration
        Debug.Log("Rate of Fire upgrade chosen (stub – hook into weapon/rig guns later).");

        speedUpgradeButton.gameObject.SetActive(false);
        rateOfFireUpgradeButton.gameObject.SetActive(false);
        appraiseButton.gameObject.SetActive(true);
    }
}
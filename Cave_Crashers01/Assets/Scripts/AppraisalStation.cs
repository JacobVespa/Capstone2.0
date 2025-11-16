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

    public void Interact(PlayerController interactor)
    {
        if (!canToggle) return;      // block extra calls during cooldown
        StartCoroutine(ToggleWithCooldown(interactor));

    }

    private IEnumerator ToggleWithCooldown(PlayerController interactor)
    {
        canToggle = false;

        if (!appraising)
            Enter(interactor);
        else
            Exit(interactor);

        yield return new WaitForSeconds(toggleCooldown); // cooldown length

        canToggle = true;
    }

    private void Enter(PlayerController interactor)
    {
        appraisalCanvas.gameObject.SetActive(true);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        UpdateValues();

        appraising = true;
        interactor.enabled = false; // Disable player movement while appraising

    }

    private void Exit(PlayerController interactor)
    {
        appraisalCanvas.gameObject.SetActive(false);
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        appraising = false;
        interactor.enabled = true; // Re-enable player movement
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
        // ATTACH TEMP UPGRADE HERE
        
        speedUpgradeButton.gameObject.SetActive(false);
        rateOfFireUpgradeButton.gameObject.SetActive(false);
        appraiseButton.gameObject.SetActive(true);
    }

    public void UpgradeRateOfFire()
    {
        // ATTACH TEMP UPGRADE HERE

        speedUpgradeButton.gameObject.SetActive(false);
        rateOfFireUpgradeButton.gameObject.SetActive(false);
        appraiseButton.gameObject.SetActive(true);
    }
}
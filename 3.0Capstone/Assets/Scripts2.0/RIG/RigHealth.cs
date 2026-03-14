using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class RigHealth : MonoBehaviour, IDamageReceiver
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float currentHealth;
    private float nextThreshold;

    [Tooltip("How many visual damage stages exist")]
    private int damageStages;
    [SerializeField] private GameObject[] damagedAreas; // Max Health / damageAreas == health per stage. This is the required damage to create a damage spot, and can be referenced to heal damage spot
    private List<GameObject> active;
    private List<GameObject> nonActive;

    [Header("UI")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image vignette;

    //[Header("Camera Shake")]
    //[SerializeField] private Camera mainCam;
    //[SerializeField] private float camShakeDur = 0.3f;
    //[SerializeField] private float camShakeStr = 0.1f;

    [SerializeField] private GameObject rigDeath;

    private Vector3 originalCamPos;
    private int lastDamageStage = 0;

    public float Health => currentHealth;
    public float HealthNormalized => currentHealth / maxHealth;

    public float DamageThreshold => maxHealth / damagedAreas.Length;

    private bool canTakeDamage = true;
    public bool CanTakeDamage
    {
        get => canTakeDamage;
        set => canTakeDamage = value;
    }

    [SerializeField] private GameObject gameManager;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = 1f;
            healthBarFill.color = Color.green;
        }

        damageStages = damagedAreas.Length;

        //foreach (var area in damagedAreas)
        //    area.SetActive(false);

        active = new List<GameObject>();
        nonActive = new List<GameObject>();

        for (int i = 0; i < damagedAreas.Length; i++)
        {
            active.Add(damagedAreas[i]);
            active[i].SetActive(false);
        }

        //if (mainCam != null)
            //originalCamPos = mainCam.transform.position;

        nextThreshold = maxHealth - DamageThreshold;
        

        if(GameManager.Instance == null)
        {
            Instantiate(gameManager);
        }
    }


    private void Start()
    {
        SubscribeToEvents();
    }

    public void Attacked(DamageSource d)
    {
        if (d.DamageTarget == DamageSource.DamageType.Player)
            ApplyDamage(d.DamageVal);
    }

    public void ApplyDamage(float damage)
    {
        if (!canTakeDamage) return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);

        UpdateHealthUI();
        UpdateDamageStages();

        CameraCinematic cinematic = FindFirstObjectByType<CameraCinematic>();
        cinematic.ShakeCamera(0.15f,0.25f);
        StartCoroutine(Vignette(Color.red));

        if (currentHealth <= 0f)
            StartCoroutine(Die());
    }

    public void HealDamage() //TODO make sure the player cannot heal a patched hole!
    {
        if (currentHealth <= 0f) return;

        currentHealth = Mathf.Clamp(currentHealth + (DamageThreshold), 0f, maxHealth);
        nextThreshold += DamageThreshold;

        UpdateHealthUI();
        UpdateDamageStages();

        StartCoroutine(Vignette(Color.green));
    }

    private void UpdateHealthUI()
    {
        if (healthBarFill == null) return;

        float t = HealthNormalized;
        healthBarFill.fillAmount = t;
        healthBarFill.color = Color.Lerp(Color.red, Color.green, t);
    }

    private void UpdateDamageStages()
    {
        if (damagedAreas == null || damagedAreas.Length == 0) return;

        //int currentStage = Mathf.FloorToInt((1f - HealthNormalized) * damageStages);

        if (currentHealth <= nextThreshold)
        {
            nextThreshold -= DamageThreshold;
            EnableNextDamagedArea();
        }            
    }

    private void EnableNextDamagedArea()
    {
        if (active.Count == 0)
        {
            active = new List<GameObject>(nonActive);
            nonActive.Clear();
        }

        int rng = Random.Range(0, active.Count);

        if (!active[rng].activeSelf)
        {
            active[rng].SetActive(true);
        }
        else
        {
            active[rng].GetComponentInChildren<RepairPatch>().DeactivatePatch();
        }

        damagedAreas[rng].GetComponentInChildren<ParticleSystem>().Play();
        nonActive.Add(active[rng]);
        active.RemoveAt(rng);
    }

    private IEnumerator Die()
    {
        Debug.Log("Rig ded");
        rigDeath.SetActive(true);
        var rigLossAnim = rigDeath.GetComponent<Animator>();
        rigLossAnim.SetTrigger("PDeath");
        yield return new WaitForSeconds(2.0f);
        GameManager.Instance.GameOverStatus = true;
    }

    private IEnumerator Vignette(Color color)
    {
        if (vignette == null) yield break;

        color.a = 0f;
        vignette.color = color;
        vignette.gameObject.SetActive(true);

        float alphaTarget = 100f / 255f;
        float alpha = 0f;

        while (alpha < alphaTarget)
        {
            alpha += 2f * Time.deltaTime;
            color.a = alpha;
            vignette.color = color;
            yield return null;
        }

        yield return new WaitForSeconds(0.2f);

        while (alpha > 0f)
        {
            alpha -= 2f * Time.deltaTime;
            color.a = alpha;
            vignette.color = color;
            yield return null;
        }

        vignette.gameObject.SetActive(false);
    }

    // private IEnumerator Shake()
    // {
    //     if (mainCam == null) yield break;

    //     float elapsed = 0f;
    //     Vector3 startPos = originalCamPos;

    //     while (elapsed < camShakeDur)
    //     {
    //         float x = Random.Range(-1f, 1f) * camShakeStr;
    //         float y = Random.Range(-1f, 1f) * camShakeStr;

    //         mainCam.transform.position = startPos + new Vector3(x, y, 0f);

    //         elapsed += Time.deltaTime;
    //         yield return null;
    //     }

    //     mainCam.transform.position = startPos;
    // }


    #region AAHHHHHHHHHHHHHHH
    [Header ("Turret Power Events")]
    RigEvents rigEvents = new RigEvents();
    public RigEvents RigEvents => rigEvents;
    [SerializeField] Turret left, right;
    bool isPowerShootSpeed;

    void SubscribeToEvents()
    {
        rigEvents.SpeedStart += StartSpeed;

        isPowerShootSpeed = false;
    }

    public void StartSpeed()
    {
        if (isPowerShootSpeed)
        {
            return;
        }
        StartCoroutine(ShootSpeed());
    }

    IEnumerator ShootSpeed()
    {
        isPowerShootSpeed = true;
        left.shootingCD = 0.1f;
        right.shootingCD = 0.1f;
        yield return new WaitForSeconds(5f);
        left.shootingCD = 0.15f;
        right.shootingCD = 0.15f;
        isPowerShootSpeed = false;

    }


    #endregion
}
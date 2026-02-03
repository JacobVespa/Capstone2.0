using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RigHealth : MonoBehaviour, IDamageReceiver
{
    [Header("Health")]
    [SerializeField] private float maxHealth = 10f;
    [SerializeField] private float currentHealth;

    [Tooltip("How many visual damage stages exist")]
    private int damageStages;
    [SerializeField] private GameObject[] damagedAreas;

    [Header("UI")]
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image vignette;

    [Header("Camera Shake")]
    [SerializeField] private Camera mainCam;
    [SerializeField] private float camShakeDur = 0.3f;
    [SerializeField] private float camShakeStr = 0.1f;

    private Vector3 originalCamPos;
    private int lastDamageStage = 0;

    public float Health => currentHealth;
    public float HealthNormalized => currentHealth / maxHealth;

    private void Awake()
    {
        currentHealth = maxHealth;

        if (healthBarFill != null)
        {
            healthBarFill.fillAmount = 1f;
            healthBarFill.color = Color.green;
        }

        damageStages = damagedAreas.Length;

        foreach (var area in damagedAreas)
            area.SetActive(false);

        if (mainCam != null)
            originalCamPos = mainCam.transform.position;
    }

    public void Attacked(DamageSource d)
    {
        if (d.DamageTarget == DamageSource.DamageType.Player)
            ApplyDamage(d.DamageVal);
    }

    private void ApplyDamage(float damage)
    {
        if (currentHealth <= 0f) return;

        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);

        UpdateHealthUI();
        UpdateDamageStages();

        CameraCinematic cinematic = GameObject.FindObjectsByType<CameraCinematic>(FindObjectsSortMode.None)[0];
        cinematic.ShakeCamera(0.15f,0.25f);
        StartCoroutine(Vignette(Color.red));

        if (currentHealth <= 0f)
            Die();
    }

    public void HealDamage(float amount)
    {
        if (currentHealth <= 0f) return;

        currentHealth = Mathf.Clamp(currentHealth + amount, 0f, maxHealth);

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

        int currentStage = Mathf.FloorToInt((1f - HealthNormalized) * damageStages);

        if (currentStage <= lastDamageStage)
            return;

        lastDamageStage = currentStage;

        EnableNextDamagedArea();
    }

    private void EnableNextDamagedArea()
    {
        foreach (var area in damagedAreas)
        {
            if (!area.activeSelf)
            {
                area.SetActive(true);
                return;
            }
        }
    }

    private void Die()
    {
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
}
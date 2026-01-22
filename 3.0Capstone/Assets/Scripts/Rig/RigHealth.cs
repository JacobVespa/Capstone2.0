
using System.Collections;
using UnityEngine;
using UnityEngine.U2D;
using UnityEngine.UI;



public class RigHealth : MonoBehaviour, IDamageReceiver
{
    [SerializeField] private float health = 100;
    [SerializeField] private float currentHealth;
    [SerializeField] private float lastHealthStep;
    [SerializeField] private GameObject[] damagedAreas;

    //Health bar slop or whatever
    [SerializeField] private Image healthBarFill;
    [SerializeField] private Image vignette;

    //Rig Shake
    [Header("Cam Shake Stats")]
    [SerializeField] public Camera mainCam;
    public float camShakeDur = 0.3f;   // how long the shake lasts
    public float camShakeStr = 0.1f;    // how strong the shake is
    private Vector3 originalPosition;



    public float Health { get { return currentHealth; } set {  currentHealth = value; } }

    private void Awake()
    {
        
        healthBarFill.fillAmount = health;
        healthBarFill.color = Color.green;

        //Set areaOccupied to false, as to indicate a damagedAreaSpawn area is not occupied
        for(int i = 0; i < damagedAreas.Length; i++)
        {
            damagedAreas[i].SetActive(false);
            
        }

        currentHealth = health;
        lastHealthStep = currentHealth / 5;
    }

    public void Attacked(DamageSource d)
    {
        if (d.DamageTarget == DamageSource.DamageType.Player)
        {
            TakeDamage(d.DamageVal);
        }
    }

    private void TakeDamage(float damage)
    {
        currentHealth -= damage;
        healthBarFill.fillAmount -= 1.0f / currentHealth;
        Color newcolor = new Color();
        newcolor.a = 1;
        newcolor.r = 1 - (1 / currentHealth);
        newcolor.g = 1 / currentHealth;
        healthBarFill.color = Color.Lerp(healthBarFill.color, newcolor, 1.0f / currentHealth);


        StartCoroutine(Shake());
        StartCoroutine(Vignette(Color.red));

        float currentStep = currentHealth / 5;

        if (currentHealth <= 0)
        {
            //Debug.Log("Here");
            Death();
        }

        if (currentStep < lastHealthStep)
        {
            
            EnableDamagedArea();
            lastHealthStep = currentStep;
        }
    }

    public void HealDamage(float healed)
    {
        currentHealth += healed;
        healthBarFill.fillAmount += 1.0f / currentHealth;

        //change vignette color
        StartCoroutine(Vignette(Color.green));
        

        if (currentHealth >= 30)
        {
            currentHealth = health;
        }
    }

    private void EnableDamagedArea()
    {
        if (CheckDamagedArea()) { return; }
        int spawnIndex = Random.Range(0, damagedAreas.Length);

        while (true)
        {
            if (damagedAreas[spawnIndex].activeSelf == false)
            {
                damagedAreas[spawnIndex].SetActive(true);
                break;
            }

            spawnIndex = Random.Range(0, damagedAreas.Length);
        }

    }

    private bool CheckDamagedArea()
    {

        foreach (GameObject da in damagedAreas)
        {
            if(da.activeSelf == false) { return false; }
        }

        return true;
    }

    private void Death()
    {
        GameManager.Instance.GameOverStatus = true;
        //this.gameObject.SetActive(false);
    }

    IEnumerator Vignette(Color vigColor)
    {
        vigColor.a = 0f;
        vignette.color = vigColor;
        vignette.gameObject.SetActive(true);
        float alphaEnd = 100f / 255f;
        float alpha = 0.0f;

        while (alpha < alphaEnd)
        {
            alpha += 2.0f * Time.deltaTime;
            vigColor.a = alpha;
            vignette.color = vigColor;
            yield return null;
        }

        vigColor.a = alphaEnd;
        vignette.color = vigColor;

        yield return new WaitForSeconds(0.2f);

        while (alpha > 0.0f)
        {
            alpha -= 2.0f * Time.deltaTime;
            vigColor.a = alpha;
            vignette.color = vigColor;
            yield return null;
        }

        vignette.gameObject.SetActive(false);
    }
    IEnumerator Shake()
    {
        float elapsed = 0f;
        originalPosition = mainCam.transform.position;

        //mainCam = gameObject.transform.parent.gameObject;

        while (elapsed < camShakeDur)
        {
            float x = Random.Range(-1f, 1f) * camShakeStr;
            float y = Random.Range(-1f, 1f) * camShakeStr;

            mainCam.transform.position = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        mainCam.transform.position = originalPosition;
    }

}

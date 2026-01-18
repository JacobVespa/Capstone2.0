
using UnityEngine;



public class RigHealth : MonoBehaviour, IDamageReceiver
{
    [SerializeField] private float health = 100;
    [SerializeField] private float currentHealth;
    [SerializeField] private float lastHealthStep;
    [SerializeField] private GameObject[] damagedAreas;
    
    
    public float Health { get { return health; } set {  health = value; } }

    private void Start()
    {
        

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

        float currentStep = currentHealth / 5;

        if (currentHealth <= 0)
        {
            Debug.Log("Here");
            Death();
        }

        if (currentStep < lastHealthStep)
        {
            Debug.Log(CheckDamagedArea());
            EnableDamagedArea();
            lastHealthStep = currentStep;
        }
    }

    public void HealDamage(float healed)
    {
        currentHealth += healed;

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
        this.gameObject.SetActive(false);
    }
}

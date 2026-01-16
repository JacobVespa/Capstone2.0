using System;
using UnityEngine;

public class RigHealth : MonoBehaviour, IDamageReceiver
{
    [SerializeField] private float health = 100;
    [SerializeField] private float currentHealth;
    [SerializeField] private float lastHealthStep;
    [SerializeField] private GameObject[] damagedAreas;
    private bool[] areaOccupied;
    
    public float Health { get { return health; } set {  health = value; } }

    private void Start()
    {
        areaOccupied = new bool[damagedAreas.Length];

        //Set areaOccupied to false, as to indicate a damagedAreaSpawn area is not occupied
        for(int i = 0; i < damagedAreas.Length; i++)
        {
            damagedAreas[i].SetActive(false);
            areaOccupied[i] = false;
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

    public void SetAreaFalse(GameObject area)
    {
        for (int i = 0; i < damagedAreas.Length; ++i)
        {
            if (damagedAreas[i] == area)
            {
                areaOccupied[i] = false;
            }
        }
    }

    private void EnableDamagedArea()
    {
        int spawnIndex = UnityEngine.Random.Range(0, damagedAreas.Length);

        if (areaOccupied[spawnIndex] == false)
        {
            damagedAreas[spawnIndex].SetActive(true);
            areaOccupied[spawnIndex] = true;
        }
        else
        {
            EnableDamagedArea();
        }
        
    }

    private void Death()
    {
        this.gameObject.SetActive(false);
    }
}

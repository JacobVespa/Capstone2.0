using UnityEngine;

public class Hammer : MonoBehaviour
{
    
    private DamageSource currentDamage;

    void Start()
    {
        currentDamage = GetComponent<DamageSource>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            Debug.Log("Hit the tick");
        }
    }

}

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
            //Debug.Log("Hit the tick");
            var body = other.GetComponent<EnemyBody>();
            body.Attacked(currentDamage);
        }
        if(other.CompareTag("Item"))
        {
            SpriteRenderer spriteRenderer = other.GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;
            Debug.Log("GOT THE ITEM");
        }
    }

}

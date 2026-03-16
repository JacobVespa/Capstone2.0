using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Hammer : MonoBehaviour
{
    
    private DamageSource currentDamage;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] hammerMiss;
    [SerializeField] private AudioClip[] hammerHit;

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
            int clipIndex = Random.Range(0, hammerHit.Length);
            audioSource.clip = hammerHit[clipIndex];
            audioSource.Play();
        }
        else
        {
            int clipIndex = Random.Range(0, hammerMiss.Length);
            audioSource.clip = hammerMiss[clipIndex];
            audioSource.Play();
        } 

        if (other.CompareTag("Item"))
        {
            SpriteRenderer spriteRenderer = other.GetComponent<SpriteRenderer>();
            spriteRenderer.enabled = false;
            Light2D light2D = other.GetComponentInChildren<Light2D>();
            light2D.enabled = false;
            Debug.Log("GOT THE ITEM");
            Item item = other.GetComponent<Item>();
            item.GainItemEffect(item.itemType);
            item.StartCoroutine(item.DespawnItem());
        }
    }

}

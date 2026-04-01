using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Hammer : MonoBehaviour
{
    
    private DamageSource currentDamage;

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] hammerMiss;
    [SerializeField] private AudioClip[] hammerHit;
    public SpriteRenderer aoeSprite;
    private Vector3 initialAOESize;
    public bool isPoweredUp;

    void Start()
    {
        currentDamage = GetComponent<DamageSource>();
        initialAOESize = new Vector3(3, 3, 3);
        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.CompareTag("Enemy"))
        {
            Debug.Log("Hit an enemy");
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
            //SpriteRenderer spriteRenderer = other.GetComponent<SpriteRenderer>();
            //spriteRenderer.enabled = false;
            Debug.Log("GOT THE ITEM");
            Item item = other.GetComponent<Item>();
            item.GainItemEffect(item.itemType);
        }
    }

    public void CallStupidPulse()
    {
        //StartCoroutine(AOEPulse(initialAOESize, new Vector3(9, 9, 9), 0.67f));
        Debug.Log("this is supposed to pulse ig");
    }

    private IEnumerator AOEPulse(Vector3 initialSize, Vector3 finalSize, float pulseDuration)
    {
        aoeSprite.transform.localScale = initialSize;

        float timer = 0f;

        while (timer < pulseDuration)
        {
            aoeSprite.transform.localScale = Vector3.Lerp(initialSize, finalSize, timer / pulseDuration);
            timer += Time.deltaTime;
            yield return null;
        }
    }

}

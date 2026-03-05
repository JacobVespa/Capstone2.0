using UnityEngine;
using System.Collections;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

public class Resource : MonoBehaviour
{
    [SerializeField] Sprite[] crystalStates;
    private int damageVar = 0;

    [SerializeField] int OriginalHP = 3;
    private int currentHP;

    private int breakMulitplier = 2;
    private int damageMulitplier = 1;

    [SerializeField] private ParticleSystem crystalCrack;
    [SerializeField] private ParticleSystem shardScatter;
    [SerializeField] private GameObject crystalShine;
    private Color gemColor;

    [SerializeField] AudioClip resource;
    [SerializeField] AudioSource audioSource;

    void Start()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = crystalStates[0];
        
        originalPosition = transform.localPosition;
        currentHP = OriginalHP;
        gemColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        GetComponentInChildren<SpriteRenderer>().color = gemColor;
        shardScatter.startColor = gemColor;

    }

    public void Damage()
    {
        if (currentHP > 0)
        {
            damageVar++;
            if (damageVar < crystalStates.Length)
            {
                
                GetComponentInChildren<SpriteRenderer>().sprite = crystalStates[damageVar];
            }

                int previousHP = currentHP;
            crystalCrack.Play();
            shardScatter.Play();
            audioSource.Play();
            //shardScatter.Play();
            StartCoroutine(Shake());
            currentHP--;
            if (currentHP <= 0)
            {
                GetComponentInChildren<SpriteRenderer>().enabled = false;
            }

            RewardShards(-(currentHP - previousHP), currentHP <= 0);
        }
        

    }

    public void Respawn()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = crystalStates[0];
        damageVar = 0;
        gameObject.GetComponentInChildren<SpriteRenderer>().enabled = true;
        gemColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        GetComponentInChildren<SpriteRenderer>().color = gemColor;
        shardScatter.startColor = gemColor;
        currentHP = OriginalHP;
        crystalShine.SetActive(true);
        GetComponentInChildren<Light2D>().enabled = true;
    }

    private void RewardShards(int shards, bool broken)
    {
        if(GameManager.Instance == null) { return; }
        if (broken)
        {
            GameManager.Instance.AddShards(shards * breakMulitplier);
            crystalShine.SetActive(false);
            GetComponentInChildren<Light2D>().enabled = false;
        }
        else
        {
            GameManager.Instance.AddShards(shards * damageMulitplier);
        }
    }

    
    public float shakeDuration = 0.3f;   // how long the shake lasts
    public float shakeStrength = 0.1f;    // how strong the shake is
    private Vector3 originalPosition;

    IEnumerator Shake()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeStrength;
            float y = Random.Range(-1f, 1f) * shakeStrength;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }

    private IEnumerator RespawnAfterDestroyed()
    {
        yield return new WaitForSeconds(5f);
        Respawn();
    }

}

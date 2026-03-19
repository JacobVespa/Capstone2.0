using UnityEngine;
using System.Collections;
using UnityEngine.Rendering.Universal;

public class Resource : MonoBehaviour
{
    [SerializeField] private Sprite[] crystalStates;
    private int damageVar = 0;

    [SerializeField] private int OriginalHP = 3;
    private int currentHP;

    private int breakMulitplier = 2;
    private int damageMulitplier = 1;

    [SerializeField] private ParticleSystem crystalCrack;
    [SerializeField] private ParticleSystem shardScatter;
    [SerializeField] private GameObject crystalShine;
    private Color gemColor;

    [SerializeField] private AudioClip resource;
    [SerializeField] private AudioSource audioSource;

    // Item stuff
    [SerializeField] private bool hasItem;
    private int randomItemNum;
    [SerializeField] private ItemTypes itemManager;
    private ItemTypes.Items currentItem = ItemTypes.Items.NONE;

    public float shakeDuration = 0.3f;
    public float shakeStrength = 0.1f;
    private Vector3 originalPosition;

    private void Start()
    {
        GetComponentInChildren<SpriteRenderer>().sprite = crystalStates[0];

        originalPosition = transform.localPosition;
        currentHP = OriginalHP;

        gemColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        GetComponentInChildren<SpriteRenderer>().color = gemColor;
        shardScatter.startColor = gemColor;
        GetComponentInChildren<Light2D>().color = gemColor;

        ItemInitialization();
        Debug.Log("item that this gem has: " + currentItem);
    }

    private void ItemInitialization()
    {
        currentItem = ItemTypes.Items.NONE;

        if (hasItem)
        {
            randomItemNum = Random.Range(0, 3);

            switch (randomItemNum)
            {
                case 0:
                    currentItem = ItemTypes.Items.RapidFire;
                    break;
                case 1:
                    currentItem = ItemTypes.Items.ShockwaveHammer;
                    break;
                case 2:
                    currentItem = ItemTypes.Items.RepairBurst;
                    break;
                default:
                    currentItem = ItemTypes.Items.NONE;
                    break;
            }
        }
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

            if (crystalCrack != null) crystalCrack.Play();
            if (shardScatter != null) shardScatter.Play();
            if (audioSource != null) audioSource.Play();

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
        GetComponentInChildren<SpriteRenderer>().enabled = true;

        gemColor = new Color(Random.Range(0f, 1f), Random.Range(0f, 1f), Random.Range(0f, 1f));
        GetComponentInChildren<SpriteRenderer>().color = gemColor;
        shardScatter.startColor = gemColor;

        currentHP = OriginalHP;
        crystalShine.SetActive(true);
        GetComponentInChildren<Light2D>().enabled = true;

        ItemInitialization();
        Debug.Log("item that this gem has: " + currentItem);
    }

    private void RewardShards(int shards, bool broken)
    {
        if (GameManager.Instance == null) return;

        if (broken)
        {
            GameManager.Instance.AddShards(shards * breakMulitplier);

            crystalShine.SetActive(false);
            GetComponentInChildren<Light2D>().enabled = false;

            if (hasItem && currentItem != ItemTypes.Items.NONE && itemManager != null)
            {
                itemManager.SpawnItem(currentItem);
            }
        }
        else
        {
            GameManager.Instance.AddShards(shards * damageMulitplier);
        }
    }

    private IEnumerator Shake()
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
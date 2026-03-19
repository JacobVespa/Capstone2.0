using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Hammer : MonoBehaviour
{
    private DamageSource currentDamage;

    [Header("Audio")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] hammerMiss;
    [SerializeField] private AudioClip[] hammerHit;

    [Header("Shockwave Power-Up")]
    [SerializeField] private SpriteRenderer aoeSprite;
    [SerializeField] private float shockwaveRadius = 10f;
    [SerializeField] private float shockwaveDuration = 0.4f;
    [SerializeField] private Vector3 pulseStartScale = new Vector3(2.5f, 2.5f, 2.5f);
    [SerializeField] private Vector3 pulseEndScale = new Vector3(9f, 9f, 9f);

    // Optional stuff that would be nice to have but isn't required for the core shockwave functionality:
    [Header("Powered Hammer Visuals")]
    [SerializeField] private float poweredHammerScaleMultiplier = 1.35f;
    [SerializeField] private SpriteRenderer poweredGlowSprite;
    [SerializeField] private Light2D poweredLight;

    private Vector3 originalHammerScale;
    private bool shockwavePoweredUp;
    private bool pulseOnCooldown;

    private void Start()
    {
        currentDamage = GetComponent<DamageSource>();
        originalHammerScale = transform.localScale;

        if (aoeSprite != null)
        {
            aoeSprite.enabled = false;
            aoeSprite.transform.localScale = pulseStartScale;
        }

        if (poweredGlowSprite != null)
        {
            poweredGlowSprite.enabled = false;
        }

        if (poweredLight != null)
        {
            poweredLight.enabled = false;
        }

        gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            Debug.Log("Hit an enemy");

            EnemyBody body = other.GetComponent<EnemyBody>();
            if (body != null)
            {
                body.Attacked(currentDamage);
            }

            PlayRandomClip(hammerHit);

            if (shockwavePoweredUp && !pulseOnCooldown)
            {
                TriggerShockwave();
            }
        }
        else if (other.CompareTag("Item"))
        {
            SpriteRenderer spriteRenderer = other.GetComponent<SpriteRenderer>();
            if (spriteRenderer != null) spriteRenderer.enabled = false;

            Light2D light2D = other.GetComponentInChildren<Light2D>();
            if (light2D != null) light2D.enabled = false;

            Collider2D itemCollider = other.GetComponent<Collider2D>();
            if (itemCollider != null) itemCollider.enabled = false;

            Debug.Log("GOT THE ITEM");

            Item item = other.GetComponent<Item>();
            if (item != null)
            {
                item.GainItemEffect(item.itemType);
                item.StartCoroutine(item.DespawnItem());
            }
        }
        else
        {
            PlayRandomClip(hammerMiss);
        }
    }

    private void PlayRandomClip(AudioClip[] clips)
    {
        if (audioSource == null || clips == null || clips.Length == 0) return;

        int clipIndex = Random.Range(0, clips.Length);
        audioSource.clip = clips[clipIndex];
        audioSource.Play();
    }

    public void ActivateShockwaveHammer(float duration)
    {
        StopCoroutine(nameof(ShockwaveHammerCoroutine));
        StartCoroutine(ShockwaveHammerCoroutine(duration));
    }

    private IEnumerator ShockwaveHammerCoroutine(float duration)
    {
        shockwavePoweredUp = true;

        // Make hammer visibly stronger
        transform.localScale = originalHammerScale * poweredHammerScaleMultiplier;

        if (poweredGlowSprite != null)
        {
            poweredGlowSprite.enabled = true;
        }

        if (poweredLight != null)
        {
            poweredLight.enabled = true;
        }

        yield return new WaitForSeconds(duration);

        shockwavePoweredUp = false;
        transform.localScale = originalHammerScale;

        if (poweredGlowSprite != null)
        {
            poweredGlowSprite.enabled = false;
        }

        if (poweredLight != null)
        {
            poweredLight.enabled = false;
        }
    }

    // DESIGN INTENT:
    // The idea here is that the hammer enters a powered-up state that acts
    // like a bit of a panic button sort of. When it's active, hammer hits can send
    // out a shockwave that clears enemies around the RIG and buys the player
    // some breathing room.
    //
    // Right now it just uses the normal damage call, but if enemy scaling
    // later makes that feel weak, this could easily be swapped for a direct
    // enemy clear / despawn method instead.
    // Let me know if you agree with this
    private void TriggerShockwave()
    {
        StartCoroutine(ShockwaveCooldown(0.15f));
        StartCoroutine(AOEPulse());

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, shockwaveRadius);

        foreach (Collider2D hit in hits)
        {
            if (!hit.CompareTag("Enemy")) continue;
            if (hit.gameObject == gameObject) continue;

            EnemyBody body = hit.GetComponent<EnemyBody>();
            if (body != null)
            {
                // Can be changed later to be stronger according to the difficulty curve if needed, but for now it just uses the normal hammer damage
                body.Attacked(currentDamage);

                StartCoroutine(ClearEnemyVisual(hit.gameObject));
            }
        }
    }

    private IEnumerator AOEPulse()
    {
        if (aoeSprite == null) yield break;

        aoeSprite.enabled = true;
        aoeSprite.transform.localScale = pulseStartScale;

        float timer = 0f;

        while (timer < shockwaveDuration)
        {
            aoeSprite.transform.localScale = Vector3.Lerp(
                pulseStartScale,
                pulseEndScale,
                timer / shockwaveDuration
            );

            timer += Time.deltaTime;
            yield return null;
        }

        aoeSprite.transform.localScale = pulseStartScale;
        aoeSprite.enabled = false;
    }

    private IEnumerator ShockwaveCooldown(float duration)
    {
        pulseOnCooldown = true;
        yield return new WaitForSeconds(duration);
        pulseOnCooldown = false;
    }

    // I think it would be cool to have a Thanos style snap effect if not too much work
    private IEnumerator ClearEnemyVisual(GameObject enemy)
    {
        if (enemy == null) yield break;

        SpriteRenderer sr = enemy.GetComponentInChildren<SpriteRenderer>();
        if (sr != null)
        {
            Color original = sr.color;
            sr.color = Color.white;
            yield return new WaitForSeconds(0.04f);
            sr.color = original;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, shockwaveRadius);
    }
}
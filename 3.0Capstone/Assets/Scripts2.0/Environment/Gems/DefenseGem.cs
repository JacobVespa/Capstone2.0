using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class DefenseGem : MonoBehaviour
{
    [SerializeField] private ParticleSystem crystalCrack;
    [SerializeField] private ParticleSystem shardScatter;
    [SerializeField] private GameObject crystalShine;
    private Color gemColor;

    [SerializeField] AudioClip resource;
    [SerializeField] AudioSource audioSource;

    public float shakeDuration = 0.9f;   // how long the shake lasts
    public float shakeStrength = 0.1f;    // how strong the shake is
    private Vector3 originalPosition;

    private void Start()
    {
        originalPosition = transform.localPosition;
    }

    public void ScatterShard()
    {
        audioSource.Play();
        shardScatter.Play();
    }

    public void ShakeCrystal(float delay)
    {
        StartCoroutine(Shake(delay));
    }

    public void Damage()
    {
        StartCoroutine(Shake(0.8f));
    }

    public void BreakCrystal(float delay)
    {
        StartCoroutine(Shake(delay));
        shardScatter.Play();
        shardScatter.Play();
        this.GetComponentInChildren<SpriteRenderer>().enabled = false;
    }

    IEnumerator Shake(float delay)
    {
        yield return new WaitForSeconds(delay);

        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeStrength;
            float y = Random.Range(-1f, 1f) * shakeStrength;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        crystalCrack.Play();
        shardScatter.Play();
        audioSource.Play();

        transform.localPosition = originalPosition;
    }
}

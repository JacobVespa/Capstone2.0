using UnityEngine;
using System.Collections;
using System.Xml.Serialization;

public class Resource : MonoBehaviour
{

    [SerializeField] int OriginalHP = 3;
    private int currentHP;

    void Start()
    {
        originalPosition = transform.localPosition;
        currentHP = OriginalHP;
    }

    public void Damage()
    {
        StartCoroutine(Shake());
        currentHP--;
        if (currentHP <= 0) Destroy(gameObject);
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
}

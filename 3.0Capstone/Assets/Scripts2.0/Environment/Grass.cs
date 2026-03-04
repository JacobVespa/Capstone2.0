using System.Collections;
using UnityEngine;

public class Grass : MonoBehaviour
{
    private SpriteRenderer grassRenderer;
    private ParticleSystem grassParticle;
    private float grassTime = 0.7f; //length of time for how long the object should exist for

    private void Start()
    {
        grassRenderer = GetComponent<SpriteRenderer>();
        grassParticle = GetComponentInChildren<ParticleSystem>();
    }

    public void grassClipped()
    {
        //StartCoroutine(GrassCoroutine(grassTime));
        grassParticle.Play();
    }

    IEnumerator GrassCoroutine(float grassTime)
    {
        grassRenderer.enabled = false;
        grassParticle.Play();
        yield return new WaitForSeconds(grassTime);
        grassParticle.Stop();

    }
}

using System.Collections;
using UnityEngine;

public class PlayHammerSounds : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] hammerMiss;
    [SerializeField] private AudioClip[] hammerHit;
    [SerializeField] private Hammer Hammer;

    public bool inUse = false;

    private void Start()
    {
        inUse = false;
        
    }

    public void PlayHammerCoroutine()
    {
        
        if (!inUse) StartCoroutine(playHammerSound());
    }

    public IEnumerator playHammerSound()
    {
        if(inUse)
        {
            yield break;
        }
        inUse = true;
        yield return new WaitForSeconds(0.18f);

        if (Hammer.Hit)
        {
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

        yield return new WaitForSeconds(0.16f);
        inUse = false;
    }
}

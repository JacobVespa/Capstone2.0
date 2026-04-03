using UnityEngine;

public class PlayHammerSounds : MonoBehaviour
{

    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip[] hammerMiss;
    [SerializeField] private AudioClip[] hammerHit;
    [SerializeField] private Hammer Hammer;

    public void playHammerSound()
    {
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
        Debug.Log(Hammer.Hit);
    }
}

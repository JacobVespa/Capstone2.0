using UnityEngine;

public class RepairStation : MonoBehaviour
{
    [SerializeField] AudioClip repairStation;
    [SerializeField] AudioSource audioSource;

    //instantiating the ammo round
    public void PlayPickupSound()
    {
        audioSource.Play();
    }

}

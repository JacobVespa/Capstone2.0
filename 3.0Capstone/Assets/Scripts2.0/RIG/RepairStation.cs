using UnityEngine;

public class RepairStation : MonoBehaviour
{
    [SerializeField] AudioClip repairStation;
    [SerializeField] AudioSource audioSource;
    [SerializeField] public GameObject buttonPromptXB;

    private void Start()
    {
        buttonPromptXB.SetActive(false);
    }

    //instantiating the ammo round
    public void PlayPickupSound()
    {
        audioSource.Play();
    }

}

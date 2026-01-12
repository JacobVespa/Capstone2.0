using UnityEngine;

public class AmmoBox : MonoBehaviour
{
    [SerializeField] AudioClip ammoBox;
    [SerializeField] AudioSource audioSource;
    //reference to the ammo round
    public GameObject ammoRound;

    //instantiating the ammo round
    public void SpawnAmmo()
    {
        audioSource.Play();
        //Instantiate(ammoRound);
    }

}

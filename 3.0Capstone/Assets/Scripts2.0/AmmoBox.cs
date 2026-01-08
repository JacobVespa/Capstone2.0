using UnityEngine;

public class AmmoBox : MonoBehaviour
{

    //reference to the ammo round
    public GameObject ammoRound;

    //instantiating the ammo round
    public void SpawnAmmo()
    {
        Instantiate(ammoRound);
    }

}

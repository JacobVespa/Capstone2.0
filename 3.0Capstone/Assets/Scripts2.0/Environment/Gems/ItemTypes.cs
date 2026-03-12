using System.Collections;
using UnityEngine;

public class ItemTypes : MonoBehaviour
{
    //Could have power up class which contains all powerups and activates when destroyed

    [SerializeField] private GameObject[] items;

    public enum Items
    {
        RapidFire,
        LargeHammer,
        NONE
    }

    //spawning the power-up crystal
    public void SpawnItem(int index)
    {
        switch(index)
        {
            case 0:
                //spawn RapidFire crystal
                Instantiate(items[index]);
                break;
            case 1:
                //spawn LargeHammer crystal
                Instantiate(items[index]);
                break;
            default:
                Debug.Log("No crystal found");
                break;
        }
    }

}

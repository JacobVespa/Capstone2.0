using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemTypes : MonoBehaviour
{
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
        GameObject go;
        switch(index)
        {
            case 0:
                //spawn RapidFire crystal
                go = Instantiate(items[index]);
                go.transform.position = new Vector3(0, -8, 0);
                break;
            case 1:
                //spawn LargeHammer crystal
                go = Instantiate(items[index]);
                go.transform.position = new Vector3(0, -8, 0);
                break;
            default:
                Debug.Log("No crystal found");
                break;
        }
    }

}

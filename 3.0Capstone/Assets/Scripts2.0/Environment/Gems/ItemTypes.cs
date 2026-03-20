using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;

public class ItemTypes : MonoBehaviour
{
    [SerializeField] private GameObject[] items;
    [SerializeField] private Vector2 rigXBounds = new Vector2(-1.2f, 1.2f);
    [SerializeField] private Vector2 rigYBounds = new Vector2(-0.5f, 2.7f);

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
        Vector3 spawnPos = new Vector3(Random.Range(rigXBounds.x, rigXBounds.y), Random.Range(rigYBounds.x, rigYBounds.y), GameManager.Instance.RigObject.transform.position.z);
        switch(index)
        {
            case 0:
                //spawn RapidFire crystal
                go = Instantiate(items[index]);
                go.transform.position = GameManager.Instance.RigObject.transform.position + spawnPos;
                break;
            case 1:
                //spawn LargeHammer crystal
                go = Instantiate(items[index]);
                go.transform.position = GameManager.Instance.RigObject.transform.position + spawnPos;
                break;
            default:
                Debug.Log("No crystal found");
                break;
        }
    }

}

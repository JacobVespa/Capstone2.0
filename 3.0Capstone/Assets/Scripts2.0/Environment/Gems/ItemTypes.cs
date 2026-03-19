using UnityEngine;

public class ItemTypes : MonoBehaviour
{
    [SerializeField] private GameObject[] items;
    [SerializeField] private Vector2 rigXBounds = new Vector2(-1.2f, 1.2f);
    [SerializeField] private Vector2 rigYBounds = new Vector2(-0.5f, 2.7f);

    public enum Items
    {
        RapidFire,
        ShockwaveHammer,
        RepairBurst,
        NONE
    }

    // Spawns the selected power-up crystal somewhere on the RIG
    public void SpawnItem(Items itemType)
    {
        if (itemType == Items.NONE)
        {
            Debug.Log("No item selected to spawn.");
            return;
        }

        int index = (int)itemType;

        if (index < 0 || index >= items.Length)
        {
            Debug.LogWarning("Item index out of range for itemType: " + itemType);
            return;
        }

        if (GameManager.Instance == null || GameManager.Instance.RigObject == null)
        {
            Debug.LogWarning("GameManager or RigObject is missing, cannot spawn item.");
            return;
        }

        Vector3 localOffset = new Vector3(
            Random.Range(rigXBounds.x, rigXBounds.y),
            Random.Range(rigYBounds.x, rigYBounds.y),
            0f
        );

        Vector3 spawnPos = GameManager.Instance.RigObject.transform.position + localOffset;

        GameObject go = Instantiate(items[index], spawnPos, Quaternion.identity);

        Item itemComponent = go.GetComponent<Item>();
        if (itemComponent != null)
        {
            itemComponent.itemType = itemType;
        }
    }
}
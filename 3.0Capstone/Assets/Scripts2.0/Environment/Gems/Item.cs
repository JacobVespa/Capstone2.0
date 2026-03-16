using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] public ItemTypes.Items itemType;

    //RAPIDFIRE REFERENCE
    private RigHealth rig;

    RigEvents local;

    private void Start()
    {
        rig = FindFirstObjectByType<RigHealth>();
       
        local = rig.RigEvents;
    }

    public void GainItemEffect(ItemTypes.Items currentItem)
    {
        switch(currentItem)
        {
            case ItemTypes.Items.RapidFire:
                //gain rapid fire effect
                Debug.Log("FIRE RATE GO BRRRRRRRR");
                local.CallSpeedStart();
                break;
            case ItemTypes.Items.LargeHammer:
                //gain large hammer effect
                break;
            default:
                Debug.Log("No item");
                break;
        }
    }

    public IEnumerator DespawnItem()
    {
        yield return new WaitForSeconds(3);
        gameObject.SetActive(false);
    }

}




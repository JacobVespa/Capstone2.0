using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] public ItemTypes.Items itemType;

    //RAPIDFIRE REFERENCE
    private RigHealth rig;
    RigEvents local;

    //HAMMER REFERENCE, I hope....
    private Hammer hammerRef;
    private GameObject hammer;
    private BoxCollider2D boxCollider;
    private CircleCollider2D circleCollider;
    private SpriteRenderer aoeSprite;

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
                HammerSlop();
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

    //trying something goofy and stupid
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Hammer"))
        {
            hammer = collision.gameObject;
        }
    }

    private void HammerSlop()
    {
        boxCollider = hammer.GetComponent<BoxCollider2D>();
        circleCollider = hammer.GetComponent<CircleCollider2D>();
        hammerRef = hammer.GetComponent<Hammer>();
        aoeSprite = hammerRef.aoeSprite;

        boxCollider.enabled = false;
        circleCollider.enabled = true;
        aoeSprite.enabled = true;

        //IMPORTANT LINE
        hammerRef.isPoweredUp = true;
    }

}




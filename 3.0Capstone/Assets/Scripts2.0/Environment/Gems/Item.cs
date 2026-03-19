using System.Collections;
using UnityEngine;

public class Item : MonoBehaviour
{
    [SerializeField] public ItemTypes.Items itemType;

    private RigHealth rig;
    private RigEvents local;
    private Hammer hammerRef;

    [Header("Power-Up Durations")]
    [SerializeField] private float shockwaveDuration = 8f;

    private void Start()
    {
        rig = FindFirstObjectByType<RigHealth>();
        hammerRef = FindFirstObjectByType<Hammer>();

        if (rig != null)
        {
            local = rig.RigEvents;
        }
    }

    public void GainItemEffect(ItemTypes.Items currentItem)
    {
        switch (currentItem)
        {
            case ItemTypes.Items.RapidFire:
                Debug.Log("FIRE RATE GO BRRRRRRRR");
                if (local != null)
                {
                    local.CallSpeedStart();
                }
                break;

            case ItemTypes.Items.ShockwaveHammer:
                Debug.Log("SHOCKWAVE HAMMER ACTIVATED");
                if (hammerRef != null)
                {
                    hammerRef.ActivateShockwaveHammer(shockwaveDuration);
                }
                break;

            case ItemTypes.Items.RepairBurst:
                Debug.Log("REPAIR BURST ACTIVATED");
                if (rig != null)
                {
                    rig.FullRepairBurst();
                }
                break;

            default:
                Debug.Log("No item");
                break;
        }
    }

    public IEnumerator DespawnItem()
    {
        yield return new WaitForSeconds(3f);
        gameObject.SetActive(false);
    }

    /**
    //trying something goofy and stupid
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Hammer"))
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
    **/
}
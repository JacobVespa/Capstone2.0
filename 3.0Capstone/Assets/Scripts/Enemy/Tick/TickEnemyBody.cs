using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class TickEnemyBody : EnemyBody
{

    private Rigidbody2D rb;

    private Vector2 notifPos;

    public bool dropping = false;

    protected override void Awake()
    {
        base.Awake();
        if (rb == null) { rb = GetComponent<Rigidbody2D>(); }
        
        if(notifPos == Vector2.zero) { notifPos = attackNotif.transform.position; }
        
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Attack(GameObject target)
    {
        if (attackTimer < attackStartUp) { return; }

        base.Attack(target);

        if (target.TryGetComponent<IDamageReceiver>(out IDamageReceiver dr))
        {
            dr.Attacked(damageSource);
        }
        else { Debug.LogError("No damage receiver found on target"); }
    }

    public IEnumerator DropOnRig(Vector2 targetPos)
    {
        
        dropping = true;
        transform.position = new Vector2(targetPos.x, targetPos.y + 75);
        rb.gravityScale = 1;
        Collider2D colliders = GetComponent<Collider2D>();
        colliders.enabled = false;
        attackNotif.transform.position = notifPos;
        

        attackNotif.SetActive(true);

        while (true)
        {

            yield return null;
            rb.gravityScale += 1 * Time.deltaTime;
            attackNotif.transform.position = notifPos;
            
            if (transform.position.y < targetPos.y ) 
            {
                rb.linearVelocityY *= -0.5f;
                
                if(rb.linearVelocityY <= 10){ break; }
                transform.position = targetPos;
            }
        }
        
        attackNotif.transform.position = notifPos;
        attackNotif.SetActive(false);

        rb.gravityScale = 0;
        rb.linearVelocityY = 0;

        dropping = false;
        colliders.enabled = true;
        ai.behaviour = EnemyAI.Behaviour.Ready;
        ai.AddToAttackQueue();
    }
}
    

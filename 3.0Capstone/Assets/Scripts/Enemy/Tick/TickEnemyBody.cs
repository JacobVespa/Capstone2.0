using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

/*  TickEnemyBody is used mainly for dropping the tick on the rig and attacking
 * 
 *  contains:
 *  - varibles for rigidbody, dropping boolean, and position the attack notifcation should be
 *  - Coroutine for dropping the tick on the ship
 *  - method for attacking
 */

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

    // same implementation as the melee enemy
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

    //  sets tick postiion to above its intial placement then drops it 
    //  it will bounce until it loses enough speed than transition to its next state
    //  note:   current implementation is pretty shaky since i just decided 75 was a good heigth
    //          to drop it from and then fine tuned the varibles so it would bounce in a way I personally liked
    //          
    public IEnumerator DropOnRig(Vector2 targetPos)
    {
        
        dropping = true;
        transform.position = new Vector2(targetPos.x, targetPos.y + 75); 
        sprite.SetActive(true);
        rb.gravityScale = 1;
        Collider2D colliders = GetComponent<Collider2D>();  // turn off colliders when dropping 
        colliders.enabled = false;
        attackNotif.transform.position = notifPos;
        

        attackNotif.SetActive(true);
        // continually increase gravity on tick,
        // when it passes the position in will land at its vertical velocity is halved and reversed(bouncing)
        // once its vertical velocity is low enough it will stop bouncing and land in position
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
    

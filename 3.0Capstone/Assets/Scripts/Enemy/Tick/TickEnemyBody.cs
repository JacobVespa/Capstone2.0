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
        
        if(notifPos == Vector2.zero) { notifPos = attackNotif.transform.position; Debug.Log("notif"); }
        
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public override void Attack(GameObject target)
    {
        if (attackCooldown < attackRate) { return; }
        base.Attack(target);

        if (target.TryGetComponent<IDamageReceiver>(out IDamageReceiver dr))
        {
            dr.Attacked(damageSource);
        }
        else { Debug.LogError("No damage receiver found on target"); }
    }

    public IEnumerator DropOnRig(Vector2 targetPos)
    {
        Debug.Log("drop");
        dropping = true;
        transform.position = new Vector2(targetPos.x, targetPos.y + 75);
        rb.gravityScale = 1;

        attackNotif.transform.position = targetPos - notifPos;
        attackNotif.SetActive(true);

        while (true)
        {

            yield return null;
            rb.gravityScale += 1 * Time.deltaTime;
            attackNotif.transform.position = targetPos - notifPos;
            if (transform.position.y <= targetPos.y ) 
            {
                
                Debug.Log("hit");
                Debug.Log(rb.gravityScale);
                Debug.Log(rb.linearVelocityY);
                if(rb.linearVelocityY <= -1) 
                {
                    
                    rb.linearVelocityY *= -0.6f; 
                    rb.gravityScale *= 0.4f;
                }
                else if (rb.linearVelocityY > -1) { Debug.Log("stop"); break;  }
                transform.position = targetPos;
            }
            
            
        }
        Debug.Log("stop drop");
        attackNotif.transform.position = notifPos;
        rb.gravityScale = 0;
        rb.linearVelocityY = 0;
        dropping = false;
        ai.behaviour = EnemyAI.Behaviour.Attack;
        //attackNotif.transform = notifPos;
    }
}
    

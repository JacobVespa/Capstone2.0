using System.Collections;
using UnityEngine;

public class RunnerEnemyBody: EnemyBody
{
    

    protected Vector2 notifPos;

    public bool dropping = false;

    private bool invincible = false;

    protected override void Awake()
    {
        base.Awake();

        if (rb == null) { rb = GetComponent<Rigidbody2D>(); }

        if (notifPos == Vector2.zero) { notifPos = attackNotif.transform.position; }

    }

    public override void Attacked(DamageSource d)
    {
        //if (invincible) {  return;  }
        base.Attacked(d);
        
        //StartCoroutine(Iframes());
    }

    void Start()
    {
        if (DifficultyManager.Instance != null)
        {
            this.Health = DifficultyManager.Instance.RunnerHealth + 1;
        }
    }

    protected override void FixedUpdate()
    {
        
        base.FixedUpdate();
    }
    

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 9)
        {
            //Debug.LogError("Whoops");
            //TakeDamage(health);
        }
    }

    IEnumerator Iframes()
    {
        invincible = true;
        //moveSpeed *= 1.5f;
        
        yield return new WaitForSeconds(0.5f);

        invincible = false;
        //moveSpeed *= 2 / 3;
    }

    public override void Attack(GameObject target)
    {
        if (attackTimer < attackStartUp) { return; }

        base.Attack(target);
        int clipIndex = Random.Range(0, attackClip.Length);
        audioSource.clip = attackClip[clipIndex];
        audioSource.Play();
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
        sprite.SetActive(true);
        rb.gravityScale = 1;
        Collider2D[] colliders = GetComponents<Collider2D>();  // turn off colliders when dropping 
        foreach (Collider2D c in colliders) { c.enabled = false; }


        // continually increase gravity on tick,
        // when it passes the position in will land at its vertical velocity is halved and reversed(bouncing)
        // once its vertical velocity is low enough it will stop bouncing and land in position
        while (true)
        {

            yield return null;
            rb.gravityScale += 1 * Time.deltaTime;
            attackNotif.transform.position = notifPos;

            if (transform.position.y < targetPos.y)
            {
                rb.linearVelocityY *= -0.2f;

                if (rb.linearVelocityY <= 10) { break; }
                transform.position = targetPos;
            }
        }

        attackNotif.transform.position = notifPos;
        attackNotif.SetActive(false);

        rb.gravityScale = 0;
        rb.linearVelocityY = 0;

        dropping = false;
        foreach (Collider2D c in colliders) { c.enabled = true; }
        ai.behaviour = EnemyAI.Behaviour.Ready;
        ai.AddToAttackQueue();
    }
}
using System.Collections;
using UnityEngine;

public class BigTickEnemyBody : TickEnemyBody
{

    private bool invincible = false;

    protected override void Awake()
    {
        base.Awake();

        if (rb == null) { rb = GetComponent<Rigidbody2D>(); }

        if (notifPos == Vector2.zero) { notifPos = attackNotif.transform.position; }

    }

    public override void Attacked(DamageSource d)
    {
       // if (invincible) { Debug.Log("Denied");  return;  }
        base.Attacked(d);
        
        StartCoroutine(Iframes());
    }

    void Start()
    {
        if (DifficultyManager.Instance != null)
        {
            this.Health = DifficultyManager.Instance.BigTickHealth + 1;
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
        moveSpeed *= 1.5f;
        Debug.Log("hit");
        yield return new WaitForSeconds(0.5f);
        Debug.Log("done");
        invincible = false;
        moveSpeed *= 2 / 3;
    }
}
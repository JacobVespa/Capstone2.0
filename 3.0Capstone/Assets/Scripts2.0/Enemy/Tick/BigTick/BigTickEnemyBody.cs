using System.Collections;
using UnityEngine;

public class BigTickEnemyBody : TickEnemyBody
{



    protected override void Awake()
    {
        base.Awake();

        if (rb == null) { rb = GetComponent<Rigidbody2D>(); }

        if (notifPos == Vector2.zero) { notifPos = attackNotif.transform.position; }

    }

    void Start()
    {
        if (DifficultyManager.Instance != null)
        {
            this.Health = DifficultyManager.Instance.TickHealth + 1;
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

}
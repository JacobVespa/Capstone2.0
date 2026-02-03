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

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

}
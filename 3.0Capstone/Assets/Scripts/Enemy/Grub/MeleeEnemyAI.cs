using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class MeleeEnemyAI : EnemyAI
{
    protected MeleeEnemyBody body;

    private Vector2 targetDist = Vector3.zero;
    private float totalDist = 0;

    private bool inRange = false;

    private Vector2 moveInput;
    

    protected override void Awake()
    {
        base.Awake();
        if (body == null) { body = GetComponent<MeleeEnemyBody>(); }
        
        if (hasTarget == true){ targetDist = transform.position - target.transform.position; totalDist = targetDist.magnitude; }
        behaviour = Behaviour.Moving;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void AIFlowChart()
    {
        
        if (!hasTarget) return;

        switch (behaviour)
        {
            case Behaviour.Moving:
                ApproachTarget();
                break;

            case Behaviour.Ready:
                moveInput = Vector2.zero;
                TryAttackTarget();
                break;
            
        }

    }

    private void ApproachTarget()
    {
        if (!hasTarget) { moveInput = Vector2.zero; return; }

        targetDist = target.transform.position - transform.position;

        totalDist = targetDist.magnitude;

        moveInput = new Vector2(targetDist.x, targetDist.y);
        moveInput = moveInput.normalized;

        body.InputDir = moveInput;
    }

    private void TryAttackTarget()
    {
        if (!hasTarget) { return; }

        body.Attack(target);
        
    }

    public void CheckIfTarget()
    {
        if (target == null) { hasTarget = false; }
        else { hasTarget = true; }
    }



    

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (behaviour == Behaviour.Dead) return;
        if (collision.isTrigger) return;
        /*
        if (collision.gameObject == target)
        {
            behaviour = Behaviour.Attack;
            inRange = true;
        }
        */

        if (target != null && collision.transform.root == target.transform)
        {
            behaviour = Behaviour.Ready;
            inRange = true;
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (behaviour == Behaviour.Dead) return;
        if (collision.isTrigger) return;
        /*
        if (collision.gameObject == target)
        {
            behaviour = Behaviour.None;
            inRange = false;
            
        }
        */

        if (target != null && collision.transform.root == target.transform )
        {
            
            behaviour = Behaviour.None;
            inRange = false;
        }

    }
}


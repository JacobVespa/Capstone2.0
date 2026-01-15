using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class GrubEnemyAI : EnemyAI
{
    protected GrubEnemyBody body;

    private Vector2 targetDist = Vector3.zero;
    private float totalDist = 0;

    private bool inRange = false;

    private Vector2 moveInput;
    

    protected override void Awake()
    {
        base.Awake();
        if (body == null) { body = GetComponent<GrubEnemyBody>(); }
        
        if (hasTarget == true){ targetDist = transform.position - target.transform.position; totalDist = targetDist.magnitude; }
        behaviour = Behaviour.Moving;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void AIFlowChart()
    {
        /*
        if (!hasTarget) { return; }

        //Debug.Log(totalDist);

        if((int)behaviour == 1)
        {
            if(behaviour != Behaviour.None) { behaviour = Behaviour.None; }
            ApproachTarget();
            body.AttackCooldown = 0;
        }
        else if((int)behaviour == 2)
        {
            if(behaviour != Behaviour.Attack) { behaviour = Behaviour.Attack; }
            
            moveInput = Vector2.zero;
            AttackTarget();
        }
        */
        if (!hasTarget) return;

        switch (behaviour)
        {
            case Behaviour.Moving:
                ApproachTarget();
                break;

            case Behaviour.Attack:
                moveInput = Vector2.zero;
                AttackTarget();
                break;

            default:
                // If it ever gets set to None, recover gracefully
                behaviour = Behaviour.Moving;
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

    private void AttackTarget()
    {

        if (!hasTarget) { return; }

        Vector3 attackAim = target.transform.position - transform.position;
        attackAim = attackAim.normalized;
        body.Attack(target);
        if (target.activeSelf == false)
        {
            target = null;
            hasTarget = false;
            inRange = false;

        }
    }

    public void CheckIfTarget()
    {
        if (target == null) { hasTarget = false; }
        else { hasTarget = true; }
    }



    

    private void OnTriggerEnter2D(Collider2D collision)
    {
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
            behaviour = Behaviour.Attack;
            inRange = true;
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.isTrigger) return;
        /*
        if (collision.gameObject == target)
        {
            behaviour = Behaviour.None;
            inRange = false;
            
        }
        */

        if (target != null && collision.transform.root == target.transform)
        {
            behaviour = Behaviour.Attack;
            inRange = true;
        }

    }
}


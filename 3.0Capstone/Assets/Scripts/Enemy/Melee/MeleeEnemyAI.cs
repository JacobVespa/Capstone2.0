using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class MeleeEnemyAI : EnemyAI
{
    protected MeleeEnemyBody body;

    

    protected override void Awake()
    {
        base.Awake();
        if (body == null) { body = GetComponent<MeleeEnemyBody>(); }
        
        
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
                if (agent.CanDealDamage)
                {
                    behaviour = Behaviour.Attacking;
                }
                break;
            case Behaviour.Attacking:
                body.Attack(target);
                break;
            case Behaviour.CoolDown:
                MoveToBottomOfQueue();
                break;
        }

    }

    protected override void ApproachTarget()
    {
        base.ApproachTarget();
        body.InputDir = moveInput;
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
            AddToAttackQueue();
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
            RemoveFromAttackQueue();
        }
        

    }
}


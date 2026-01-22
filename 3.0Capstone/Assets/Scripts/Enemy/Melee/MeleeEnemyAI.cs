using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class MeleeEnemyAI : EnemyAI
{
    protected MeleeEnemyBody body;

    protected override void Awake()
    {
        base.Awake();
        body = GetComponent<MeleeEnemyBody>();
        
        if (body == null)
        {
            Debug.LogError("MeleeEnemyBody component not found!", this);
        }
        
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
                TryAttackTarget();
                break;
            case Behaviour.CoolDown:
                MoveToBottomOfQueue();
                break;
        }
    }

    protected override void ApproachTarget()
    {
        base.ApproachTarget();
        
        if (body != null)
        {
            body.InputDir = moveInput;
        }
    }

    private void TryAttackTarget()
    {
        if (!hasTarget || body == null || target == null) return;

        body.Attack(target);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (behaviour == Behaviour.Dead) return;
        if (collision.isTrigger) return;

        if (target != null && collision.transform.root == target.transform)
        {
            behaviour = Behaviour.Ready;
            AddToAttackQueue();
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (behaviour == Behaviour.Dead) return;
        if (collision.isTrigger) return;

        if (target != null && collision.transform.root == target.transform)
        {
            behaviour = Behaviour.None;
            RemoveFromAttackQueue();
        }
    }
}
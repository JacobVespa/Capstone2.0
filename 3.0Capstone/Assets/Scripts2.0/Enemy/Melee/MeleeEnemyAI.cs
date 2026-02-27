using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;
using YourNamespace;

/*  MeleeEnemyAI determines the actions of melee enemies takes, what state they are in and hwo they transition states
 * 
 *  contains:
 *  - varible for EnemyBody
 *  - AI Flowchart whcih tells the meleeEnemtBody what to do depending on the current state its in
 *  - OnTriggerEnter/Exit for when the enemy gets in attack range of its target
 */

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
        switch (behaviour)
        {
            case Behaviour.Moving: 
                ApproachTarget();
                break;
            case Behaviour.Ready:
                moveInput = Vector2.zero;
                if (agent.CanDealDamage) { behaviour = Behaviour.Attacking; }
                break;
            case Behaviour.Knockback:
                moveInput = Vector2.zero; // Make the move input zero ad apply the knockback else where
                break;
            case Behaviour.Attacking:
                TryAttackTarget();
                break;
            case Behaviour.CoolDown:    // change so that it starts a cooldown timer and only goes to back of queue once the timer is done
                if (body.CheckCoolDown()) { MoveToBottomOfQueue();}
                break;
            case Behaviour.Dead:
                if (CheckDeathPlayed()) { DestroyEnemy(); }
                break;
        }
    }

    private void TryAttackTarget()
    {
        if (!hasTarget) { return; }
        if(body.attackNotif.activeSelf == false) { body.attackNotif.SetActive(true); }
        body.Attack(attackTarget);
        
    }

    // if an enemy enters a trigger of its target is will be placed in queue and set behaviour to ready
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (behaviour == Behaviour.Dead) return;
        if (collision.isTrigger) return;

        

        if (attackTarget != null && collision.transform.root == targetLoc.transform.root && CheckInView())
        {
            behaviour = Behaviour.Ready;
            AddToAttackQueue();
        }
    }

    // if the enemy is exits a trigger of its target it will be set back to moving and removed from the attack queue
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (behaviour == Behaviour.Dead) return;
        if (collision.isTrigger) return;

        if (attackTarget != null && collision.transform.root == targetLoc.transform.root)
        {
            behaviour = Behaviour.Moving;
            RemoveFromAttackQueue();
        }
    }
}
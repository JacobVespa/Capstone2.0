using UnityEngine;
using YourNamespace;

public class ThumperEnemyAI : MeleeEnemyAI
{
    protected ThumperEnemyBody Tbody;

    protected override void Awake()
    {
        base.Awake();

        Tbody = GetComponent<ThumperEnemyBody>();
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
                if (agent.CanDealDamage && !Tbody.shield) { behaviour = Behaviour.Attacking; }
                break;
            case Behaviour.Knockback:
                moveInput = Vector2.zero; // Make the move input zero and apply the knockback else where
                break;
            case Behaviour.Attacking:
                TryAttackTarget();
                break;
            case Behaviour.CoolDown:    // change so that it starts a cooldown timer and only goes to back of queue once the timer is done
                if (body.CheckCoolDown()) { MoveToBottomOfQueue(); }
                break;
            case Behaviour.Dead:
                if (CheckDeathPlayed()) { DestroyEnemy(); }
                break;
        }
    }

    /*
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (behaviour == Behaviour.Dead) return;
        if (collision.isTrigger) return;
       


        if (attackTarget != null && collision.transform.root == targetLoc.transform.root && CheckInView(0.99f) && !Tbody.shield)
        {
            behaviour = Behaviour.Ready;
            AddToAttackQueue();
        }
    }
    */
}

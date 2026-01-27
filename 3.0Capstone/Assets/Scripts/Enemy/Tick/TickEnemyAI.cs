using UnityEngine;

/*  TickEnemyAI determiens what actiosn the body takes depending on the state
 * 
 *  contains;
 *  - varibles for TickEnemyBody and intial position for it to land from
 *  - AIFlowCHart to determine its actiosn dpeending on its state
 */

public class TickEnemyAI : EnemyAI
{
    protected TickEnemyBody body;

    private Vector2 dropPos;

    protected override void Awake()
    {
        base.Awake();
        
        behaviour = Behaviour.Spawning;
        
        if(body == null) { body = GetComponent<TickEnemyBody>(); }
        if(dropPos == Vector2.zero) { dropPos = gameObject.transform.position;  }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void AIFlowChart()
    {

        switch(behaviour)
        {
            case Behaviour.Spawning:
                if (!body.dropping) { StartCoroutine(body.DropOnRig(dropPos)); }
                break;
            case Behaviour.Ready:
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

    private void TryAttackTarget()
    {
        if (!hasTarget) { return; }
        if (body.attackNotif.activeSelf == false) { body.attackNotif.SetActive(true); }
        body.Attack(target);
    }

}

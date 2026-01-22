using UnityEngine;

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
                body.Attack(target);
                break;
            case Behaviour.CoolDown:
                MoveToBottomOfQueue();
                break;
        }
    }

}

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
        if(dropPos == null) { dropPos = transform.position;}
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void AIFlowChart()
    {
        if((int)behaviour == 4)
        {
            body.DropOnRig(dropPos);
        }
    }

}

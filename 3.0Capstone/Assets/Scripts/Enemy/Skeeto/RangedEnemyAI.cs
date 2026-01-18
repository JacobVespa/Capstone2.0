using UnityEngine;

public class RangedEnemyAI : EnemyAI
{
    protected RangedEnemyBody body;

    protected override void Awake()
    {
        base.Awake();
        if (body == null) { body = GetComponent<RangedEnemyBody>(); }
        behaviour = Behaviour.Attack;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void AIFlowChart()
    {
        
        if((int) behaviour == 2)
        {
            body.Attack(target);
        }
        
        
    }
}

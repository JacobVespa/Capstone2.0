using UnityEngine;

public class SkeetoEnemyAI : EnemyAI
{
    protected SkeetoEnemyBody body;

    protected override void Awake()
    {
        base.Awake();
        if (body == null) { body = GetComponent<SkeetoEnemyBody>(); }
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

using UnityEngine;

public class SkeetoEnemyAI : EnemyAI
{
    protected SkeetoEnemyBody body;

    protected override void Awake()
    {
        base.Awake();
        if (body == null) { body = GetComponent<SkeetoEnemyBody>(); }
        
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void AIFlowChart()
    {
        

        behaviour = Behaviour.Attack;
        body.Attack(target);
    }
}

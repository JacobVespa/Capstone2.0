using UnityEngine;

public class SkeetoEnemyAI : EnemyAI
{
    protected SkeetoEnemyBody body;

    private void Awake()
    {

        if (body == null) { body = GetComponent<SkeetoEnemyBody>(); }
        if (target == null) { hasTarget = false; }
    }

    private void FixedUpdate()
    {
        AIFlowChart();
    }
    protected override void AIFlowChart()
    {
        

        behaviour = Behaviour.Attack;
        body.Attack(target);
    }
}

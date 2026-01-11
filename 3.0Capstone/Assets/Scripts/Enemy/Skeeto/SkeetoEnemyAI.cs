using UnityEngine;

public class SkeetoEnemyAI : EnemyAI
{
    protected SkeetoEnemyBody body;

    private void Start()
    {
        if (body == null) { body = GetComponent<SkeetoEnemyBody>(); }
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

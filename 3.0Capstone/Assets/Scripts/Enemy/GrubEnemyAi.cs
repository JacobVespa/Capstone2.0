using UnityEngine;

public class GrubEnemyAI : EnemyAI
{
    protected GrubEnemyBody body;

    private Vector2 targetDist = Vector3.zero;
    private float totalDist = 0;

    private Vector2 moveInput;

    private void Start()
    {
        if (body == null) { body = GetComponent<GrubEnemyBody>(); }
        if (target == null) { hasTarget = false; }
        else { hasTarget = true; targetDist = transform.position - target.transform.position; totalDist = targetDist.magnitude; }
    
    }

    protected override void AIFlowChart()
    {
        if (!hasTarget) { return; }

        if(totalDist >= body.AttackRange*2/3)
        {
            ApproachTarget();
            body.AttackCooldown = 0;
        }
        else
        {
            moveInput = Vector2.zero;
            //AttackTarget();
        }
    }

    private void ApproachTarget()
    {
        if (!hasTarget) { moveInput = Vector2.zero; return; }

        targetDist = target.transform.position - transform.position;

        totalDist = targetDist.magnitude;

        moveInput = new Vector2(targetDist.x, targetDist.y);
        moveInput = moveInput.normalized;

        body.InputDir = moveInput;
    }

    private void AttackTarget()
    {

        if (!hasTarget) { return; }

        Vector3 attackAim = target.transform.position - transform.position;
        attackAim = attackAim.normalized;
        body.Attack(attackAim);
        if (target.activeSelf == false)
        {
            target = null;
            hasTarget = false;

        }
    }

    public void CheckIfTarget()
    {
        if (target == null) { hasTarget = false; }
        else { hasTarget = true; }
    }
}


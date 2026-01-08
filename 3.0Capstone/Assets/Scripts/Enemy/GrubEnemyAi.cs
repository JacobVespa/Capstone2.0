using UnityEngine;

public class GrubEnemyAI : EnemyAI
{
    protected GrubEnemyBody body;

    private Vector2 targetDist = Vector3.zero;
    private float totalDist = 0;

    private bool inRange = false;

    private Vector2 moveInput;
    private Vector2 contact;

    private void Start()
    {
        if (body == null) { body = GetComponent<GrubEnemyBody>(); }
        if (target == null) { hasTarget = false; }
        else { hasTarget = true; targetDist = transform.position - target.transform.position; totalDist = targetDist.magnitude; }
    
    }

    protected override void AIFlowChart()
    {
        if (!hasTarget) { return; }

        //Debug.Log(totalDist);

        if(!inRange)
        {
            ApproachTarget();
            body.AttackCooldown = 0;
        }
        else if(inRange)
        {
            moveInput = Vector2.zero;
            AttackTarget();
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
            inRange = false;

        }
    }

    public void CheckIfTarget()
    {
        if (target == null) { hasTarget = false; }
        else { hasTarget = true; }
    }



    private void OnCollisionEnter2D(Collision2D collision)
    {
        
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {

        Debug.Log("hit");
        if(collision.gameObject == target)
        {
            inRange = true;

            

        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.gameObject == target)
        {
            inRange = false;
            contact = Vector2.zero;
        }
    }
}


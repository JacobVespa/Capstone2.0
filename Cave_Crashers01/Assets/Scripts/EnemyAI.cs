using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(EnemyBody))]
public class EnemyAI : MonoBehaviour
{
    [SerializeField] private EnemyBody body;
    [SerializeField] private GameObject target;
    [SerializeField] private bool hasTarget;
    private Vector3 targetDist = Vector3.zero;
    private float totalDist = 0;

    private Vector3 moveInput;

    private void Start()
    {
        if (body == null) { body = GetComponent<EnemyBody>(); }
        if (target == null) { hasTarget = false; } 
        else { hasTarget = true; targetDist = transform.position - target.transform.position; totalDist = targetDist.magnitude; }
    }

    private void Update()
    {
        AIFlowChart();
    }

    private void AIFlowChart()
    {
        if (!hasTarget) { return; }
        
        if(totalDist >= body.AttackRange*2/3)
        {
            ApproachTarget();
        }
        else
        {
            moveInput = Vector3.zero;
            
            AttackTarget();
        }
        
            
        
    }

    private void ApproachTarget()
    {
        if (!hasTarget) {moveInput = Vector3.zero; return; }

        targetDist = transform.position - target.transform.position ;

        totalDist = targetDist.magnitude ;

        moveInput = new Vector3(targetDist.x, 0, targetDist.z);
        moveInput = moveInput.normalized;

        body.InputDir = moveInput;
    }

    private void AttackTarget()
    {
        if (!hasTarget) { return; }

        Vector3 attackAim = target.transform.position - transform.position ;
        attackAim = attackAim.normalized;
        body.Attack(attackAim);
        if(target.active == false)
        {
            target = null;
            hasTarget = false;
            
        }
    }
}

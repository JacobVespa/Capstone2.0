using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(EnemyBody))]
public abstract class EnemyAI : MonoBehaviour
{
    
     

    [SerializeField] protected GameObject target;
    [SerializeField] protected bool hasTarget;
    [SerializeField] public AttackQueueAgent agent;
    public Behaviour behaviour;

    
    protected Vector2 targetDist = Vector3.zero;
    protected float totalDist;
    protected bool inRange = false;
    protected Vector2 moveInput;


    public enum Behaviour
    {
        None,
        Moving,     // enemy is moving toward rig(melee only)
        Ready,      // enemy is in range of the rig is able to attack but waiting for its turn in queue
        //ChargeUp 
        Attacking,  // enemy is currently attacking the rig
        CoolDown,
        Dead,       // enemy is dead/dying
        Spawning,   // enemy is spawning in
    }

    protected virtual void FixedUpdate()
    {
        AIFlowChart();
    }

    protected virtual void Awake()
    {
        
        if (target == null) { hasTarget = false; }
        else { hasTarget = true; }
        if (hasTarget == true) { targetDist = transform.position - target.transform.position; }
        if (agent == null) { agent = GetComponent<AttackQueueAgent>(); }
    }

    protected abstract void AIFlowChart();

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
        hasTarget = (target != null);
    }

    protected virtual void ApproachTarget()
    {
        if (!hasTarget) { moveInput = Vector2.zero; return; }

        targetDist = target.transform.position - transform.position;

        moveInput = new Vector2(targetDist.x, targetDist.y);
        moveInput = moveInput.normalized;
    }


    public void AddToAttackQueue()
    {
        agent.OnEnterAttackZone(AttackQueueManager.instance, AttackQueueManager.instance.transform);
    }

    public void RemoveFromAttackQueue()
    {
        agent.OnExitAttackZone();
    }

    public void MoveToBottomOfQueue()
    {
        RemoveFromAttackQueue();
        AddToAttackQueue();

        behaviour = Behaviour.Ready;
    }
}

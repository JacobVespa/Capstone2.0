using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(EnemyBody))]
public abstract class EnemyAI : MonoBehaviour
{
    //public virtual EnemyBody body { get; set; }
   

    [SerializeField] protected GameObject target;
    [SerializeField] protected bool hasTarget;
    [SerializeField] public AttackQueueAgent agent;
    public Behaviour behaviour;

    public enum Behaviour
    {
        None,
        Moving,     // enemy is moving toward rig(melee only)
        Ready,      // enemy is in range of the rig is able to attack but waiting for its turn in queue
        Attacking,  // enemy is currently attacking the rig
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
        if(agent == null) { agent = GetComponent<AttackQueueAgent>(); }
    }

    protected abstract void AIFlowChart();

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
        hasTarget = (target != null);
    }

    public void AddToAttackQueue()
    {
        agent.OnEnterAttackZone(AttackQueueManager.instance, AttackQueueManager.instance.transform);
    }

    public void RemoveFromAttackQueue()
    {
        agent.OnExitAttackZone();
    }
}

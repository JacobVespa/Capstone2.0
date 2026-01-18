using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(EnemyBody))]
public abstract class EnemyAI : MonoBehaviour
{
    //public virtual EnemyBody body { get; set; }
   

    [SerializeField] protected GameObject target;
    [SerializeField] protected bool hasTarget;
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
    }

    protected abstract void AIFlowChart();

    public void SetTarget(GameObject newTarget)
    {
        target = newTarget;
        hasTarget = (target != null);
    }

}

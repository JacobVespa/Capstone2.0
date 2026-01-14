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
        None = 0,
        Moving = 1,
        Attack = 2,
        Dead = 3,
        Spawning = 4,
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

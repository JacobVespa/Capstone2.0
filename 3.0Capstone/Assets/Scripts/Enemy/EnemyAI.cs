using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(EnemyBody))]
public abstract class EnemyAI : MonoBehaviour
{
    //public virtual EnemyBody body { get; set; }

    [SerializeField] protected GameObject target;
    [SerializeField] protected bool hasTarget;

    private void Update()
    {
        AIFlowChart();
    }

    private void Start()
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

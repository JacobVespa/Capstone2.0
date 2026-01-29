using UnityEngine;
using UnityEngine.UIElements;


/*  EnemyAI is a base class with varibles and methods that all or most enemyAI will need and tells the enemyBody what actions to perform.
 *  
 * 
 *  contains:
 *  - varible to for what the enemies current state is 
 *  - varibles for target
 *  - virtual method of lifetime or event methods(fixed updated, awake)
 *  - method for finding the direction of the target and setting the target
 *  - method for adding, removing, or moving to the bottome of the attack queue
 *  
 *  no enemyBody varible since it could only be for the base enemyBody class 
 */

[RequireComponent(typeof(EnemyBody))]
public abstract class EnemyAI : MonoBehaviour
{
    private EnemyBody baseBody;
    public Behaviour behaviour;

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

    [SerializeField] public AttackQueueAgent agent;

    [SerializeField] protected GameObject attackTarget;
    [SerializeField] protected GameObject targetLoc;
    
    [SerializeField] protected bool hasTarget;
    
    protected Vector2 targetDist = Vector3.zero;
    protected float totalDist;
    protected bool inRange = false;
    protected Vector2 moveInput;


    

    protected virtual void FixedUpdate()
    {
        AIFlowChart();
    }

    protected virtual void Awake()
    {
        if (targetLoc == null) { targetLoc = attackTarget; }
        if (baseBody == null) { baseBody = GetComponent<EnemyBody>(); }
        if (attackTarget == null) { hasTarget = false; }
        else { hasTarget = true; }
        if (targetLoc != null) { targetDist = transform.position - targetLoc.transform.position; }
        if (agent == null) { agent = GetComponent<AttackQueueAgent>(); }
    }

    //  method all EnemyAI need to implement which determines the enemyBody's action depending on the current state
    protected abstract void AIFlowChart();

    public void SetTarget(GameObject attack, GameObject loc)
    {
        attackTarget = attack;
        targetLoc = loc;
        hasTarget = (attackTarget != null);
    }

    //  find the direction between the enemy and the target
    //  can't send the direction to the body since the body is only implemnted in the enemy specific AI script
    protected virtual void ApproachTarget()
    {
        if (!hasTarget) { moveInput = Vector2.zero; return; }

        targetDist = targetLoc.transform.position - transform.position;

        moveInput = new Vector2(targetDist.x, targetDist.y);
        moveInput = moveInput.normalized;
    }

    // adds, removes or moves the enemy to the back fo the attack queue
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

    // checks if the animation being played is death and if its done then returns true or false
    // NOTE: aniamtion state must have a tag called "Death" for this function to work
    protected bool CheckDeathPlayed()
    {
        if (baseBody.Animator == null) { return true; }
        
            AnimatorStateInfo state = baseBody.Animator.GetCurrentAnimatorStateInfo(0);
        
        if (state.IsTag("Death") && state.normalizedTime >= 1)
        {
            return true;
        }
        else { return false; }
    }

    protected void DestroyEnemy()
    {
        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddKills(1);
        }
        Destroy(this.gameObject);
    }
}

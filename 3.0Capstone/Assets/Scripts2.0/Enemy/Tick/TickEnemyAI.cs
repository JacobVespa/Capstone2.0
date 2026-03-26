using UnityEditor.Recorder;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

/*  TickEnemyAI determiens what actiosn the body takes depending on the state
 * 
 *  contains;
 *  - varibles for TickEnemyBody and intial position for it to land from
 *  - AIFlowCHart to determine its actiosn dpeending on its state
 */

public class TickEnemyAI : EnemyAI
{
    protected TickEnemyBody body;

    protected Vector2 dropPos;

    protected override void Awake()
    {
        base.Awake();
        
        behaviour = Behaviour.Spawning;
        
        if(body == null) { body = GetComponent<TickEnemyBody>(); }
        if(dropPos == Vector2.zero) { dropPos = gameObject.transform.position;  }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    protected override void AIFlowChart()
    {

        switch(behaviour)
        {
            case Behaviour.Spawning:
                if (!body.dropping) { StartCoroutine(body.DropOnRig(dropPos)); }
                break;
            case Behaviour.Attacking:
                if(body.attackNotif.activeSelf != true) { body.attackNotif.SetActive(true); }
                if (CheckExploded())
                {
                    DeathDamage();
                    Destroy(this.gameObject);
                }
                break;

            case Behaviour.Dead:
                //if (CheckDeathPlayed()) {
                    DestroyEnemy(); 
                //}
                break;
        }
    }

    protected bool CheckExploded()
    {
        AnimatorStateInfo state = baseBody.Animator.GetCurrentAnimatorStateInfo(0);

        if (state.IsTag("Explode") && state.normalizedTime >= 1)
        {
            return true;
        }
        else return false;
    }


    protected void DeathDamage()
    {
        if (attackTarget.TryGetComponent<IDamageReceiver>(out IDamageReceiver dr))
        {
            dr.Attacked(GetComponent<DamageSource>());
        }
    }

}

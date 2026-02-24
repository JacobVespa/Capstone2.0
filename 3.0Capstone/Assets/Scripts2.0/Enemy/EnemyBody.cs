using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;




/*  EnemyBody is a base class that holds varibles and methods that lets enemy perform actions(move, attack ,etc) that all or most enemies will need
 *  
 *  contains:
 *  - instances of key varibles that will be used in all or most enemy body implementation
 *  - virtual method of lifetime method and event method (fixed update, awake)
 *  - methods all enemies will need or need ot implement(attack, attacked, death, etc)
 */

//[RequireComponent(typeof(EnemyAI))]
[RequireComponent(typeof(DamageSource))]
public class EnemyBody : MonoBehaviour, IDamageReceiver
{
    [Header("Required Components")]
    [SerializeField] protected EnemyAI ai;  // only the base class of enemyAI, methods in enemy spcific AI scripts can't be called, must implemneted in the base class
    [SerializeField] protected Animator animator;
    [SerializeField] protected GameObject sprite;
    [SerializeField] protected DamageSource damageSource;

    public Animator Animator { get { return animator; } }
    public GameObject Sprite { get { return sprite; } }

    [Header("Visual Effect Componenets")]
    [SerializeField] public GameObject attackNotif;
    [SerializeField] private ParticleSystem comicHurt;
    [SerializeField] private ParticleSystem bugGoo;
    


    [Header("Movement Stats")]
    [SerializeField] protected float moveSpeed = 5;
    protected Vector3 motion = Vector2.zero;
    protected Vector2 inputDir = Vector2.zero;
    public Vector2 InputDir { get { return inputDir; } set { inputDir = value; } }

    [Header("Shake Stats")]
    public float shakeDuration = 0.3f;   // how long the shake lasts
    public float shakeStrength = 0.1f;    // how strong the shake is
    private Vector3 originalPosition;

    [Header("Combat Stats")]
    [SerializeField] protected float health = 3;
    public float Health {  get { return health; } set {  health = value; } }

    [SerializeField] protected float attackStartUp = 3;
    public float AttackStartUp { get {  return attackStartUp; } set { attackStartUp = value; } }

    [SerializeField] protected float attackCoolDown = 3;
    public float AttackCoolDown { get { return attackCoolDown; } set { attackCoolDown = value; } }

    protected float attackTimer = 0;
    public float AttackTimer { get { return AttackTimer; } set { AttackTimer = value; } }


    protected virtual void Awake()
    {
        if(attackNotif != null)
        {
            attackNotif.SetActive(false);
        }
        
        attackTimer = 0;
        
        if(damageSource == null)
        {
            damageSource = GetComponent<DamageSource>();
        }
        if(animator == null && sprite != null)
        {
            sprite.TryGetComponent<Animator>(out Animator a);
            {
                animator = a;
            }
        }   
    }

    protected virtual void FixedUpdate()
    {
        UpdateStartUp();
        UpdateMovement();
    }

    //  attackTimer increases only if the enemy is in attacking state or in the cooldown state
    //  or if attackTimer value is less than the attackStartUp or attackCoolDown values
    private void UpdateStartUp()
    {
        if(ai.behaviour != EnemyAI.Behaviour.Attacking && ai.behaviour != EnemyAI.Behaviour.CoolDown) { return; }
        if(attackTimer >= attackStartUp && attackTimer >= attackCoolDown) { return; }
        //if (ai.agent != null && !ai.agent.CanDealDamage) { return; }
        
        attackTimer += 1 * Time.fixedDeltaTime;
        //if(attackNotif.activeSelf == false  && attackTimer >= AttackStartUp/2) {  }
          
    }

    //  all enemy attacks should reset the attack timer, turn off the attack notif and switch behaviour to cooldown
    public virtual void Attack(GameObject target) 
    {
        attackTimer = 0;
        attackNotif.SetActive(false);
        
        ai.behaviour = EnemyAI.Behaviour.CoolDown;
    }

    public virtual bool CheckCoolDown()
    {
        if( attackTimer >= attackCoolDown)
        {
            attackTimer = 0;
            return true;
        }
        else { return false; }
    }

    #region Handle Movement
    //  takes a directional input given from the ai script and moves in that directions based on its moveSpeed value
    protected virtual void UpdateMovement()
    {
        ReceiveDirection();
        transform.position = (transform.position + (motion * Time.fixedDeltaTime));
    }

    protected virtual void ReceiveDirection()
    {
        if (inputDir == Vector2.zero) { motion = Vector2.zero; return; }
        motion = transform.TransformDirection(inputDir) * moveSpeed;
        inputDir = Vector2.zero;
    }
    #endregion

    #region Handle Attacked

    //  IdamageReceiver method, needs a damage source as a parameter
    //  takes damage, shakes, and plays the damage particle effect
    //  check if the damage source targeting an enemy, otherwise does nothing
    public void Attacked(DamageSource d)
    {
        if(d.DamageTarget != DamageSource.DamageType.Enemy) { return; }

        TakeDamage(d.DamageVal);
        
        //originalPosition = transform.position;
        originalPosition = transform.position;

        StartCoroutine(Shake());
    }

    private void TakeDamage(float damage)
    {
        health -= damage;
        comicHurt.Play();
        if (health <= 0)
        {
            StartCoroutine(StartDeath());
        }
    }


    //  Death Coroutine 
    //  - sets ai state to death, turns off colldiers on enemy, removes it from the attack queue
    //  - plays comic death and death animation if they aren't null
    //  - after a second the gameobject is destroyed
    // NOTE: this coroutine does nto actual delete the game object, that is dealt with in the enemy ai 
    private IEnumerator StartDeath()
    {
        ai.RemoveFromAttackQueue();
        ai.behaviour = EnemyAI.Behaviour.Dead;
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D c in colliders) { c.enabled = false; }
        attackNotif.SetActive(false);
        

        if (bugGoo != null)
        {
            Debug.Log("fuckle");
            ParticleSystem bugDeath = Instantiate(bugGoo, transform);
            bugDeath.transform.parent = null;
            bugDeath.Play();
            Destroy(bugDeath, 3);
        }

        if (animator != null) //if enemy has animations, play them before triggering death
        {
            animator.SetBool("Death", true); // couldn't this just be done with a trigger instead of a bool?
            yield return new WaitForSeconds(1);
            animator.SetBool("Death", false);
        }

    }

    //  shakes the parent of the sprite object
    //  please put the animated sprite under a blank game object 
    //  otherwise it shakes the root and messes with the colliders as well
    IEnumerator Shake()
    {
        float elapsed = 0f;
        
        GameObject spritePos = sprite.transform.parent.gameObject;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeStrength;
            float y = Random.Range(-1f, 1f) * shakeStrength;

            spritePos.transform.position = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        spritePos.transform.position = originalPosition;
    }
    #endregion


}

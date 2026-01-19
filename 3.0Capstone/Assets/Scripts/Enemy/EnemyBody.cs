using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

//[RequireComponent(typeof(EnemyAI))]
//[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(DamageSource))]

public class EnemyBody : MonoBehaviour, IDamageReceiver
{
    [Header("Required Components")]
    [SerializeField] protected EnemyAI ai;
    [SerializeField] protected DamageSource damageSource;
    [SerializeField] protected GameObject sprite;
    [SerializeField] protected Animator animator;
    [SerializeField] protected GameObject attackNotif;

    [SerializeField] private ParticleSystem comicHurt;
    [SerializeField] private ParticleSystem comicDeath;


    public enum EnemyMoveType
    {
        NUll = 0,
        Crawler = 1,
        Flyer = 2,
        Hopper = 3,
    }

    public enum EnemyAttackType
    {
        NULL = 0,
        Melee = 1,
        Ranged = 2,
    }

    //[Header("Enemy Type")]
    //protected EnemyMoveType moveType;
    //protected EnemyAttackType attackType;


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

    [Header("Animator")]
    [SerializeField] private Animator enemyAnims;

    

    protected virtual void Awake()
    {
        
        attackNotif.SetActive(false);
        attackTimer = 0;
        
        if(damageSource == null)
        {
            damageSource = GetComponent<DamageSource>();
        }
        if(animator == null && sprite != null)
        {
            animator = sprite.GetComponent<Animator>();
        }
        else { Debug.LogError("Animator not set in code becuase Sprite was not set manually"); }
        
    }

    protected virtual void FixedUpdate()
    {
        UpdateCooldown();
    }

    private void UpdateCooldown()
    {
        if(ai.behaviour != EnemyAI.Behaviour.Attacking && ai.behaviour != EnemyAI.Behaviour.CoolDown) { return; }
        if(attackTimer >= attackStartUp || attackTimer >= attackCoolDown) { return; }
        //if (ai.agent != null && !ai.agent.CanDealDamage) { return; }

        attackTimer += 1 * Time.fixedDeltaTime;
        if(attackNotif.activeSelf == false  && attackTimer >= AttackStartUp/2) { attackNotif.SetActive(true); }
          
    }

    public virtual void Attack(GameObject target) 
    {
        attackTimer = 0;
        attackNotif.SetActive(false);
        
        ai.behaviour = EnemyAI.Behaviour.CoolDown;
    }
    

    

    #region Handle Attacked
    public void Attacked(DamageSource d)
    {
        if(d.DamageTarget == DamageSource.DamageType.Enemy)
        {
            TakeDamage(d.DamageVal);
            comicHurt.Play();
            //originalPosition = transform.position;
            originalPosition = transform.position;
            
            StartCoroutine(Shake());
        }
        
    }

    private void TakeDamage(float damage)
    {
        
        health -= damage;
        if(health <= 0)
        {
            StartCoroutine(Death());
        }
    }

    private IEnumerator Death()
    {
        ai.RemoveFromAttackQueue();
        ai.behaviour = EnemyAI.Behaviour.Dead;
        Collider2D[] colliders = GetComponents<Collider2D>();
        foreach (Collider2D c in colliders) { c.enabled = false; }
        attackNotif.SetActive(false);


        if (comicDeath != null)
        {
            ParticleSystem comicDeath2 = Instantiate(comicDeath);
            comicDeath2.transform.parent = null;
            comicDeath2.Play();
            Destroy(comicDeath2, 5);
        }

        if (enemyAnims != null) //if enemy has animations, play them before triggering death
        {
            enemyAnims.SetBool("Death", true); //MAKE DEATH ANIM PARAMETER THE SAME NAME FOR ALL ENEMIES
            yield return new WaitForSeconds(1);
            enemyAnims.SetBool("Death", false);
        }
        
        
        GameManager.Instance.AddKills(1);
        Destroy(this.gameObject);
    }
    
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

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
    [SerializeField] protected SpriteRenderer sprite;
    

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

    

    

    

    [Header("Combat Stats")]
    [SerializeField] protected float health = 100;
    public float Health {  get { return health; } set {  health = value; } }

    [SerializeField] protected float attackRate = 3;
    public float AttackRate { get {  return attackRate; } set { attackRate = value; } }

    protected float attackCooldown = 3;
    public float AttackCooldown { get { return attackCooldown; } set { attackCooldown = value; } }

    

    

    private void Start()
    {
        attackCooldown = attackRate;
        if(ai == null)
        {
            ai = GetComponent<EnemyAI>();
        }
        if(damageSource == null)
        {
            damageSource = GetComponent<DamageSource>();
        }
        if(sprite == null)
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
            
        }
        
    }

    protected void UpdateCooldown()
    {
        if(attackCooldown >= attackRate || ai.behaviour != EnemyAI.Behaviour.Attack) { return; }

        attackCooldown += 1 * Time.fixedDeltaTime;
        
    }

    

    #region Handle Attacked
    public void Attacked(DamageSource d)
    {
        if(d.DamageTarget == DamageSource.DamageType.Enemy)
        {
            TakeDamage(d.DamageVal);
        }
        
    }

    private void TakeDamage(float damage)
    {
        
        health -= damage;
        if(health <= 0)
        {
            Death();
        }
    }

    private void Death()
    {
        this.gameObject.SetActive(false);
    }

    #endregion




}

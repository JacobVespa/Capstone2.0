using System.Collections;
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
            originalPosition = transform.localPosition;
            StartCoroutine(Shake());
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

    public float shakeDuration = 0.3f;   // how long the shake lasts
    public float shakeStrength = 0.1f;    // how strong the shake is
    private Vector3 originalPosition;
    
    IEnumerator Shake()
    {
        float elapsed = 0f;

        while (elapsed < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeStrength;
            float y = Random.Range(-1f, 1f) * shakeStrength;

            transform.localPosition = originalPosition + new Vector3(x, y, 0);

            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPosition;
    }
    #endregion




}

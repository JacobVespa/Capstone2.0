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

    [SerializeField] protected float attackRate = 3;
    public float AttackRate { get {  return attackRate; } set { attackRate = value; } }

    protected float attackCooldown = 0;
    public float AttackCooldown { get { return attackCooldown; } set { attackCooldown = value; } }

    

    

    protected virtual void Awake()
    {
        
        attackNotif.SetActive(false);
        attackCooldown = 0;
        
        if(damageSource == null)
        {
            damageSource = GetComponent<DamageSource>();
        }
        
        
    }

    protected virtual void FixedUpdate()
    {
        UpdateCooldown();
    }

    private void UpdateCooldown()
    {
        if(attackCooldown >= attackRate || ai.behaviour != EnemyAI.Behaviour.Attack) { return; }

        attackCooldown += 1 * Time.fixedDeltaTime;
        if(attackNotif.activeSelf == false  && attackCooldown >= attackRate/2) { attackNotif.SetActive(true); }
          
    }

    public virtual void Attack(GameObject target) 
    {
        attackCooldown = 0;
        attackNotif.SetActive(false);
    }
    

    

    #region Handle Attacked
    public void Attacked(DamageSource d)
    {
        if(d.DamageTarget == DamageSource.DamageType.Enemy)
        {
            TakeDamage(d.DamageVal);
            comicHurt.Play();
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
        
        ai.behaviour = EnemyAI.Behaviour.Dead;
        sprite.SetActive(false);
        Collider2D[] colliders = GetComponents<Collider2D>();
        attackNotif.SetActive(false);

        foreach(Collider2D c in colliders) { c.enabled = false; }

        ParticleSystem comicDeath2 = Instantiate(comicDeath); 
        comicDeath2.transform.parent = null;
        comicDeath2.Play();
        Destroy(comicDeath2, 5);
        //this.gameObject.SetActive(false);
        StartCoroutine(DestroyObject());
    }
    
    protected virtual IEnumerator DestroyObject()
    {

        

        

        yield return null;
    }



    
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

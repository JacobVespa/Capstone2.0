using UnityEngine;
using UnityEngine.UIElements;

[RequireComponent(typeof(EnemyAI))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(DamageSource))]

public class EnemyBody : MonoBehaviour, IDamageReceiver
{
    [Header("Required Components")]
    [SerializeField] private EnemyAI ai;
    [SerializeField] private CharacterController bodyController;
    [SerializeField] private DamageSource damageSource;
    [SerializeField] private SpriteRenderer sprite;
    [SerializeField] private Camera sceneCamera;
     

    [Header("Movement Stats")]
    [SerializeField] private float moveSpeed = 5;
    [SerializeField] private float turnSpeed = 5;
    [SerializeField] private float gravity = 9.8f;

    private Vector3 motion = Vector3.zero;

    private Vector3 inputDir = Vector3.zero;
    public Vector3 InputDir {  get { return inputDir; } set {  inputDir = value; } }

    

    private Vector3 viewDir = Vector3.zero;
    public Vector3 ViewDir { get { return viewDir; } set {  viewDir = value; } }

    [Header("Combat Stats")]
    [SerializeField] private float health = 100;
    public float Health {  get { return health; } set {  health = value; } }

    [SerializeField] private float attackRate = 0.75f;
    public float AttackRate { get {  return attackRate; } set { attackRate = value; } }

    private float attackCooldown = 0.75f;

    [SerializeField] private float attackRange = 2f;
    public float AttackRange { get {  return attackRange; } set {  attackRange = value; } }

    

    private void Start()
    {
        attackCooldown = attackRate;
        if(ai == null)
        {
            ai = GetComponent<EnemyAI>();
        }
        if(bodyController == null)
        {
            bodyController = GetComponent<CharacterController>();
        }
        if(damageSource == null)
        {
            damageSource = GetComponent<DamageSource>();
        }
        if(sprite == null)
        {
            sprite = GetComponentInChildren<SpriteRenderer>();
            Debug.Log(sprite.gameObject.name);
        }
        
    }

    private void UpdateCooldown()
    {
        if(attackCooldown >= attackRate) { return; }

        attackCooldown += 1 * Time.fixedDeltaTime;
        
    }

    public void Attack(Vector3 dir)
    {
        if (attackCooldown < attackRate) { return; }
        
        Ray r = new Ray(gameObject.transform.position, dir);
        Debug.DrawRay(gameObject.transform.position, dir * attackRange, Color.blue);
        if (Physics.Raycast(r, out RaycastHit hitInfo, attackRange))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out IDamageReceiver damageTarget))
            {
                damageTarget.Attacked(damageSource);
            }

        }
        attackCooldown = 0;
    }

    private void Update()
    {
        
    }

    private void FixedUpdate()
    {
        UpdateMovemnet();
        UpdateCooldown();
    }

    private void UpdateMovemnet()
    {
        
        HandleMovement();
        //HandleGravity();
        bodyController.Move(motion * Time.fixedDeltaTime);
    }

    private void HandleMovement()
    {
        if(inputDir == Vector3.zero) { motion = Vector3.zero;  return; }
        
        motion = transform.TransformDirection(inputDir) * moveSpeed;
        inputDir = Vector3.zero;
        
    }

    private void HandleGravity()
    {
        if (bodyController.isGrounded) { motion.y = -1; }
        else if (!bodyController.isGrounded) {  motion.y  -= gravity; }
    }

    


    #region Handle Attacked
    public void Attacked(DamageSource d)
    {
        TakeDamage(d.DamageVal);
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




    private void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent(out DamageSource d))
        {
            if(d.DamageTarget == DamageSource.DamageType.Enemy)
            {
                Attacked(d);
            }
            else if(d.DamageTarget != DamageSource.DamageType.Enemy && other.gameObject != this.gameObject)
            {
                
                Debug.LogError("DamageTarget is set to the wrong value to damage this");
            }
        }

        
    }

    
    
}

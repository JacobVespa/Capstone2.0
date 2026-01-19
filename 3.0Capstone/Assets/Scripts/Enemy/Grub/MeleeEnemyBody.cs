using System.Collections;
using UnityEditor.PackageManager;
using UnityEngine;

public class MeleeEnemyBody : EnemyBody
{

    [SerializeField] private CircleCollider2D attackRange;
    [SerializeField] private float attackRangeVal = 2;
    public float AttackRangeVal { get { return attackRangeVal; } set {  attackRangeVal = value; attackRange.radius = attackRangeVal; } }


    [Header("Movement Stats")]
    [SerializeField] private float moveSpeed = 5;

    private Animator grubAnims;
    
    

    private Vector3 motion = Vector2.zero;

    private Vector2 inputDir = Vector2.zero;
    public Vector2 InputDir { get { return inputDir; } set { inputDir = value; } }

    protected override void Awake()
    {
        base.Awake();
        attackRange.radius = attackRangeVal;
        grubAnims = sprite.GetComponent<Animator>();
    }

    

    public override void Attack(GameObject target)
    {
        if (attackCooldown < attackRate) { return; }


        base.Attack(target);
        StartCoroutine(GrubAttack());

        if (target.TryGetComponent<IDamageReceiver>(out IDamageReceiver damageTarget))
        {
            damageTarget.Attacked(damageSource);
        }
        else{ Debug.LogError("no IDamage Receiver found on target");}

    }

    IEnumerator GrubAttack()
    {
        if(grubAnims != null)
        {
            grubAnims.SetBool("Attack", true);
            yield return new WaitForSeconds(0.25f); //can adjust the time on this
            grubAnims.SetBool("Attack", false);
        }
        
    }

    

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        UpdateMovemnet();
    }

    private void UpdateMovemnet()
    {
        HandleMovement();
        transform.position = (transform.position + (motion * Time.fixedDeltaTime));
    }

    private void HandleMovement()
    {
        if (inputDir == Vector2.zero) { motion = Vector2.zero; return; }

        motion = transform.TransformDirection(inputDir) * moveSpeed;
        inputDir = Vector2.zero;

    }
    

}

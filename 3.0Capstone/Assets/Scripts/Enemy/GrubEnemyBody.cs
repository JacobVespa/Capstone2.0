using UnityEditor.PackageManager;
using UnityEngine;

public class GrubEnemyBody : EnemyBody
{

    
    [SerializeField] private float attackRangeVal = 2;
    public float AttackRange { get { return attackRangeVal; } set {  attackRangeVal = value; } }
    

    [Header("Movemnet Stats")]
    [SerializeField] private float moveSpeed = 5;
    
    

    private Vector3 motion = Vector2.zero;

    private Vector2 inputDir = Vector2.zero;
    public Vector2 InputDir { get { return inputDir; } set { inputDir = value; } }

    

    public void Attack(Vector2 dir)
    {
        //Debug.Log("attack");
        
        if (attackCooldown < attackRate) { return; }

        Ray2D r = new Ray2D(gameObject.transform.position, dir * 3f);
        Debug.DrawRay(gameObject.transform.position, dir * 3f, Color.red);
        RaycastHit2D hit = Physics2D.Raycast(gameObject.transform.position, dir, 100f,1);
        if (hit)
        {
            Debug.Log(hit.collider.name);
        }


    }

    

    private void FixedUpdate()
    {
        UpdateMovemnet();
        UpdateCooldown();
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

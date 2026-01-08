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

    

    public void Attack(Vector3 dir)
    {
        Debug.Log("attack");
        Ray2D r = new Ray2D(gameObject.transform.position, new Vector2(1, 1));
        Debug.DrawRay(gameObject.transform.position, new Vector2(1, 1), Color.red);
        if (attackCooldown < attackRate) { return; }

        

        
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

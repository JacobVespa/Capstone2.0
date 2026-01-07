using UnityEditor.PackageManager;
using UnityEngine;

public class GrubEnemyBody : EnemyBody
{
    

    [SerializeField] private float attackRange;
    public float AttackRange { get { return attackRange; } set {  attackRange = value; } }
    

    [Header("Movemnet Stats")]
    [SerializeField] private float moveSpeed = 5;
    
    

    private Vector2 motion = Vector2.zero;

    private Vector2 inputDir = Vector2.zero;
    public Vector2 InputDir { get { return inputDir; } set { inputDir = value; } }


    public void Attack(Vector3 dir)
    {
        
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
        bodyController.Move(motion * Time.fixedDeltaTime);
    }

    private void HandleMovement()
    {
        if (inputDir == Vector2.zero) { motion = Vector2.zero; return; }

        motion = transform.TransformDirection(inputDir) * moveSpeed;
        inputDir = Vector2.zero;

    }



}

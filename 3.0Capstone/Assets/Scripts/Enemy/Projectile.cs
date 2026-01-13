using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private DamageSource damageSource;
    public DamageSource DamageSource { get { return damageSource; } set {  damageSource = value; } }

    [Header("Projectile Stats")]
    [SerializeField] private float lifetime = 5f;
    public float Lifetime { get { return lifetime; } set {  lifetime = value; } }
    /*
    [SerializeField] private float speed;
    public float Speed { get { return speed; } set { speed = value; } }
    private Vector2 direction;
    public Vector2 Direction { get { return direction; } set { direction = value; } }
    */
    private float spawnTime;

    private void Awake()
    {
        this.spawnTime = Time.time;

        if(body == null) { body = GetComponent<Rigidbody2D>(); }
        
    }

    private void FixedUpdate()
    {
        body.AddForce(Vector2.down * 1);
    }

    public void Fire(float speed, Vector2 Dir)
    {
        transform.rotation = Quaternion.LookRotation(Dir);
        Debug.Log("fire");
        
        
    }
}
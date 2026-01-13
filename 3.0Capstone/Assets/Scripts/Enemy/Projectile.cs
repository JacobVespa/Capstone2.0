using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class Projectile : MonoBehaviour
{
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private DamageSource damageSource;
    public DamageSource DamageSource { get { return damageSource; } set {  damageSource = value; } }

    [Header("Projectile Stats")]
    public bool active = false;
    [SerializeField] private float lifetime = 5f;
    public float Lifetime { get { return lifetime; } set {  lifetime = value; } }
    
    [SerializeField] private float speed;
    public float Speed { get { return speed; } set { speed = value; } }
    private Vector2 direction;
    public Vector2 Direction { get { return direction; } set { direction = value; } }
    
    private float spawnTime;

    private void Awake()
    {
        this.spawnTime = Time.time;

        if(body == null) { body = GetComponent<Rigidbody2D>(); }
        
    }

    private void FixedUpdate()
    {
        Debug.Log(Time.time - spawnTime);
        if(Time.time - spawnTime >= lifetime) { DisableProjectile(); }
        

        body.AddForce(direction * speed);
    }

    public void Fire(float spd, Vector2 Dir)
    {
        gameObject.SetActive(true);
        active = true;

        speed = spd;
        direction = Dir;

        Debug.Log(speed);
        Debug.Log(direction);

        float rotatation = Mathf.Atan2(direction.y,direction.x) * Mathf.Rad2Deg;

        float offset = 90;

        transform.rotation = Quaternion.Euler(Vector3.forward * (rotatation + offset));

        Debug.Log("fire");
    }

    private void DisableProjectile()
    {
        gameObject.SetActive(false);
        active = false;
        speed = 0f;
        direction = Vector2.zero;
        this.spawnTime = Time.time;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        DisableProjectile();
        if (collision.gameObject.TryGetComponent<IDamageReceiver>(out IDamageReceiver dr)) { dr.Attacked(damageSource); }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        DisableProjectile();

        if(collision.gameObject.TryGetComponent<IDamageReceiver>(out IDamageReceiver dr)) { dr.Attacked(damageSource); }
        
    }
}
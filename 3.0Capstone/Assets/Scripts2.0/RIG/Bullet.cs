using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject turretObject;
    private Turret turret;

    private void Start()
    {
        turretObject = GameObject.FindGameObjectWithTag("Turret");
        turret = turretObject.GetComponent<Turret>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            //Debug.Log("Hit Enemy with a bullet");
            turret.HitEnemy(collision);
            Destroy(gameObject);
        }
        if(collision.CompareTag("Gem"))
        {
            turret.HitGem(collision);
            Destroy(gameObject);
        }
        if (collision.CompareTag("Projectile"))
        {
            turret.HitProjectile(collision);
            Destroy(gameObject);
        }
        if (collision.CompareTag("Grass"))
        {
            turret.HitGrass(collision);
            Destroy(gameObject);
        }
    }
}

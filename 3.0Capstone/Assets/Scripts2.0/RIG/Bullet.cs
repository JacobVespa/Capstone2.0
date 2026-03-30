using System.Collections;
using UnityEditor;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private GameObject turretObject;
    private Turret turret;
    private ParticleSystem sparks;

    private void Start()
    {
        turretObject = GameObject.FindGameObjectWithTag("Turret");
        turret = turretObject.GetComponent<Turret>();
        sparks = GetComponentInChildren<ParticleSystem>();
    }

    private void FixedUpdate()
    {
        if (!CheckInView())
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Enemy"))
        {
            //Debug.LogError("Hit Enemy with a bullet");
            turret.HitEnemy(collision);
            StartCoroutine(DestroyBullet());
        }
        if(collision.CompareTag("Gem"))
        {
            turret.HitGem(collision);
            StartCoroutine(DestroyBullet());
        }
        if (collision.CompareTag("DefenseGem"))
        {
            Debug.Log("BIG GEM HIT!!!");
            turret.HitDefenseGem(collision);
            StartCoroutine(DestroyBullet());
        }
        if (collision.CompareTag("Projectile"))
        {
            turret.HitProjectile(collision);
            StartCoroutine(DestroyBullet());
        }
        if (collision.CompareTag("Grass"))
        {
            //Debug.Log("BRUH");
            turret.HitGrass(collision);
            //Destroy(gameObject);
        }
    }

    public bool CheckInView()
    {
        Vector3 vPos = Camera.main.WorldToViewportPoint(transform.position);

        if (vPos.x < 0.99 && vPos.x > 0.01 && vPos.y < 0.99 && vPos.y > 0.01)
        {
            return true;
        }

        else { return false; }
    }

    IEnumerator DestroyBullet()
    {
        sparks.Play();
        yield return new WaitForSeconds(0.05f);
        Destroy(gameObject);
    }
}

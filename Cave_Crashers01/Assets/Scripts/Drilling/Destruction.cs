using System;
using System.Collections;
using UnityEngine;

public class Destruction : MonoBehaviour
{

    private Rigidbody rb;
    [SerializeField] private GameObject brokenPrefab;
    [SerializeField] private float explosionPower = 1000;
    [SerializeField] private float explosionRadius = 2f;
    [SerializeField] private float pieceFadeSpeed = 0.25f;
    [SerializeField] private float pieceDestroyDelay = 5f;
    [SerializeField] private float pieceSleepCheckDelay = 0.5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Explode()
    {
        if(rb != null)
        {
            Destroy(rb);
        }

        if(TryGetComponent<Collider>(out Collider collider))
        {
            collider.enabled = false;
        }
        if (TryGetComponent<Renderer>(out Renderer rend))
        {
            rend.enabled = false;
        }

        GameObject brokenInstance = Instantiate(brokenPrefab, transform.position, transform.rotation);

        Rigidbody[] rigidbodies = brokenInstance.GetComponentsInChildren<Rigidbody>();

        foreach(Rigidbody r in rigidbodies)
        {
            if(rb != null)
            {
                r.linearVelocity = rb.linearVelocity;
            }

            r.AddExplosionForce(explosionPower, transform.position, explosionRadius);
        }

        StartCoroutine(FadeOutRigidBodies(rigidbodies));

    }

    private IEnumerator FadeOutRigidBodies(Rigidbody[] rigidBodies)
    {
        WaitForSeconds wait = new WaitForSeconds(pieceSleepCheckDelay);
        int activeRigidBodies = rigidBodies.Length;

        while(activeRigidBodies > 0)
        {
            yield return wait;

            foreach(Rigidbody r in rigidBodies)
            {
                if(r.IsSleeping())
                {
                    activeRigidBodies--;
                }
            }
        }

        yield return new WaitForSeconds(pieceDestroyDelay);

        float time = 0;
        Renderer[] rends = Array.ConvertAll(rigidBodies, GetRendererFromRigidBody);

        foreach(Rigidbody r in rigidBodies)
        {
            Destroy(r.GetComponent<Collider>());
            Destroy(r);
        }

        while(time < 1)
        {
            float step = Time.deltaTime * pieceFadeSpeed;
            foreach(Renderer rend in rends)
            {
                rend.transform.Translate(Vector3.down * (step / rend.bounds.size.y), Space.World);
            }

            time += step;
            yield return null;
        }

        foreach(Renderer rend in rends)
        {
            Destroy(rend.gameObject);
        }

        Destroy(gameObject);

    }

    private Renderer GetRendererFromRigidBody(Rigidbody rigid)
    {
        return rigid.GetComponent<Renderer>();
    }

}

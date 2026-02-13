using System;
using System.Runtime.CompilerServices;
using UnityEngine;

public class RigDrill : MonoBehaviour
{
    public float knockbackForce = 5f;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("AHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH");
            Rigidbody2D rb = other.gameObject.GetComponent<Rigidbody2D>();
            Vector2 difference = (transform.position - other.transform.position).normalized;
            Debug.Log("difference Vector: " + difference);
            Vector2 force = difference * knockbackForce;
            rb.AddForce(force, ForceMode2D.Impulse);
        }
    }
}

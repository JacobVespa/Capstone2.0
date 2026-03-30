using UnityEngine;
using YourNamespace;

public class AaronsBFScript : MonoBehaviour
{
    [SerializeField] private Animator bfAnimator;
    [SerializeField] private MeleeEnemyBody bfEnemyBody;

    private float currentHealth;



    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        currentHealth = bfEnemyBody.Health;
    }

    // Update is called once per frame
    void Update()
    {
        if (bfEnemyBody.Health <= bfEnemyBody.Health / 3)
        {
            bfAnimator.SetTrigger("LoseArmor");
        }

        if (bfEnemyBody.Health < currentHealth && bfEnemyBody.Health > bfEnemyBody.Health / 3)
        {
            currentHealth = bfEnemyBody.Health;
            bfAnimator.SetTrigger("Hit");
           
        }
    }
}

using System.Collections;
using UnityEngine;
using YourNamespace;

public class ThumperEnemyBody : MeleeEnemyBody
{
    protected float maxHealth;

    private bool sheild = false;
    private bool loseArmour = false;

    private void Start()
    {
        if(DifficultyManager.Instance != null)
        {
            Debug.Log(DifficultyManager.Instance.BottomFeederHealth);
            this.health = DifficultyManager.Instance.BottomFeederHealth;
        }
        maxHealth = Health;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        
        if (sheild && animator.speed != 0)
        {
            AnimatorStateInfo state = animator.GetCurrentAnimatorStateInfo(0);
            if(state.normalizedTime >= 0.15 && state.IsName("BFHit"))
            {
                Debug.LogError("trig");
                animator.speed = 0;
                moveSpeed = 0;
            }
            
        }
    }

    public override void Attacked(DamageSource d)
    {
        base.Attacked(d);
        if(loseArmour == false)
        {
            animator.SetBool("Hit",true);
            sheild = true;
            
        }
        
    }

    protected override void TakeDamage(float damage)
    {
        if(sheild == true)
        {
            damage /= 2;
        }

        base.TakeDamage(damage);

        if (health <= maxHealth / 3 && loseArmour == false)
        {
            animator.SetBool("Hit", false);
            animator.SetTrigger("LoseArmor");
            animator.speed = 1;
            
            loseArmour = true;
            sheild = false;
        }
    }

    private IEnumerator hit()
    {


        yield return new WaitForSeconds(2.5f);

        animator.SetBool("Hit", false);
        animator.speed = 1;


    }
}

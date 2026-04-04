using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using YourNamespace;

public class ThumperEnemyBody : MeleeEnemyBody
{
    protected float maxHealth;
    

    private bool hit = false;
    public bool shield = false;
    private bool loseArmour = false;

    private AnimatorStateInfo state;

    private Coroutine isShielding;
    

    private void Start()
    {
        if(DifficultyManager.Instance != null)
        {
            
            this.health = DifficultyManager.Instance.BottomFeederHealth;
        }
        maxHealth = Health;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();

       

        if (hit && !shield && !loseArmour)
        {
            state = animator.GetCurrentAnimatorStateInfo(0);

            

            if (state.normalizedTime%1.0 >= 0.25 && state.IsTag("Hit"))
            {
                isShielding = StartCoroutine(Shielding());
            }
        }
 
    }

    public override void Attack(GameObject target)
    {
        if (shield) { return; }

        base.Attack(target);
    }

    public override void Attacked(DamageSource d)
    {
        base.Attacked(d);
        if(loseArmour == false && ai.CheckInView(0.85f))
        {
            animator.SetBool("Hit",true);
            hit = true;
            
        }
        
    }

    protected override void TakeDamage(float damage)
    {
        if(shield == true)
        {
            damage /= 2;
        }

        base.TakeDamage(damage);

        
        if (health <= maxHealth / 3 && loseArmour == false)
        {
            

            if (shield)
            {
                StopCoroutine(isShielding);
                hit = false;
                animator.speed = 1;
                shield = false;
                animator.SetBool("Hit", false);
                
            }
            animator.SetTrigger("LoseArmor");
            
            loseArmour = true;
            StartCoroutine(LoseArmour());

            
        }
        
    }

    private IEnumerator Shielding()
    {
        hit = false;
        shield = true;
        animator.speed = 0;
        float regularSpeed = moveSpeed;
        moveSpeed = 0;

        while (true)
        {
            yield return new WaitForSeconds(1f);

            if(hit == false || loseArmour)
            {
                break;
            }
            else { hit = false; }
        }

        

        
        animator.speed = 1;

        
        
        animator.SetBool("Hit", false);

        while (true)
        {
            yield return new WaitForEndOfFrame();
            state = animator.GetCurrentAnimatorStateInfo(0);

            if(!state.IsTag("Hit") || loseArmour)
            {
                break;
            }
        }
        
        moveSpeed = regularSpeed;
        shield = false;

    }

    IEnumerator LoseArmour()
    {


        while (true)
        {
            yield return new WaitForEndOfFrame();
            state = animator.GetCurrentAnimatorStateInfo(0);
            /*
            if (state.IsTag("LoseArmour"))
            {
                Debug.Log(state.normalizedTime);
            }
            */
            
            if (state.IsTag("LoseArmour") && state.normalizedTime >= 0.95)
            {
                //Debug.Log("start");
                break;
            }
        }
        moveSpeed = 5;
        
    }
}

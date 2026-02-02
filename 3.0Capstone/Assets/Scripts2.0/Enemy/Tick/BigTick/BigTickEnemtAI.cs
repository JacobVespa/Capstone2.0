using Unity.VisualScripting;
using UnityEngine;

public class BigTickEnemtAI : TickEnemyAI
{


    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.layer == 8)
        {
            Debug.Log("player");
        }
    }
}

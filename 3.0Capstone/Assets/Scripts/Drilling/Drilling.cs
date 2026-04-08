using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Drilling : MonoBehaviour
{

    private GameObject driller;
    bool isDrilling = false;
    float drillTime = 0f;
    float drillDurationMax = 3f;
    Destruction currentTarget;

    //list of gems (MOVE LOCATION OF THIS LATER)
    List<Gem> gems = new List<Gem>();

    private void Start()
    {
        driller = gameObject;
        Debug.Log("The current drill target is: " + currentTarget); 
    }

    // Update is called once per frame
    void Update()
    {
        //check if player is holding mouse button down
        if (Input.GetMouseButton(0))
        {
            //rotate the drill and set isDrilling to true
            StartCoroutine(rotateDrill());
            isDrilling = true;

            //if the target isn't null, increase the timer, then if past drill duration time, explode object
            if(currentTarget != null && isDrilling)
            {
                drillTime += Time.deltaTime;
                if (drillTime > drillDurationMax)
                {
                    currentTarget.Explode();
                    currentTarget = null;
                    drillTime = 0f;
                    Debug.Log("Current target is: " + currentTarget);
                }
            }
        }
    }

    //if drill hits a wall, set the current target to that piece of wall, if it hits a gem, collect it (IMPLEMENT DAMAGING GEM LATER)
    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Wall"))
        {
            currentTarget = collision.gameObject.GetComponent<Destruction>();
            Debug.Log("Drill is against the wall");
        }
        //TESTING for collecting gem
        if(collision.gameObject.CompareTag("Gem"))
        {
            collision.gameObject.SetActive(false);
            //adding to gems list
            Gem collected = new Gem(collision.gameObject.GetComponent<Gem>().gemName, collision.gameObject.GetComponent<Gem>().gemSize); //THIS IS A MESS
            gems.Add(collected);
            Debug.Log("Gem collected! Gem Name: " + collected.gemName + "\tGem Size: " + collected.gemSize);
        }
    }

    //if drill leaves a wall, set the current target back to null
    private void OnCollisionExit(Collision collision)
    {
        if(collision.gameObject.CompareTag("Wall"))
        {
            currentTarget = null;
            drillTime = 0f;
        }
    }

    //if you destroy a piece of wall bu the drill is still colliding with another piece of wall
    private void OnCollisionStay(Collision collision)
    {
        if(currentTarget == null && collision.gameObject.CompareTag("Wall"))
        {
            currentTarget = collision.gameObject.GetComponent<Destruction>();
            Debug.Log("Drill is against the wall");
        }
    }

    //rotating the drill
    IEnumerator rotateDrill()
    {
        driller.transform.Rotate(Vector3.right, 10);
        yield return new WaitForSeconds(0.2f);
    }

}

using System.Runtime.CompilerServices;
using UnityEngine;

public class DrillController : MonoBehaviour
{

    [SerializeField] private float moveSpeed;

    //drill script reference
    private Drilling drilling;

    //drill camera
    [SerializeField] private GameObject drillCamera;

    private void Start()
    {
        //getting the drilling script and camera objects and disabling them
        drilling = GetComponentInChildren<Drilling>();
        drilling.enabled = false;
        drillCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        //DO NOT NEED DRILL MOVEMENT, WAS JUST FOR TESTING
        //MoveDrill();

        //if interaction to start drill station occurs, call it here
    }

    private void MoveDrill()
    {
        //move forward
        if (Input.GetKey(KeyCode.W))
        {
            gameObject.transform.Translate(Vector3.right * moveSpeed * Time.deltaTime);
        }
        //move back
        if (Input.GetKey(KeyCode.S))
        {
            gameObject.transform.Translate(Vector3.left * moveSpeed * Time.deltaTime);
        }
        //move left
        if (Input.GetKey(KeyCode.A))
        {
            gameObject.transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
        }
        //move right
        if (Input.GetKey(KeyCode.D))
        {
            gameObject.transform.Translate(Vector3.back * moveSpeed * Time.deltaTime);
        }
    }

    public void EnableDrill()
    {
        drilling.enabled = true;
        drillCamera.SetActive(true);
    }

}

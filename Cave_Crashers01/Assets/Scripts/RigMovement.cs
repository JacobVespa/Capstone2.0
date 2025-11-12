using UnityEngine;
using UnityEngine.InputSystem;

public class RigMovement : MonoBehaviour
{
    public GameObject player;
    public GameObject pilotSeat;
    public GameObject rig;
    public Rigidbody rigBody;
    public Transform exitTransform;

    private void Awake()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        pilotSeat = GameObject.FindGameObjectWithTag("PilotSeat");
        rig = GameObject.FindGameObjectWithTag("Rig");
        rigBody = rig.GetComponent<Rigidbody>();
        exitTransform = pilotSeat.GetComponentInChildren<Transform>(); //This might be wrong
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  // public void Interact()
  // {
  //     //Set Players Transform to Pilot Seat Transform
  //     player.transform.position = pilotSeat.transform.position; 
  //     //Debug.Log("Here");
  //
  //     //UI Transition?
  //
  //     //Movement Controls
  //     //Get rigidbody of the ship 
  //     //AddForce to the Rig as neccesary
  //
  //     //trying to figure out how to use the new input system with this
  //
  //
  //     
  //
  // }

}

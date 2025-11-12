using UnityEngine;

public class DrillController : MonoBehaviour
{

    [SerializeField] private float moveSpeed;

    // Update is called once per frame
    void Update()
    {
        MoveDrill();
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

}

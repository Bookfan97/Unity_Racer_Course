using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody theRB;
    public float maxSpeed, forwardAcceleration = 8f, reverseAcceleration = 4f;
    private float speedInput;
    // Start is called before the first frame update
    void Start()
    {
        theRB.transform.parent = null;
    }

    // Update is called once per frame
    void Update()
    {
        speedInput = 0;
        if (Input.GetAxis("Vertical") > 0)
        {
            speedInput = Input.GetAxis("Vertical") * forwardAcceleration;
        }
        else if(Input.GetAxis("Vertical") < 0)
        {
            speedInput = Input.GetAxis("Vertical") * reverseAcceleration;
        }
        transform.position = theRB.position;
    }

    private void FixedUpdate()
    {
        theRB.AddForce(new Vector3(0, 0f, speedInput * 1000));
    }
}

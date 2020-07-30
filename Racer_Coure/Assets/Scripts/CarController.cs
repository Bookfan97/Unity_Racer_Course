using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody theRB;
    public float maxSpeed, forwardAcceleration = 8f, reverseAcceleration = 4f, turnStrength = 180f;
    private float speedInput, turnInput;
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

        turnInput = Input.GetAxis("Horizontal");
        if (Input.GetAxis("Vertical") != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength  * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.velocity.magnitude/maxSpeed), 0f));
        }
        transform.position = theRB.position;
    }

    private void FixedUpdate()
    {
        theRB.AddForce(transform.forward * speedInput * 1000f);
        if (theRB.velocity.magnitude > maxSpeed)
        {
            theRB.velocity = theRB.velocity.normalized * maxSpeed;
        }
        Debug.Log(theRB.velocity.magnitude);
    }
}

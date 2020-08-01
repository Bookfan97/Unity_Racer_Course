using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour
{
    public Rigidbody theRB;

    public float maxSpeed,
        forwardAcceleration = 8f,
        reverseAcceleration = 4f,
        turnStrength = 180f,
        groundRayLength = 0.75f,
        gravityMod = 10f,
        maxWheelTurn = 20f,
        maxEmission = 25f,
        emissionFadeSpeed = 20f, 
        skidFadeSpeed = 2f;
    private float speedInput, turnInput, dragOnGround, dragInAir = 0.1f, emissionRate;
    public bool grounded;
    public Transform groundRayPoint, groundRayPoint2, leftFrontWheel, rightFrontWheel, leftBackWheel, rightBackWheel;
    public LayerMask whatIsGround;
    public ParticleSystem[] dustTrail;
    public AudioSource engineSound, skidSound;
    // Start is called before the first frame update
    void Start()
    {
        theRB.transform.parent = null;
        dragOnGround = theRB.drag;
        emissionRate = 25f;
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
        /*if (grounded && Input.GetAxis("Vertical") != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength  * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.velocity.magnitude/maxSpeed), 0f));
        }*/
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
        leftBackWheel.localRotation = Quaternion.Euler(leftBackWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftBackWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.eulerAngles.x, (turnInput * maxWheelTurn), rightFrontWheel.localRotation.eulerAngles.z);
        rightBackWheel.localRotation = Quaternion.Euler(rightBackWheel.eulerAngles.x, (turnInput * maxWheelTurn), rightBackWheel.localRotation.eulerAngles.z);
        //transform.position = theRB.position;
        emissionRate = Mathf.MoveTowards(emissionRate, 0f, emissionFadeSpeed * Time.deltaTime);
        if (grounded && (Mathf.Abs(turnInput) > 0.5f || (theRB.velocity.magnitude < maxSpeed * 0.5f && theRB.velocity.magnitude !=0)))
        {
            emissionRate = maxEmission;
        }

        if (theRB.velocity.magnitude <= 0.5f)
        {
            emissionRate = 0;
        }
        for (int i = 0; i < dustTrail.Length; i++)
        {
            var emissionModule = dustTrail[i].emission;
            emissionModule.rateOverTime = emissionRate;
        }

        if (engineSound != null)
        {
            engineSound.pitch = 1f + ((theRB.velocity.magnitude/maxSpeed) * 2f);
        }

        if (skidSound != null)
        {
            if (Mathf.Abs(turnInput) > 0.5f)
            {
                skidSound.volume = 1f;
            }
            else
            {
                skidSound.volume = Mathf.MoveTowards(skidSound.volume, 0f, skidFadeSpeed * Time.deltaTime);
            }
        }
    }

    private void FixedUpdate()
    {
        grounded = false;
        RaycastHit hit;
        Vector3 normalTarget = Vector3.zero;
        if (Physics.Raycast(groundRayPoint.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;
            normalTarget = hit.normal;
        }

        if (Physics.Raycast(groundRayPoint2.position, -transform.up, out hit, groundRayLength, whatIsGround))
        {
            grounded = true;
            normalTarget = (normalTarget+ hit.normal) / 2f;
        }
        
        if (grounded)
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, normalTarget) * transform.rotation;
        }
        if (grounded)
        {
            theRB.drag = dragOnGround;
            theRB.AddForce(transform.forward * (speedInput * 1000f)); 
        }
        else
        {
            theRB.drag = dragInAir;
            theRB.AddForce(-Vector3.up * (gravityMod * 100f));
        }
        if (theRB.velocity.magnitude > maxSpeed)
        {
            theRB.velocity = theRB.velocity.normalized * maxSpeed;
        }
        //Debug.Log(theRB.velocity.magnitude);
        transform.position = theRB.position;
        if (grounded && Input.GetAxis("Vertical") != 0) 
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength  * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.velocity.magnitude/maxSpeed), 0f)); 
        }
    }
}

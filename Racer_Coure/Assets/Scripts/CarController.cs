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
         maxWheelTurn = 25f,
         maxEmission = 25f,
         emissionFadeSpeed = 20f,
         skidFadeSpeed = 2f,
         lapTime, bestLapTime, totalTime,
         resetCooldown = 2f,
         aiAccelerateSpeed = 1f,
         aiTurnSpeed = .8f,
         aiReachPointRange = 5f,
         aiPointVariance = 3f,
         aiMaxTurn = 15f;
    private float speedInput, turnInput, dragOnGround, dragInAir = 0.1f, emissionRate, aiSpeedInput, aiSpeedMod;
    private bool grounded;
    public Transform groundRayPoint, groundRayPoint2, leftFrontWheel, rightFrontWheel, leftBackWheel, rightBackWheel;
    public LayerMask whatIsGround;
    public ParticleSystem[] dustTrail;
    public AudioSource engineSound, skidSound;
    public int nextCheckpoint, currentLap, currentTarget;
    private float resetCounter;
    public bool isAI;
    private Vector3 targetPoint;

    // Start is called before the first frame update
    private void Start()
    {
        theRB.transform.parent = null;
        dragOnGround = theRB.drag;
        if (isAI)
        {
            targetPoint = RaceManager.instance.Checkpoints[currentTarget].transform.position;
            RandomiseAITarget();

            aiSpeedMod = Random.Range(.8f, 1.1f);
        }
        //UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;
        //UIManager.instance.lapCounterText.text = currentLap.ToString() + "/" + RaceManager.instance.totalLaps.ToString();
        resetCounter = resetCooldown;
    }

    // Update is called once per frame
    private void Update()
    {
        if (!RaceManager.instance.isStarting)
        {
        lapTime += Time.deltaTime;
        if (!isAI)
        {
            var ts = System.TimeSpan.FromSeconds(lapTime);
            UIManager.instance.currentTimeText.text = string.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);
            speedInput = 0f;
            if (Input.GetAxis("Vertical") > 0)
            {
                speedInput = Input.GetAxis("Vertical") * forwardAcceleration;
            }
            else if (Input.GetAxis("Vertical") < 0)
            {
                speedInput = Input.GetAxis("Vertical") * reverseAcceleration;
            }
            if (Input.GetKeyDown(KeyCode.R) && resetCooldown <= 0 )
            {
                ResetToTrack();
            }
            resetCooldown -= Time.deltaTime;
            turnInput = Input.GetAxis("Horizontal");
            if (resetCounter > 0)
            {
                resetCounter -= Time.deltaTime;
            }
        }
        else
        {
            targetPoint.y = transform.position.y;

            if (Vector3.Distance(transform.position, targetPoint) < aiReachPointRange)
            {
                SetNextAITarget();
            }

            Vector3 targetDir = targetPoint - transform.position;
            float angle = Vector3.Angle(targetDir, transform.forward);

            Vector3 localPos = transform.InverseTransformPoint(targetPoint);
            if (localPos.x < 0f)
            {
                angle = -angle;
            }

            turnInput = Mathf.Clamp(angle / aiMaxTurn, -1f, 1f);

            if (Mathf.Abs(angle) < aiMaxTurn)
            {
                aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, 1f, aiAccelerateSpeed);
            }
            else
            {
                aiSpeedInput = Mathf.MoveTowards(aiSpeedInput, aiTurnSpeed, aiAccelerateSpeed);
            }

            speedInput = aiSpeedInput * forwardAcceleration * aiSpeedMod;
        }

        //turning the wheels
        leftFrontWheel.localRotation = Quaternion.Euler(leftFrontWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftFrontWheel.localRotation.eulerAngles.z);
        leftBackWheel.localRotation = Quaternion.Euler(leftBackWheel.localRotation.eulerAngles.x, (turnInput * maxWheelTurn) - 180, leftBackWheel.localRotation.eulerAngles.z);
        rightFrontWheel.localRotation = Quaternion.Euler(rightFrontWheel.eulerAngles.x, (turnInput * maxWheelTurn), rightFrontWheel.localRotation.eulerAngles.z);
        rightBackWheel.localRotation = Quaternion.Euler(rightBackWheel.eulerAngles.x, (turnInput * maxWheelTurn), rightBackWheel.localRotation.eulerAngles.z);
        //transform.position = theRB.position;

        //control particle emissions
        emissionRate = Mathf.MoveTowards(emissionRate, 0f, emissionFadeSpeed * Time.deltaTime);
        if (grounded && (Mathf.Abs(turnInput) > .5f || (theRB.velocity.magnitude < maxSpeed * .5f && theRB.velocity.magnitude != 0)))
        {
            emissionRate = maxEmission;
        }

        if (theRB.velocity.magnitude <= .5f)
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
            engineSound.pitch = 1f + ((theRB.velocity.magnitude / maxSpeed) * 2f);
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
            normalTarget = (normalTarget + hit.normal) / 2f;
        }

        if (grounded)
        {
            transform.rotation = Quaternion.FromToRotation(transform.up, normalTarget) * transform.rotation;
        }
        if (grounded)
        {
            theRB.drag = dragOnGround;
            theRB.AddForce(transform.forward * speedInput * 1000f);
        }
        else
        {
            theRB.drag = .1f;
            theRB.AddForce(-Vector3.up * gravityMod * 100f);
        }

        if (theRB.velocity.magnitude > maxSpeed)
        {
            theRB.velocity = theRB.velocity.normalized * maxSpeed;
        }

        //Debug.Log(theRB.velocity.magnitude);

        transform.position = theRB.position;
        if (grounded && speedInput != 0)
        {
            transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles + new Vector3(0f, turnInput * turnStrength * Time.deltaTime * Mathf.Sign(speedInput) * (theRB.velocity.magnitude / maxSpeed), 0f));
        }
    }

    public void CheckpointHit(int checkpointNumber)
    {
        //Debug.Log(checkpointNumber);
        if (checkpointNumber == nextCheckpoint)
        {
            nextCheckpoint++;
            if (nextCheckpoint == RaceManager.instance.Checkpoints.Length)
            {
                nextCheckpoint = 0;
                LapCompleted();
            }
        }
        if (isAI)
        {
            if (checkpointNumber == currentTarget)
            {
                SetNextAITarget();
            }
        }
    }

    public void SetNextAITarget()
    {
        currentTarget++;
        if (currentTarget >= RaceManager.instance.Checkpoints.Length)
        {
            currentTarget = 0;
        }

        targetPoint = RaceManager.instance.Checkpoints[currentTarget].transform.position;
        RandomiseAITarget();
    }

    public void LapCompleted()
    {
        currentLap++;
        if (lapTime < bestLapTime || bestLapTime == 0)
        {
            bestLapTime = lapTime;
        }

        totalTime += lapTime;
        if (currentLap <= RaceManager.instance.totalLaps)
        {
            lapTime = 0f;

            if (!isAI)
            {
                var ts = System.TimeSpan.FromSeconds(bestLapTime);
                UIManager.instance.bestTimeText.text = string.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);
                if (currentLap == RaceManager.instance.totalLaps)
                {
                    UIManager.instance.lapCounterText.text = "FINAL LAP";
                }
                else
                {
                    UIManager.instance.lapCounterText.text = currentLap + "/" + RaceManager.instance.totalLaps;    
                }
            }
        }
        else
        {
            if (!isAI)
            {
                isAI = true;
                aiSpeedMod = 1f;
                targetPoint = RaceManager.instance.Checkpoints[currentTarget].transform.position;
                RandomiseAITarget();
                var ts = System.TimeSpan.FromSeconds(bestLapTime);
                UIManager.instance.bestTimeText.text = string.Format("{0:00}:{1:00}.{2:000}", ts.Minutes, ts.Seconds, ts.Milliseconds);
                var tt = System.TimeSpan.FromSeconds(totalTime);
                UIManager.instance.totalTimeText.text = string.Format("{0:00}:{1:00}.{2:000}", tt.Minutes, tt.Seconds, tt.Milliseconds);               
                RaceManager.instance.FinishRace();
            }
        }
    }

    public void RandomiseAITarget()
    {
        targetPoint += new Vector3(Random.Range(-aiPointVariance, aiPointVariance), 0f, Random.Range(-aiPointVariance, aiPointVariance));
    }

    private void ResetToTrack()
    {
        int pointToGoTo = nextCheckpoint - 1;
        if (pointToGoTo < 0)
        {
            pointToGoTo = RaceManager.instance.Checkpoints.Length - 1;
        }
        transform.position = RaceManager.instance.Checkpoints[pointToGoTo].transform.position;
        theRB.transform.position = transform.position;
        theRB.velocity = Vector3.zero;
        speedInput = 0f;
        turnInput = 0f;
        resetCounter = resetCooldown;
    }

    //this is just used for recording footage :)
    public void SwitchToAI()
    {
        aiSpeedMod = 1f;
        targetPoint = RaceManager.instance.Checkpoints[currentTarget].transform.position;
        RandomiseAITarget();
    }
}
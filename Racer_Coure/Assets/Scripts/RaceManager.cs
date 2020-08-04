using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;
    public Checkpoint[] Checkpoints;
    public int totalLaps, playerPosition, countdownCurrent = 3, playerStartPosition, numAIToSpawn;
    public CarController playerCar;
    public List<CarController> AICars = new List<CarController>();
    public float timeBetweenPositionCheck = 0.2f, AIDefaultSpeed = 30f, playerDefaultSpeed = 30f, rubberBandSpeedMod = 3.5f, rubberBandAcceleration = 0.5f, timeBetweenStartCount = 1f;
    private float positionCheckCounter, startCounter;
    public bool isStarting;
    public Transform[] startPoints;
    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Checkpoints.Length; i++)
        {
            Checkpoints[i].checkpointNumber = i;
        }
        isStarting = true;
        startCounter = timeBetweenStartCount;
        UIManager.instance.countdownText.text = countdownCurrent.ToString();
    }

    // Update is called once per frame
    void Update()
    {
        if (isStarting)
        {
            startCounter -= Time.deltaTime;
            if (startCounter <= 0)
            {
                countdownCurrent--;
                startCounter = timeBetweenStartCount;
                UIManager.instance.countdownText.text = countdownCurrent.ToString();
                if (countdownCurrent == 0)
                {
                    isStarting = false;
                    UIManager.instance.countdownText.gameObject.SetActive(false);
                    UIManager.instance.GOText.gameObject.SetActive(true);
                }
            }
        }
        else
        {
            positionCheckCounter -= Time.deltaTime;
            if (positionCheckCounter <= 0)
            {
                playerPosition = 1;
                foreach (var AICar in AICars)
                {
                    if (AICar.currentLap > playerCar.currentLap)
                    {
                        playerPosition++;
                    }
                    else if (AICar.currentLap == playerCar.currentLap)
                    {
                        if (AICar.nextCheckpoint > playerCar.nextCheckpoint)
                        {
                            playerPosition++;
                        }
                        else if (AICar.nextCheckpoint == playerCar.nextCheckpoint)
                        {
                            if (Vector3.Distance(AICar.transform.position,
                                Checkpoints[AICar.nextCheckpoint].transform.position) < Vector3.Distance(
                                playerCar.transform.position,
                                Checkpoints[playerCar.nextCheckpoint].transform.position))
                            {
                                playerPosition++;
                            }
                        }
                    }
                }

                positionCheckCounter = timeBetweenPositionCheck;
                UIManager.instance.positionText.text = playerPosition + "/" + (AICars.Count + 1);
            }

            if (playerPosition == 1)
            {
                foreach (var AICar in AICars)
                {
                    AICar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, AIDefaultSpeed + rubberBandSpeedMod,
                        rubberBandAcceleration * Time.deltaTime);
                }

                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerDefaultSpeed - rubberBandSpeedMod,
                    rubberBandAcceleration * Time.deltaTime);
            }
            else if (playerPosition >= AICars.Count / 2)
            {
                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed,
                    AIDefaultSpeed + (rubberBandSpeedMod * ((float) playerPosition / (AICars.Count + 1))),
                    rubberBandAcceleration * Time.deltaTime);
                foreach (var AICar in AICars)
                {
                    AICar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed,
                        AIDefaultSpeed - (rubberBandSpeedMod * ((float) playerPosition / (AICars.Count + 1))),
                        rubberBandAcceleration * Time.deltaTime);
                }
            }
        }
    }
}
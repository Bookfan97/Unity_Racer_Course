using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;
    public Checkpoint[] Checkpoints;
    public int totalLaps, playerPosition, countdownCurrent = 3, playerStartPosition, numAIToSpawn;
    public CarController playerCar;
    public List<CarController> AICars = new List<CarController>();
    public float timeBetweenPositionCheck = 0.2f, AIDefaultSpeed = 30f, playerDefaultSpeed = 30f, rubberBandSpeedMod = 3.5f, rubberBandAcceleration = 0.5f, timeBetweenStartCount = 1f;
    private float positionCheckCounter, startCounter;
    public bool isStarting, raceCompleted;
    public Transform[] startPoints;
    public List<CarController> carsToSpawn = new List<CarController>();
    public string RaceCompleteScene;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
        totalLaps = RaceInfoManager.instance.noOfLaps;
        numAIToSpawn = RaceInfoManager.instance.noOfAI;
        for (int i = 0; i < Checkpoints.Length; i++)
        {
            Checkpoints[i].checkpointNumber = i;
        }
        isStarting = true;
        startCounter = timeBetweenStartCount;
        UIManager.instance.countdownText.text = countdownCurrent.ToString();
        playerStartPosition = Random.Range(0, numAIToSpawn + 1);
        playerCar = Instantiate(RaceInfoManager.instance.racerToUse, startPoints[playerStartPosition].position, startPoints[playerStartPosition].rotation);
        playerCar.isAI = false;
        playerCar.GetComponent<AudioListener>().enabled = true;
        CameraSwitcher.instance.SetTarget(playerCar);

        //playerCar.transform.position = startPoints[playerStartPosition].position;
        //playerCar.theRB.transform.position = startPoints[playerStartPosition].position;

        for (int i = 0; i < numAIToSpawn + 1; i++)
        {
            if (i != playerStartPosition)
            {
                int selectedCar = Random.Range(0, carsToSpawn.Count);
                AICars.Add(Instantiate(carsToSpawn[selectedCar], startPoints[i].position, startPoints[i].rotation));
                if (carsToSpawn.Count > numAIToSpawn - i)
                {
                    carsToSpawn.RemoveAt(selectedCar);
                }
            }
        }
        UIManager.instance.positionText.text = (playerStartPosition + 1) + "/" + (AICars.Count + 1);
    }

    // Update is called once per frame
    private void Update()
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

                foreach (CarController AICar in AICars)
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
                                    Checkpoints[AICar.nextCheckpoint].transform.position))
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
                foreach (CarController AICar in AICars)
                {
                    AICar.maxSpeed = Mathf.MoveTowards(AICar.maxSpeed, AIDefaultSpeed + rubberBandSpeedMod,
                        rubberBandAcceleration * Time.deltaTime);
                }

                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerDefaultSpeed - rubberBandSpeedMod,
                    rubberBandAcceleration * Time.deltaTime);
            }
            else
            {
                foreach (CarController AICar in AICars)
                {
                    AICar.maxSpeed = Mathf.MoveTowards(AICar.maxSpeed,
                        AIDefaultSpeed - (rubberBandSpeedMod * ((float)playerPosition / ((float)AICars.Count + 1))),
                        rubberBandAcceleration * Time.deltaTime);
                }
                playerCar.maxSpeed = Mathf.MoveTowards(playerCar.maxSpeed, playerDefaultSpeed + (rubberBandSpeedMod * ((float)playerPosition / ((float)AICars.Count + 1))), rubberBandAcceleration * Time.deltaTime);
            }
        }
    }

    public void FinishRace()
    {
        raceCompleted = true;
        string place;
        switch (playerPosition)
        {
            case 1:
                place = "1st";
                break;

            case 2:
                place = "2nd";
                break;

            case 3:
                place = "3rd";
                break;

            default:
                place = playerPosition + "th";
                break;
        }
        UIManager.instance.raceResultText.text = "You Finished " + place;
        UIManager.instance.resultScreen.SetActive(true);
    }

    public void ExitRace()
    {
        SceneManager.LoadScene(RaceCompleteScene);
    }
}
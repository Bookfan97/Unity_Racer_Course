﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public TMP_Text lapCounterText, bestTimeText, currentTimeText, positionText, countdownText, GOText, raceResultText, totalTimeText;
    public GameObject resultScreen, pauseScreen, trackUnlockedMessage;
    public bool isPaused;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseUnpause();
        }
    }

    public void PauseUnpause()
    {
        isPaused = !isPaused;
        pauseScreen.SetActive(isPaused);

        if (isPaused)
        {
            Time.timeScale = 0f;
        }
        else
        {
            Time.timeScale = 1f;
        }
    }

    public void ExitRace()
    {
        Time.timeScale = 1f;
        RaceManager.instance.ExitRace();
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Quit");
    }
}
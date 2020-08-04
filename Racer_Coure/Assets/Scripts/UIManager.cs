using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
    public TMP_Text lapCounterText, bestTimeText, currentTimeText, positionText, countdownText, GOText, raceResultText, totalTimeText;
    public GameObject resultScreen;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ExitRace()
    {
        RaceManager.instance.ExitRace();
    }
}

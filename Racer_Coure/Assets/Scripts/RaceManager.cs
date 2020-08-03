using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaceManager : MonoBehaviour
{
    public static RaceManager instance;
    public Checkpoint[] Checkpoints;
    public int totalLaps; 
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
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

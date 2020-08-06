using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackSelectButton : MonoBehaviour
{
    public string trackSceneName;
    public Image TrackImage;
    public int raceLaps = 4;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SelectTrack()
    {
        RaceInfoManager.instance.trackToLoad = trackSceneName;
        RaceInfoManager.instance.noOfLaps = raceLaps;
        MainMenu.instance.trackSelectImage.sprite = TrackImage.sprite;
        RaceInfoManager.instance.trackSprite = TrackImage.sprite;
        MainMenu.instance.CloseTrackSelect();
    }
}

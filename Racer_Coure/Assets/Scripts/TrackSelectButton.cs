using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TrackSelectButton : MonoBehaviour
{
    public string trackSceneName, trackToUnlock;
    public Image TrackImage;
    public int raceLaps = 4;
    private bool isLocked;

    public GameObject unlockedText;

    // Start is called before the first frame update
    void Start()
    {
        if (!PlayerPrefs.HasKey(trackSceneName + "_unlocked"))
        {
            isLocked = true;
            TrackImage.color = Color.grey;
            unlockedText.SetActive(true);
        }
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SelectTrack()
    {
        if (!isLocked)
        {
            unlockedText.SetActive(false);
            RaceInfoManager.instance.trackToLoad = trackSceneName;
            RaceInfoManager.instance.numLaps = raceLaps;
            MainMenu.instance.trackSelectImage.sprite = TrackImage.sprite;
            RaceInfoManager.instance.trackSprite = TrackImage.sprite;
            MainMenu.instance.CloseTrackSelect();
            RaceInfoManager.instance.trackToUnlock = trackToUnlock;
        }
    }
}

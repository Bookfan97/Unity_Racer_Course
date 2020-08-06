using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
    public AudioSource MusicPlayer;
    public AudioClip[] musicSelection;
    // Start is called before the first frame update
    void Start()
    {
        MusicPlayer.clip = musicSelection[Random.Range(0, musicSelection.Length)];
        MusicPlayer.Play();
    }
}

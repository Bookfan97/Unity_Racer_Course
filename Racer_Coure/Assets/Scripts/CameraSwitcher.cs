using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwitcher : MonoBehaviour
{
    public static CameraSwitcher instance;
    public GameObject[] cameras;
    private int currentCamera;
    public CameraController topDownCam;
    public Cinemachine.CinemachineVirtualCamera FollowCamera;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.C))
        {
            currentCamera++;
            if (currentCamera >= cameras.Length)
            {
                currentCamera = 0;
            }

            for (int i = 0; i < cameras.Length; i++)
            {
                if (i == currentCamera)
                {
                    cameras[i].SetActive(true);
                }
                else
                {
                    cameras[i].SetActive(false);
                }
            }
        }
    }

    public void SetTarget(CarController playerCar)
    {
        topDownCam.target = playerCar;
        FollowCamera.m_Follow = playerCar.transform;
        FollowCamera.m_LookAt = playerCar.transform;
    }
}

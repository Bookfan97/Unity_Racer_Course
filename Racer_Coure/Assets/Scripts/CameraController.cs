using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public CarController target;
    private Vector3 offsetDirection;
    public float minDistance =  20f, maxDistance = 50f;
    private float activeDistance;
    public Transform startTargetOffest;
    // Start is called before the first frame update
    void Start()
    {
        offsetDirection = transform.position - startTargetOffest.position;//target.transform.position;
        activeDistance = minDistance;
        offsetDirection.Normalize();
    }

    // Update is called once per frame
    void Update()
    {
        activeDistance = minDistance + ((maxDistance - minDistance) * (target.theRB.velocity.magnitude/target.maxSpeed));
        transform.position = target.transform.position + (offsetDirection * activeDistance);
    }
}

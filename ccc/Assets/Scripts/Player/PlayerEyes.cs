using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEyes : MonoBehaviour
{

    private Camera camera;


    private Vector3 originalPos;
    // Start is called before the first frame update
    void Start()
    {
        originalPos = transform.localPosition;
        camera = Camera.main;
    }

    public float magnitude = 0.0005f;
    
    // Update is called once per frame
    void Update()
    {
        Vector3 target = camera.ScreenToWorldPoint(Input.mousePosition);
        Vector3 distanceToTarget = target - transform.position;

    //var finalPupilPosition : Vector3 = eye.transform.position + distanceToTarget; 
        
    // clamp the distance so it never exceeds the size of the eyeball
        distanceToTarget = Vector3.ClampMagnitude( distanceToTarget, magnitude);

        // place the pupil at the desired position relative to the eyeball
        Vector3 finalPupil = originalPos + distanceToTarget;
        transform.localPosition = finalPupil;
    }
}

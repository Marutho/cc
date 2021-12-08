using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Parallax : MonoBehaviour
{
    //public Transform close;
    //public Transform mid;
    //public Transform far;

    private float closeFactor = 1.0f;
    private float midFactor = 0.8f;
    private float farFactor = 0.3f;

    private Vector3 closeInitialPos;
    private Vector3 midInitialPos;
    private Vector3 farInitialPos;

    private float speed = 3f;

    private Camera cam;
    private float height;
    private float width;
    
    private float reset = -24.24f;  // magic lol
    private float imgWidth = 16.8f; // more magic
    
    private float textureSpeed = -0.2f;
    private Image image;
    private Renderer rend;

    private void Start()
    {
        cam = Camera.main;
        //closeInitialPos = close.position;
        //midInitialPos = mid.position;
        //farInitialPos = far.position;

        image = GetComponent<Image>();

        height = cam.orthographicSize;
        width = height * cam.aspect;
        
        rend = GetComponent<Renderer>();
    }

    // Update is called once per frame
    void Update()
    {
        float dt = Time.deltaTime;
        rend.material.mainTextureOffset = new Vector2(textureSpeed * Time.deltaTime, 0f);

        //image.material.mainTextureOffset += new Vector2(textureSpeed,0.0f);
        /*
        float dt = Time.deltaTime;

        Transform[] closeChildren = new[]
        {
            close.GetChild(0),
            close.GetChild(1)
        };
        Transform[] midChildren = new[]
        {
            mid.GetChild(0),
            mid.GetChild(1)
        };
        Transform[] farChildren = new[]
        {
            far.GetChild(0),
            far.GetChild(1)
        };

        closeChildren[0].position += Vector3.left * (speed * closeFactor * dt);
        closeChildren[1].position += Vector3.left * (speed * closeFactor * dt);
        
        midChildren[0].position += Vector3.left * (speed * midFactor * dt);
        midChildren[1].position += Vector3.left * (speed * midFactor * dt);
        
        farChildren[0].position += Vector3.left * (speed * farFactor * dt);
        farChildren[1].position += Vector3.left * (speed * farFactor * dt);

        if (closeChildren[0].position.x < closeInitialPos.x - 2f * width)
            closeChildren[0].position += Vector3.right * width;
        if (closeChildren[1].position.x < closeInitialPos.x - 2f * width)
            closeChildren[1].position += Vector3.right * width;
        
        if (midChildren[0].position.x < midInitialPos.x - 2f * width)
            midChildren[0].position += Vector3.right * width;
        if (midChildren[1].position.x < midInitialPos.x - 2f * width)
            midChildren[1].position += Vector3.right * width;
        
        if (farChildren[0].position.x < farInitialPos.x - 2f * width)
            farChildren[0].position += Vector3.right * width;
        if (farChildren[1].position.x < farInitialPos.x - 2f * width)
            farChildren[1].position += Vector3.right * width;*/
    }
}

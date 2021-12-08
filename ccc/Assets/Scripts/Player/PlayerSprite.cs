using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSprite : MonoBehaviour
{
    [SerializeField]
    private GameObject playerLeft;

    [SerializeField]
    private GameObject playerRight;

    private Camera _camera;

    private PlayerBehaviour _pb;

    public GameObject PlayerLeft => playerLeft;

    public GameObject PlayerRight => playerRight;
    
    void Start()
    {
        _camera = Camera.main;
        _pb = FindObjectOfType<PlayerBehaviour>();
    }


    void Update()
    {
        
        Vector3 pos = _camera.ScreenToWorldPoint(Input.mousePosition);
        if (pos.x >= _pb.transform.position.x)
        {
            playerRight.SetActive(true);
            playerLeft.SetActive(false);
        }
        else
        {
            playerRight.SetActive(false);
            playerLeft.SetActive(true);
        }
    }

    private void OnDestroy()
    {
        Destroy(PlayerLeft);
        Destroy(PlayerRight);
    }
}

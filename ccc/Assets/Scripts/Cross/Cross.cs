using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cross : MonoBehaviour
{
    [SerializeField]
    private Texture2D _crossHover;
    
    [SerializeField]
    private Texture2D _cross;
    
    [SerializeField]
    private Texture2D _crossAction;

    private Camera camera;
    
    void Start()
    {
        camera = Camera.main;
    }

    private float timerBetweenHits = 0.0f;
    private bool hitted = false;

    RaycastHit2D[] hits = new RaycastHit2D[128];
    void Update()
    {
        int n = Physics2D.RaycastNonAlloc(transform.position, Vector2.one, hits);
        if (hitted)
        {
            timerBetweenHits += Time.deltaTime;
            if (timerBetweenHits >= 0.2f)
            {
                hitted = false;
                timerBetweenHits = 0.0f;
            }
            return;
        }
        bool hover = false;
        for (int i = 0; i < n; ++i)
        {
            if (hits[i].collider.GetComponent<ObstacleAttract>() || hits[i].collider.GetComponent<Enemy>())
            {
                hover = true;
                break;
            }
        }
        transform.position = camera.ScreenToWorldPoint(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            Cursor.SetCursor(_crossAction, Vector2.zero, CursorMode.Auto);
            hitted = true;
        }
        else if (hover) // crossHover
        {
            Cursor.SetCursor(_crossHover, Vector2.zero, CursorMode.Auto);
            //ActivateThis(_crossHover);
        }
        else
        {
            Cursor.SetCursor(_cross, Vector2.zero, CursorMode.Auto);
           // ActivateThis(_cross);
           
        }
    }

    public void ActivateThis(GameObject t)
    {
        //_crossHover.SetActive(false);
        //_cross.SetActive(false);
        //_crossAction.SetActive(false);
        //t.SetActive(true);
    }

    private void OnDestroy()
    {
        Cursor.SetCursor(null, Vector2.zero, CursorMode.Auto);
    }
}

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuHandler : MonoBehaviour
{
    private Image img;

    private void Start()
    {
        img = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 1f);
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            SceneManager.LoadScene(2);
        }
        else if (Input.GetKeyDown(KeyCode.Q))
        {
            SceneManager.LoadScene(1);
        }
        else
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0f);
        }
    }
}

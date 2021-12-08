using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public GameObject bottom;
    public Sprite sprite1;
    public Sprite sprite2;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            bottom.GetComponent<SpriteRenderer>().sprite = sprite2;
            SceneManager.LoadScene(1);
        }

    }
}

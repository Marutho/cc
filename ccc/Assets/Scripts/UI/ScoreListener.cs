using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ScoreListener : MonoBehaviour
{
    private TMP_Text text;
    
    void Start()
    {
        text = GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = $"{PlayerBehaviour.UniquePlayer.Score}";
    }
}

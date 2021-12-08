using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DeathListener : MonoBehaviour
{
    private TMP_Text t;
    private ScoreSerial score;
    
    // Start is called before the first frame update
    void Start()
    {
        t = GetComponent<TMP_Text>();
        try
        {
            score = SaveSystem.Load<ScoreSerial>("scoreserial");
        }
        catch
        {
            SaveSystem.Save("scoreserial", new ScoreSerial(0));
            score = new ScoreSerial(0);
        }
    }

    private bool deadPrev = false;
    
    // Update is called once per frame
    void Update()
    {
        if (deadPrev) return;
        
        if (PlayerBehaviour.UniquePlayer.Dead)
        {
            deadPrev = true;
            int newMax = Math.Max(score.score, PlayerBehaviour.UniquePlayer.Score);
            SaveSystem.Save("scoreserial", new ScoreSerial(newMax));
            t.text = $"Max Score: {newMax}\nCurrent Score: {PlayerBehaviour.UniquePlayer.Score}";
        }
        else
        {
            t.text = "";
        }
    }
}

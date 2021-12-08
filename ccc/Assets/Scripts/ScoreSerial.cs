using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ScoreSerial
{
    public int score = 0;

    public ScoreSerial(int newScore)
    {
        score = newScore;
    }
}

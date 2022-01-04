using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameInfo
{
    public static int correctAnswers = 0;
    public static int score = 0;

    public static void Reset()
    {
        correctAnswers = 0;
        score = 0;
    }
}
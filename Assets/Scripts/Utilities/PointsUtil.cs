using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PointsUtil
{
    public static int GetCurrentPoints()
    {
        InitializePoints();
        return PlayerPrefs.GetInt("points");
    }

    public static void AddPoints(int amount)
    {
        PlayerPrefs.SetInt("points", GetCurrentPoints() + amount);
    }

    private static void InitializePoints()
    {
        if (!PlayerPrefs.HasKey("points"))
        {
            PlayerPrefs.SetInt("points", 500);
        }


    }


}

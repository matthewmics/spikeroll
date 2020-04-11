using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{

    public static readonly Vector3 TopServiceLocation = new Vector3(0f,4f);
    public static readonly Vector3 BottomServiceLocation = new Vector3(0f,-4f);

    public static readonly float TossSpeed = 3f;
    public static readonly float NormalSpeed = 5.5f;

    public static readonly string ClientAction = "CLIENTACTION|";
    public static readonly string HostAction = "HOSTACTION|";

    static GameConstants()
    {

    }
}

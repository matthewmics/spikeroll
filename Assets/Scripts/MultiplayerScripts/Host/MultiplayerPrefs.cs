using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerPrefs : MonoBehaviour
{
    [HideInInspector]
    public string P1Country = "";


    [HideInInspector]
    public string P2Country = "";

    public CountryModel CountryPl = null;
    public CountryModel CountryP2 = null;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}

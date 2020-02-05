using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MPSelectionSingleton
{

    private static MPSelectionSingleton instance = null;
    public static MPSelectionSingleton Intance
    {
        get
        {
            if(instance == null)
            {
                instance = new MPSelectionSingleton
                {
                    NumberOfPlayers = 3
                };
            }

            return instance;
        }
    }

    public int NumberOfPlayers { get; set; }

    private MPSelectionSingleton() { }

}

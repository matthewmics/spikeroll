using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonClientSelection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void SelectPlayer1()
    {
        transform.Find("player1").gameObject.SetActive(true);
    }

    public void SelectPlayer2()
    {
        transform.Find("player2").gameObject.SetActive(true);
    }

    public void DeselectPlayer1()
    {
        transform.Find("player1").gameObject.SetActive(false);
    }
    public void DeselectPlayer2()
    {
        transform.Find("player2").gameObject.SetActive(false);
    }
}

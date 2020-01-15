using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallDestination : MonoBehaviour
{
    BallController ballController;
    GameSession gameSession;
    MoveForwardAuto mfa;
    public GameObject FlashFire;

    public int DestinationIndex = 0;
    // Start is called before the first frame update
    void Start()
    {
        gameSession = GameObject.Find("GameSession").GetComponent<GameSession>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("mainball"))
        {
            gameSession.AddScore((DestinationIndex!=0)?0:1);
            if (this.gameObject.name.Contains("Super"))
            {
                GameObject.Instantiate(FlashFire, this.transform.position, Quaternion.identity);
            }
            ballController = col.GetComponent<BallController>();
            mfa = col.GetComponent<MoveForwardAuto>();
            mfa.IsMoving = true;
            ballController.ResetBallSize();
            ballController.enabled = false;
        }
    }

    void StopMovements()
    {

    }

}

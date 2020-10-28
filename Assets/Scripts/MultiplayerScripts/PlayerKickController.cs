using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerKickController : MonoBehaviour
{
    private bool _isKicking = false;
    private SepakBall sepakBall;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.name.Equals("ball"))
        {
            sepakBall = collider.GetComponent<SepakBall>();
            _isKicking = true;
        }

    }
    void OnTriggerExit2D(Collider2D collider)
    {
        if (collider.name.Equals("ball"))
        {
            _isKicking = false;
        }

    }


    public void ServeKick(float power, float intel)
    {
        Kick(true,power,intel);
    }

    private void Kick(bool isServeKick, float power, float intel)
    {
        if (_isKicking && sepakBall.IsInRange(1.5f))
        {

            if(sepakBall.IsSpecialActive)
            {
                sepakBall.SetSpecial(false);
            }

            if (sepakBall.IsSpecial)
            {
                sepakBall.SetSpecialActive(true);
            }
            else
            {
                sepakBall.speed = power;
            }

            if (transform.parent.name.Contains("player1"))
            {
                if (sepakBall.IsSpecial)
                {
                    sepakBall.CreateDestination(PlayerHost.GeneralControl.TopServer.position);
                    PlayerHost.Server.GameSession.Player1SpecialThreshold = 0f;
                }
                else
                {
                    sepakBall.MoveToTop();
                }

                if (isServeKick)
                {

                    PlayerHost.GeneralControl.EnablePlayersMovement();
                }
                else if (!sepakBall.IsSpecial)
                {                    
                    PlayerHost.GeneralControl.AddPlayer1Threshold(intel);
                }

            }
            else
            {
                if (sepakBall.IsSpecial)
                {
                    sepakBall.CreateDestination(PlayerHost.GeneralControl.BottomServer.position);
                    PlayerHost.Server.GameSession.Player2SpecialThreshold = 0f;
                }
                else
                {
                    sepakBall.MoveToBottom();
                }

                if (isServeKick)
                {

                    PlayerHost.GeneralControl.EnablePlayersMovement();
                }
                else if(!sepakBall.IsSpecial)
                {
                    PlayerHost.GeneralControl.AddPlayer2Threshold(intel);
                }
            }


        }
    }

    public void NormalKick(float power,float intel)
    {
        Kick(false,power,intel);
    }



    public void PassBall(bool isSuper = false)
    {
        if (_isKicking && sepakBall.IsInRange(1.5f))
        {
            sepakBall.speed = 3f;
            if (isSuper)
            {
                sepakBall.SetSpecial(true);
            }

            int randIndex = Random.Range(0,MultiplayerGameSession.NumberOfPlayers - 1);

            List<Transform> receivers = new List<Transform>();

            if (transform.parent.name.Contains("player1"))
            {
                int count = PlayerHost.Server.NetworkObjects.transform.childCount;
                for(int i = 0; i < count; i++)
                {
                    Transform t = PlayerHost.Server.NetworkObjects.transform.GetChild(i);
                    if (t.name.Contains("player1ai"))
                    {
                        receivers.Add(t);
                    }
                }
            }
            else
            {
                int count = PlayerHost.Server.NetworkObjects.transform.childCount;
                for (int i = 0; i < count; i++)
                {
                    Transform t = PlayerHost.Server.NetworkObjects.transform.GetChild(i);
                    if (t.name.Contains("player2ai"))
                    {
                        receivers.Add(t);
                    }
                }
            }

            MultiplayerAIController ai = receivers[randIndex].GetComponent<MultiplayerAIController>();
            sepakBall.CreateDestination(ai.transform.position);
            ai.IsKicker = true;

            

        }
    }

}

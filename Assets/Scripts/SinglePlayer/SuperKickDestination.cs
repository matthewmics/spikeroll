using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class SuperKickDestination : MonoBehaviour
{
    private GameObject dest;
    public float speed = 2f;
    bool isRight = true;

    private BallController ball;
    private bool lockedIn = false;
    private GameSession gameSession;
    float maxDist;
    float minDist;
    // Start is called before the first frame update
    void Start()
    {
        dest = GameObject.Find("ball destinations").transform.GetChild(0).gameObject;
        ball = GameObject.FindGameObjectWithTag("mainball").GetComponent<BallController>();
        gameSession = GameObject.Find("GameSession").GetComponent<GameSession>();
        maxDist = dest.transform.position.x + (dest.transform.lossyScale.x / 2);
        minDist = dest.transform.position.x - (dest.transform.lossyScale.x / 2);
    }

    // Update is called once per frame
    void Update()
    {
        if (lockedIn)
        {
            return;
        }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            lockedIn = true;
            gameSession.ModifyCharacterMovements(true);
            ball.ShowFireAnimation(true);
            gameSession.PlayIgniteAudio();
            ball.SetDestinationAndMove(this.gameObject,22f);
            return;
        }
        float movement = speed * Time.deltaTime;
        transform.position += new Vector3(movement,0);
        if (isRight)
        {
            if (transform.position.x >= maxDist)
            {
                isRight = false;
                speed *= -1;
            }
        }
        else
        {
            if (transform.position.x <= minDist)
            { 
                isRight = true;
                speed *= -1;
            }
        }

        transform.position = new Vector2(
            Mathf.Clamp(transform.position.x,minDist,maxDist)
            ,transform.position.y);
    }
}

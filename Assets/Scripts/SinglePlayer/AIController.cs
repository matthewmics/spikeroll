using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour
{

    public float StatSpd = 0;
    public float StatPow = 0;
    public float StatInt = 0;

    private float thresholdPerKick = 15f;
    public GameObject MainBall;
    private Navigator navigator;
    private Animator animator;

    private GameSession gameSession;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        navigator = transform.parent.GetChild(1).GetComponent<Navigator>();
        gameSession = GameObject.Find("GameSession").GetComponent<GameSession>();
    }

    public void TossBall()
    {
        GetComponent<Animator>().SetTrigger("Toss");
        GameObject g = GameObject.Instantiate(MainBall,transform.position,Quaternion.identity);
        if (transform.parent.parent.name.Equals("top"))
        {
            g.GetComponent<BallController>().SpawnTarget(0,1,GameConstants.TossSpeed);
        }
        else
        {
            g.GetComponent<BallController>().SpawnTarget(1, 1, GameConstants.TossSpeed);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void OnEnable()
    {
        if(animator != null)
            animator.enabled = true;
        if (navigator != null)
            navigator.enabled = true;
    }
    void OnDisable()
    {
        animator.enabled = false;
            navigator.enabled = false;
    }

    private BallController FindBall()
    {
        return GameObject.FindGameObjectWithTag("mainball").GetComponent<BallController>();
    }
    void OnTriggerStay2D(Collider2D collider)
    {
        if (!enabled) 
            return;

        if (collider.CompareTag("mainball") && FindBall().IsBallKickable())
        {
            if (transform.parent.parent.name.Equals("bottom"))
            {
                gameSession.AddThreshold(thresholdPerKick);
            }
            navigator.DoKick();
        }
    }
}

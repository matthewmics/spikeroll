using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class PlayerController : MonoBehaviour
{

    public float StatSpd = 0;
    public float StatPow = 0;
    public float StatInt = 0;

    private float thresholdPerKick = 15f;
    public float speed = 5f;
    Rigidbody2D rb;
    private Vector2 movement;
    private Animator animator;
    private bool isBallInRange = false;
    private bool hasServed = false;

    private float stopDuration = 0.417f;
    //private float stopDuration = 0.05f;
    private float stopMarker = 0f;


    private GameSession gameSession;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        //ballCollider = GetComponentInChildren<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        movement = Vector2.zero;

        gameSession = GameObject.Find("GameSession").GetComponent<GameSession>();
    }

    // Update is called once per frame
    void Update()
    {
        if (gameSession.isServing)
        {
            if (CrossPlatformInputManager.GetButtonDown("Jump") && !hasServed)
            {
                hasServed = true;
                DoNormalKick(true);
            }
            return;
        }

        //Debug.Log(Vector2.Distance(transform.position, FindBall().gameObject.transform.position));

        if (Time.time < stopMarker)
        {
            movement = Vector2.zero;
            return;
        }

        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            DoNormalKick();
            return;
        }

        if (CrossPlatformInputManager.GetButtonDown("Fire2"))
        {
            DoPassKick();
            return;
        }

        if (CrossPlatformInputManager.GetButtonDown("Fire3"))
        {
            DoPassKick(true);
            return;
        }

        movement.x = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        movement.y = CrossPlatformInputManager.GetAxisRaw("Vertical");
        animator.SetBool("IsMoving", movement.x != 0 || movement.y != 0);
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("mainball"))
        {
            isBallInRange = true;
        }
    }

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.CompareTag("mainball"))
        {
            isBallInRange = false; 
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * StatSpd * Time.fixedDeltaTime);
    }

    void OnEnable()
    {
        if (animator != null)
            animator.enabled = true;
    }
    void OnDisable()
    {
        animator.enabled = false;
    }

    private BallController FindBall()
    {
        return GameObject.FindGameObjectWithTag("mainball").GetComponent<BallController>();
    }

    public void DoNormalKick(bool isServeKick = false)
    {
        if (isBallInRange)
        {
            BallController bctrl = FindBall();
            if (transform.parent.name.Equals("bottom"))
            {
                if (bctrl.IsBallKickable(true))
                {
                    gameSession.AddThreshold(StatInt);
                    bctrl.SpawnTarget(0,ballSpeed:StatPow);
                    gameSession.isServing = false;
                }
            }
            else
            {
                if (bctrl.IsBallKickable(true))
                    bctrl.SpawnTarget(1);
            }
        }
        if (isServeKick)
        {
            animator.SetTrigger("ServeKick");
        }
        else
        {
            animator.SetTrigger("NormalKick");
        }
        
        stopMarker = Time.time + stopDuration;        
    }

    public void DoPassKick(bool isSuper = false)
    {
        if (isBallInRange)
        {
            BallController bctrl = FindBall();
            if (bctrl.IsBallKickable(true))
            {
                bctrl.SpawnTarget(1,2,-1f,transform.GetSiblingIndex());
                if (isSuper)
                {
                    gameSession.ResetSuperThreshold();
                    bctrl.IsSuperKick = true;

                }
                gameSession.isServing = false;
            }
        }
        animator.SetTrigger("NormalKick");
        stopMarker = Time.time + stopDuration;
    }
}

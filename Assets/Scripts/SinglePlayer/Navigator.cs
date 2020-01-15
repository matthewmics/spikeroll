using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Navigator : MonoBehaviour
{
    public float Speed = 3f;
    GameObject follower;
    Vector3 destination = Vector3.zero;
    Animator animator;
    Rigidbody2D rb;
    private bool isMoving = false;
    GameSession gameSession;
    AIController aIController;
    // Start is called before the first frame update
    void Start()
    {
        follower = transform.parent.GetChild(0).gameObject;
        aIController = follower.GetComponent<AIController>();
        animator = follower.GetComponent<Animator>();

        rb = GetComponent<Rigidbody2D>();

        gameSession = GameObject.Find("GameSession").GetComponent<GameSession>();
    }

    // Update is called once per frame
    void Update()
    {
        follower.transform.position = transform.position;
        if (isMoving)
        {
            RotateToTarget(destination);
            if (HasReachedDestination())
            {
                StopMoving();
            }
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }
    }
    void FixedUpdate()
    {
        if (isMoving)
        {
            rb.MovePosition(rb.position + (Vector2)rb.transform.up * aIController.StatSpd * Time.fixedDeltaTime);
        }
    }

    //void OnCollisionEnter2D(Collision2D col)
    //{
    //    if (col.gameObject.CompareTag("Player"))
    //    {
    //        StopMoving();
    //    }
    //}

    public void DoKick()
    {
        StopMoving();
        BallController ball = FindBall();

        if (!gameSession.isServing)
        {
            if (!ball.IsSuperKick)
            {
                animator.SetTrigger("NormalKick");
            }
            else
            {
                animator.SetTrigger("SpecialKick");
            }
        }
        else
        {
            animator.SetTrigger("ServeKick");
            gameSession.isServing = false;
        }
        

        if (transform.parent.parent.name.Equals("top"))
        {
            ball.SpawnTarget(1, ballSpeed: aIController.StatPow);
        }
        else
        {
            ball.SpawnTarget(0, ballSpeed:aIController.StatPow);
        }
    }

    private BallController FindBall()
    {
        return GameObject.FindGameObjectWithTag("mainball").GetComponent<BallController>();
    }
    
    private void StopMoving()
    {
        animator.SetBool("IsMoving", false);
        isMoving = false;   
    }

    private bool HasReachedDestination()
    {

        float a = Vector2.Distance(this.transform.position,destination);
        if(a <= 0.1f)
        {
            return true;
        }
        return false;
    }

    

    public void SetDestination(Vector3 trans)
    {
        isMoving = true;
        animator.SetBool("IsMoving", true);
        RotateToTarget(trans);
        destination = trans;
    }

    private void RotateToTarget(Vector3 targ)
    {

        targ.z = 0f;
        Vector3 objectPos = transform.position;
        targ.x = targ.x - objectPos.x;
        targ.y = targ.y - objectPos.y;

        float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
    }
}

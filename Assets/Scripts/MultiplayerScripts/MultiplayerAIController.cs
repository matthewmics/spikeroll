using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerAIController : MonoBehaviour
{

    float _speed = 5f;
    float _power = 5f;
    float _int = 5;

    private Rigidbody2D rb;
    [HideInInspector]
    public bool _canmove = false;

    [SerializeField]
    private bool isKicker = false;

    PlayerKickController kickController;

    public void SetSpeed(float speed)
    {
        _speed = speed;
    }

    public void SetPower(float power)
    {
        _power = power;
    }

    public void SetInt(float intel)
    {
        _int = intel;
    }

    public bool IsKicker
    {
        get => isKicker; set
        {

            isKicker = value; 
        }
    }

    private Animator anim;
    // Start is called before the first frame update
    void Start()
    {
        IsKicker = false;
        kickController = GetComponentInChildren<PlayerKickController>();
        _canmove = false;
        anim = GetComponent<Animator>();
        movement = new Vector2();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (IsKicker)
        {
            float dist = Vector3.Distance(transform.position, SepakBall.CurrentDest.
                transform.position);
            anim.SetBool("IsMoving", !((dist) < 0.001f));

            if(dist < 0.5f)
            {
                if (SepakBall.Ball.IsInRange(0.5f))
                {
                    kickController.NormalKick(_power,_int);

                    string animation = (SepakBall.Ball.IsSpecial) ? "SpecialKick" : "NormalKick";

                    anim.SetTrigger(animation);


                    PlayerHost.Server.Send($"ANIMATIONTRIGGER|" + name + "%{animation}", true);
                    isKicker = false;
                }
            }
        }
        else
        {
            anim.SetBool("IsMoving", _canmove);

        }
    }

    private float randMoveLastime = 0f;
    private Vector2 movement;

    void FixedUpdate()
    {

        if (Time.time > randMoveLastime && !IsKicker)
        {
            movement.x = Random.Range(-1f, 1f);
            movement.y = Random.Range(-1f, 1f);
            randMoveLastime = Time.time + 0.7f;
        }


        if (!_canmove)
        {
            movement.x = movement.y = 0f;
        }

        if (IsKicker)
        {
            movement = Vector2.MoveTowards(rb.position, SepakBall.CurrentDest.transform.position, _speed * Time.fixedDeltaTime);
            rb.MovePosition(movement);
            
        }
        else
        {
            rb.MovePosition(rb.position + movement * _speed * Time.fixedDeltaTime);
        }
    }
}

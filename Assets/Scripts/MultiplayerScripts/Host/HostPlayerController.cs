using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class HostPlayerController : MonoBehaviour
{
    [HideInInspector]
    public PlayerHost server;


    //
    Vector2 movement;
    //
    float _speed = 5f;
    float _power = 8f;
    float _int = 50f;
    //
    Rigidbody2D rb;
    Animator animator;
    PlayerKickController kickController;
    // Start is called before the first frame update
    void Start()
    {
        kickController = transform.GetComponentInChildren<PlayerKickController>();
        server = GameObject.Find("Server").GetComponent<PlayerHost>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    float lastTime = 0f;

    [HideInInspector]
    public bool _canmove = false;
    [HideInInspector]
    public bool _canserve = false;

    
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

    // Update is called once per frame
    void Update()
    {
        movement.x = 0; movement.y = 0;

        if (_canmove || _canserve)
        {
            if (CrossPlatformInputManager.GetButtonDown("Jump"))
            {
                if (_canserve)
                {
                    _canserve = false;
                    animator.SetTrigger("ServeKick");
                    server.Send("ANIMATIONTRIGGER|" + name + "%ServeKick", true);
                    kickController.ServeKick(_power,_int);
                }
                else
                {
                    animator.SetTrigger("NormalKick");
                    server.Send("ANIMATIONTRIGGER|" + name + "%NormalKick", true);
                    _canmove = false;
                    Invoke("EnableMove", 0.471f);
                    kickController.NormalKick(_power,_int);
                }
            } 
            else if (CrossPlatformInputManager.GetButtonDown("Fire2"))
            {
                if (!_canserve)
                {
                    animator.SetTrigger("NormalKick");
                    server.Send("ANIMATIONTRIGGER|" + name + "%NormalKick", true);
                    _canmove = false;
                    Invoke("EnableMove", 0.471f);
                    kickController.PassBall();
                }
            }
            else if (CrossPlatformInputManager.GetButtonDown("Fire3"))
            {

                if (!_canserve)
                {
                    animator.SetTrigger("NormalKick");
                    server.Send("ANIMATIONTRIGGER|" + name + "%NormalKick", true);
                    _canmove = false;
                    Invoke("EnableMove", 0.471f);
                    kickController.PassBall(true);
                }
            }
        }

        if (_canmove)
        {
            movement.x = CrossPlatformInputManager.GetAxisRaw("Horizontal");
            movement.y = CrossPlatformInputManager.GetAxisRaw("Vertical");

            animator.SetBool("IsMoving", (movement.x != 0 || movement.y != 0));
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }


    }

    void EnableMove() 
    {
        _canmove = true;
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * _speed * Time.fixedDeltaTime);
    }

}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkClientController : MonoBehaviour
{
    PlayerHost server;
    [HideInInspector]
    public Vector2 movement;
    Rigidbody2D rb;
    Animator animator;

    [HideInInspector]
    public bool _canmove = false;
    [HideInInspector]
    public bool _canserve = false;

    PlayerKickController kickController;
    //
    float _speed = 5f;
    float _power = 5f;
    float _int = 50f;


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
    //
    // Start is called before the first frame update
    void Start()
    {
        kickController = transform.GetComponentInChildren<PlayerKickController>();
        server = GameObject.Find("Server").GetComponent<PlayerHost>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private float lastTime = 0.0f;
    // Update is called once per frame
    void Update()
    {
        if (_canmove)
        {
            animator.SetBool("IsMoving", (movement.x != 0 || movement.y != 0));
        }
        else
        {
            animator.SetBool("IsMoving", false);
        }

    }

    public void DoPass()
    {
        if (_canmove || _canserve)
        {
            animator.SetTrigger("NormalKick");
            server.Send("ANIMATIONTRIGGER|" + name + "%NormalKick", true);
            _canmove = false;
            Invoke("EnableMove", 0.471f);
            kickController.PassBall();
        }
    }

    public void DoSpecial()
    {
        if ((_canmove || _canserve) && PlayerHost.Server.GameSession.Player2SpecialThreshold == 100f)
        {
            animator.SetTrigger("NormalKick");
            server.Send("ANIMATIONTRIGGER|" + name + "%NormalKick", true);
            _canmove = false;
            Invoke("EnableMove", 0.471f);
            kickController.PassBall(true);
        }
    }

    public void DoKick()
    {
        if (_canmove || _canserve)
        {
        //animator.SetTrigger("NormalKick");
        //server.Send("ANIMATIONTRIGGER|" + name + "%NormalKick", true);
        //kickController.NormalKick();
        //_canmove = false;
        //Invoke("EnableMove", 0.471f);
        //  Debug.Log(_canserve + " can serve");

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
                kickController.NormalKick(_power, _int);
            }
        }
    }

    void EnableMove()
    {
        _canmove = true;
    }

    void FixedUpdate()
    {
        if (!_canmove)
        {
            movement.y = 0;
            movement.x = 0;
        }

        rb.MovePosition(rb.position + movement * _speed * Time.fixedDeltaTime);
    }
}

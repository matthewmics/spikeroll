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
    //
    float _speed = 5f;
    //
    // Start is called before the first frame update
    void Start()
    {
        server = GameObject.Find("Server").GetComponent<PlayerHost>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    private float lastTime = 0.0f;
    // Update is called once per frame
    void Update()
    {

        animator.SetBool("IsMoving", (movement.x != 0 || movement.y != 0));
        if(Time.time > lastTime)
        {
            server.SendPosition(name, transform, animator.GetBool("IsMoving"));
            lastTime = Time.time + 0.05f;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * _speed * Time.fixedDeltaTime);
    }
}

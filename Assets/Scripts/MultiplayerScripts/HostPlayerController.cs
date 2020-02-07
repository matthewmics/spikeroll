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
    //
    Rigidbody2D rb;
    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        server = GameObject.Find("Server").GetComponent<PlayerHost>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
    }

    float lastTime = 0f;
    // Update is called once per frame
    void Update()
    {
        movement.x = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        movement.y = CrossPlatformInputManager.GetAxisRaw("Vertical");

        animator.SetBool("IsMoving", (movement.x != 0 || movement.y != 0));

        if(Time.time > lastTime)
        {
            lastTime = Time.time + 0.05f;
            server.SendPosition(name, transform, animator.GetBool("IsMoving"));
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * _speed * Time.fixedDeltaTime);
    }
}

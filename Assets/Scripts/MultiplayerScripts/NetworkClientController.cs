using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NetworkClientController : MonoBehaviour
{
    PlayerHost server;
    [HideInInspector]
    public Vector2 movement;
    Rigidbody2D rb;
    //
    float _speed = 5f;
    //
    // Start is called before the first frame update
    void Start()
    {
        server = GameObject.Find("Server").GetComponent<PlayerHost>();
        rb = GetComponent<Rigidbody2D>();
    }

    private float lastTime = 0.0f;
    // Update is called once per frame
    void Update()
    {
        if(Time.time > lastTime)
        {
            server.SendPosition(name, transform);
            lastTime = Time.time + 0.05f;
        }
    }

    void FixedUpdate()
    {
        rb.MovePosition(rb.position + movement * _speed * Time.fixedDeltaTime);
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class ClientPlayerControllerr : MonoBehaviour
{
    PlayerClient server;
    //
    Vector2 movement;
    //
    // Start is called before the first frame update
    void Start()
    {
        server = GameObject.Find("Server").GetComponent<PlayerClient>();
    }

    float lastTime = 0f;
    // Update is called once per frame
    void Update()
    {

        movement.x = CrossPlatformInputManager.GetAxisRaw("Horizontal");
        movement.y = CrossPlatformInputManager.GetAxisRaw("Vertical");

        if(Time.time > lastTime)
        {
            lastTime = Time.time + 0.05f;
            server.SendMoveRequest(name, movement.x, movement.y);
        }
    }
}

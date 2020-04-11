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



        if (CrossPlatformInputManager.GetButtonDown("Jump"))
        {
            server.Send("CLIENTACTION|KICK",true);
        }
        else if (CrossPlatformInputManager.GetButtonDown("Fire2"))
        {
            server.Send("CLIENTACTION|PASS", true);
        }
        else if (CrossPlatformInputManager.GetButtonDown("Fire3"))
        {
            server.Send("CLIENTACTION|SPECIAL", true);
        }


        if (Time.time > lastTime)
        {
            lastTime = Time.time + DelayReducer.REQUESTED_DELAY;
            server.SendMoveRequest(name, movement.x, movement.y);
        }

    }

}

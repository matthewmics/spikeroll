using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HostStateUpdater : MonoBehaviour
{

    private PlayerHost server;
        
    private float lastTime = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        server = GameObject.Find("Server").GetComponent<PlayerHost>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Time.time > lastTime)
        {
            // Debug.Log("STATE UPDATING");
            string updateMessage = "UPDATESTATE|";

            int count = transform.childCount;
           // Debug.Log(count);
            for(int i = 0; i < count; i++)
            {
                Transform obj = transform.GetChild(i);
                updateMessage += obj.name+"%"+obj.position.x+"%"+obj.position.y;

                string ismoving = "0";
                Animator animator = obj.GetComponent<Animator>();
                if (animator != null)
                {
                    if (animator.GetBool("IsMoving"))
                    {
                        ismoving = "1";
                    }
                }

                updateMessage += "%" + ismoving + ((i==count-1) ? "" : "|");


            }
                //Debug.Log(updateMessage);
                server.Send(updateMessage);



            //Debug.Log(updateMessage);

            lastTime = Time.time + 0.05f;
        }
    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClientBall : MonoBehaviour
{

    public static ClientBall Ball;
    public Transform Destination;

    Vector2 lastDestPos;
    // Start is called before the first frame update
    void Start()
    {
        Ball = this;
        lastDestPos = new Vector2(Destination.position.x,Destination.position.y);
    }

    void Update()
    {
        if (!(lastDestPos.x == Destination.position.x && lastDestPos.y == Destination.position.y))
        {
            lastDestPos = new Vector2(Destination.position.x, Destination.position.y);
            RotateToDest();
        }
    }

    public static void RotateToDest()
    {
       // Debug.Log("rotating");
        Ball.RotateToTarget(Ball.Destination);
    }

    private void RotateToTarget(Transform target)
    {
        Vector3 targ = target.transform.position;
        targ.z = 0f;
        Vector3 objectPos = transform.position;
        targ.x = targ.x - objectPos.x;
        targ.y = targ.y - objectPos.y;

        float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle - 90f));
    }
}

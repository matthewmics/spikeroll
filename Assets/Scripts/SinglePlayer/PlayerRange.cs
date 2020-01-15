using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRange : MonoBehaviour
{
    BoxCollider2D parentCollider;
    private int actionNumber = 0;

    public void SetActionNumber(int action)
    {
        Debug.Log("action number changed!");
        actionNumber = action;
    }
    // Start is called before the first frame update
    void Start()
    {
        parentCollider = GetComponentInParent<BoxCollider2D>();
    }

    // Update is called once per frame  
    void Update()
    {
        
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.CompareTag("mainball"))
        {
            Debug.Log("ball kicked!");
            switch (actionNumber)
            {
                case 0: // NORMAL KICK
                    Debug.Log("normal kick!");
                    break;
            }
        }
    }
}

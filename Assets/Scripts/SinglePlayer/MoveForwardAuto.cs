using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MoveForwardAuto : MonoBehaviour
{
    Rigidbody2D rb;

    public bool IsMoving
    {
        get
        {
            return isMoving;
        }
        set
        {
            if (value)
            {
                Invoke("StopMoving", 0.20f);
            }
            isMoving = value;
        }
    }
    private bool isMoving = false;
    private GameSession gameSession;
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        gameSession = GameObject.Find("GameSession").GetComponent<GameSession>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void StopMoving()
    {
        isMoving = false;
        gameSession.ModifyCharacterMovements(false);
        Invoke("RestartScene", 0.5f);
    }

    void RestartScene()
    {
        //SceneManager.LoadScene(0);
        gameSession.ShowHasScored();
        Invoke("RestartScene2", 1.5f);
    }

    void RestartScene2()
    {

        gameSession.DestroyAll();
        gameSession.SpawnPlayers();
    }

    void FixedUpdate()
    {
        if(IsMoving)
        rb.MovePosition(rb.position + (Vector2)rb.transform.up * 4f * Time.fixedDeltaTime);
    }
}

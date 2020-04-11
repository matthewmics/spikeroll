using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SepakBall : MonoBehaviour
{

    public GameObject CurrentDestination;
    public static GameObject CurrentDest;
    private SpriteRenderer CurrentDestinationSprite;

    public List<GameObject> TopPlayers;
    public List<GameObject> BottomPlayers;

    private Rigidbody2D rb;

    private bool _isMoving;

    public float speed;

    private float _initialScale;
    private float _maxScale = 2.7f;

    private PlayerHost server;
    private bool _isHalfway = false;

    private bool _isDead = false;

    private bool m_isSpecial = false;
    private bool m_isSpecialActive = false;


    public static SepakBall Ball;
    // Start is called before the first frame update
    void Start()
    {

        Ball = this;
        CurrentDest = CurrentDestination;
        CurrentDestinationSprite = CurrentDestination.GetComponent<SpriteRenderer>();
        server = GameObject.Find("Server").GetComponent<PlayerHost>();
        rb = GetComponent<Rigidbody2D>();
        _initialScale = 1;
    }

    public void SetBallDead(bool isDead = true)
    {
        _isDead = isDead;
        speed = 2f;
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
        {
            MoveToTop();
        }


        if (Input.GetKeyUp(KeyCode.B))
        {
            MoveToBottom();
        }
        if (Input.GetKeyUp(KeyCode.G))
        {
            StopMove();
        }

        if (_isMoving)
        {
            float localScale = transform.lossyScale.x;

            float newscale = localScale + ((1.5f * Time.deltaTime) * (_isHalfway ? -1 : 1));
            SetScale(Mathf.Clamp(newscale,_initialScale, _maxScale));
        }



    }

    public void SetSpecial(bool isSpecial)
    {
        m_isSpecial = isSpecial;
        transform.Find("BrightFire").gameObject.SetActive(isSpecial);

        if (!isSpecial)
            SetSpecialActive(false);
    }

    public void SetSpecialActive(bool isActive)
    {
        m_isSpecialActive = isActive;
        if (isActive)
        {
            this.speed = 22f;
        }
    }

    public bool IsSpecialActive
    {
        get => m_isSpecialActive;
    }

    public bool IsSpecial
    {
        get => m_isSpecial;
    }


    void FixedUpdate()
    {

        if (_isMoving)
        {
            rb.MovePosition(transform.position + rb.transform.up * speed * Time.fixedDeltaTime);
        }
    }

    private void SetScale(float scale)
    {
        transform.localScale = new Vector3(scale,scale,scale);
    }

    public void MoveToTop()
    {
        CreateDestination(true);
    }

    public bool IsInRange(float distance)
    {
        if (_isDead)
        {
            return false;
        }
        float dist = Vector3.Distance(transform.position, CurrentDestination.transform.position);

        return dist <= distance;
    }

    
    public void MoveToBottom()
    {
        CreateDestination(false);
    }

    public void CreateDestination(Vector3 position)
    {
        RemoveCurrentDestination();

        CurrentDestination.transform.position = position;


        CurrentDestinationSprite.sortingOrder = 0;
        Move();

        CreateDestination();
    }

    private void CreateDestination()
    {
        PlayerHost.GeneralControl.FindKicker();
    }

    private void CreateDestination(bool isTop)
    {

        RemoveCurrentDestination();
        int randonIndex = Random.Range(0, 3);

        Transform target = (isTop) ? TopPlayers[randonIndex].transform : BottomPlayers[randonIndex].transform;

        float minY = (isTop) ? 1.8f : -5.5f;
        float maxY = (isTop) ? 5.5f : -1.8f;
        float addedX = Random.Range(-1.5f, 1.5f);
        float addedY = Random.Range(-1.5f, 1.5f);

        float posX = Mathf.Clamp(target.position.x + addedX, -2.8f, 2.8f);
        float posY = Mathf.Clamp(target.position.y + addedY, minY, maxY);


        CurrentDestination.transform.position = new Vector3(posX, posY);
        CurrentDestinationSprite.sortingOrder = 0;


        Move();

        CreateDestination();
    }

    public void RemoveCurrentDestination()
    {
        CurrentDestinationSprite.sortingOrder = -10;
    }

    private void Move()
    {
        _isHalfway = false;
        SetScale(_initialScale);

        float distance = Vector2.Distance(transform.position, CurrentDestination.transform.position);

        CancelInvoke();
        Invoke("SetHalfway", (distance/speed)/2);

        this.RotateToTarget(CurrentDestination.transform);
        _isMoving = true;
    }

    private void SetHalfway()
    {
        _isHalfway = true;
    }

    public void StopMove()
    {
        RemoveCurrentDestination();
        _isMoving = false;
        SetScale(_initialScale);
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



    // BALL DEAD
    void OnTriggerEnter2D(Collider2D col)
    {
        if (col.name.Equals("destination"))
        {
            SetBallDead();

            Invoke("StopMove", 0.5f);

            string winner = "Player 1";
            string wintype = "round";
           // Debug.Log("ball position is " + transform.position.y + " " + (transform.position.y >= 0));
            if(transform.position.y >= 0)
            {
                if (PlayerHost.Server.GameSession.AddPlayer1Score(1))
                {
                    wintype = "set";
                }
            }
            else
            {
                if (PlayerHost.Server.GameSession.AddPlayer2Score(1))
                {
                    wintype = "set";
                }
                winner = "Player 2";
            }

            ScoreboardUI.Scoreboard.UpdateScoreboard();

            if (PlayerHost.Server.GameSession.Winner == null)
                PlayerHost.GeneralControl.RoundOver(winner + " wins " + wintype);
            else
            {
                PlayerHost.GeneralControl.SetCanPlayersMove(false);

                if (PlayerHost.Server.GameSession.Winner.Contains("1"))
                {
                    PlayerHost.NetUI.ShowClientLost();
                }
                else
                {
                    PlayerHost.NetUI.ShowClientWon();
                }
            }
              //  Debug.Log($"Game over. {PlayerHost.Server.GameSession.Winner} is the winner");
        }
    }

}

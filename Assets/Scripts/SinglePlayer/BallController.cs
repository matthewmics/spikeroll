using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    public GameObject BallDestination;
    public float speed = 2f;
    Rigidbody2D rb;
    private bool isMoving = false;
    private GameObject currentDestination;
    private float distanceOfDestination;

    public float ScaleSpeed = 1f;
    private float currentScale;
    private float minScale = 1f;
    private float maxScale = 2.7f;
    GameSession gameSession;

    private bool isSuperKick = false;

    private GameObject brightFire;
    public bool IsSuperKick
    {
        get
        {
            return isSuperKick;
        }
        set
        {
            isSuperKick = value;
        }
    }

    void OnDisable()
    {
        if(BallDestination != null)
        {
            ShowFireAnimation(false);
            Destroy(currentDestination);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        currentScale = minScale;
        rb = GetComponent<Rigidbody2D>();
        gameSession = GameObject.Find("GameSession").GetComponent<GameSession>();

        brightFire = transform.GetChild(1).gameObject;

    }

    public void ShowFireAnimation(bool active)
    {        
        brightFire?.SetActive(active);
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            SpawnTarget(0);
        }
        if (isMoving)
        {
            if (!IsHalfDistance())
            {
                currentScale = Mathf.Clamp(currentScale + (ScaleSpeed) * Time.deltaTime, minScale, maxScale);
            }
            else
            {
                currentScale = Mathf.Clamp(currentScale - (ScaleSpeed) * Time.deltaTime, minScale, maxScale);
            }
            transform.localScale = new Vector3(currentScale,currentScale);
        }
    }

    private bool IsHalfDistance()
    {
        float dist = Vector2.Distance((Vector2)transform.position,
            (Vector2)currentDestination.transform.position);

        if(dist <= distanceOfDestination / 2)
        {
            return true;
        }
        return false;
    }

    void FixedUpdate()
    {
        if(isMoving)
        rb.MovePosition(rb.position + (Vector2)rb.transform.up * speed * Time.fixedDeltaTime);
    }

    private void RotateToTarget(Transform target)
    {
        Vector3 targ = target.transform.position;
        targ.z = 0f;
        Vector3 objectPos = transform.position;
        targ.x = targ.x - objectPos.x;
        targ.y = targ.y - objectPos.y;

        float angle = Mathf.Atan2(targ.y, targ.x) * Mathf.Rad2Deg;
        transform.rotation = Quaternion.Euler(new Vector3(0, 0, angle -90f));
    }

    public bool IsBallKickable(bool isPlayer = false)
    {
        float range = (isPlayer) ? 1.5f : 1.1f;
        if(currentDestination != null)
        {
            if (Vector2.Distance(transform.position, currentDestination.transform.position)
                <= range)
            {
                return true;
            }
        }
        return false;
    }
    
    // 0 = random , 1 = toss serve

    public void SetDestinationAndMove(GameObject destination, float speed)
    {
        this.speed = speed;
        ResetBallSize();
        this.currentDestination = destination;
        RotateToTarget(currentDestination.transform);
        isMoving = true;
    }
    public void ResetBallSize()
    {

        transform.localScale = new Vector3(minScale, minScale);
        currentScale = minScale;
    }
    public void SpawnTarget(int destinationChild = 0, int spawnType = 0, float ballSpeed = -1f, int passerIndex = 0)
    {

        if (IsSuperKick)
        {

            IsSuperKick = false;
            gameSession.SummonSuperKickDestination();
            gameSession.ModifyCharacterMovements(false);
            isMoving = false;
            if (currentDestination != null)
                Destroy(currentDestination);

            return;
        }
        ShowFireAnimation(false);

        ResetBallSize();
        if (currentDestination != null)
        {
            Destroy(currentDestination);
        }
        Transform dest = GameObject.Find("ball destinations").transform.GetChild(destinationChild);
        Vector3 pos = Vector3.zero;

        switch (spawnType)
        {
            case 0:
                //   pos = GetRandPos(dest);
                if(gameSession.SuperKickReady() && destinationChild == 1)
                {
                    //pos = 
                    pos = gameSession.GetPlayerPosition();
                }
                else
                {
                    pos = GetPlayerPos((destinationChild != 0)?0:1,passerIndex,true,dest);
                }
                break;
            case 1:
                if(destinationChild == 0)
                {
                    pos = GameConstants.TopServiceLocation;
                }
                else
                {
                    pos = GameConstants.BottomServiceLocation;
                }
                break;
            case 2:
                pos = GetPlayerPos(destinationChild, passerIndex);
                ballSpeed = GameConstants.TossSpeed;
                break;  
        }

        currentDestination = GameObject.Instantiate(BallDestination,pos, Quaternion.Euler(0,0,0));
        currentDestination.GetComponent<BallDestination>().DestinationIndex = destinationChild;

        RotateToTarget(currentDestination.transform);

        distanceOfDestination = Vector2.Distance((Vector2)transform.position,
            (Vector2)currentDestination.transform.position);

        if(ballSpeed == -1f)
        {
            this.speed = GameConstants.NormalSpeed;
        }
        else
        {
            this.speed = ballSpeed;
        }

        isMoving = true;


        gameSession?.PlayKickAudio();

        if(spawnType!=1)
        SelectFetcher(destinationChild, dest);
    }


    private Vector3 GetPlayerPos(int destinationChild, int passerIndex, bool isRandom = false, Transform dest = null)
    {
        int pCount = gameSession.GetPlayerCount();
        if(passerIndex + 1 == pCount)
        {
            passerIndex = 0;
        }
        else
        {
            passerIndex++;
        }
        GameObject playerParent = (destinationChild == 0) ? gameSession.BottomPlayers : gameSession.TopPlayers ;


        if (isRandom)
        {
            int i = Random.Range(0, gameSession.GetPlayerCount());
            Vector3 pos = playerParent.transform.GetChild(i).transform.GetChild(0).position;
            //Debug.Log(i);
            bool isPlayerTarget = i == 0 && destinationChild == 0;
            float toRange = (isPlayerTarget) ? 0.4f: 2f;
            float randomX = Random.Range(-toRange, toRange);
            float randomY = Random.Range(-toRange, toRange);
            pos.x += randomX;
            pos.y += randomY;
           // Debug.Log(pos.y);
            pos.x = Mathf.Clamp(pos.x, -(dest.lossyScale.x / 2) + dest.position.x, (dest.lossyScale.x / 2) + dest.position.x);
            pos.y = Mathf.Clamp(pos.y, -(dest.lossyScale.y / 2) + dest.position.y, (dest.lossyScale.y / 2) + dest.position.y);

            return pos;
        }

        playerParent = (destinationChild == 0) ? gameSession.TopPlayers : gameSession.BottomPlayers;
        return playerParent.transform.GetChild(passerIndex).transform.GetChild(0).position;

    }
    //private Vector3 GetOpponentPos()
    //{

    //    return Vector3.zero;
    //}
    private Vector3 GetRandPos(Transform dest)
    {
        return new Vector2(Random.Range(-(dest.lossyScale.x / 2), dest.lossyScale.x / 2) + dest.position.x,
            Random.Range(-(dest.lossyScale.y / 2), dest.lossyScale.y / 2) + dest.position.y);
    }

    private void SelectFetcher(int destinationChild, Transform dest)
    {

        Navigator[] navs = GameObject.FindObjectsOfType<Navigator>();
        List<int> movers = new List<int>();

        int ballFetcher = -1;
        float currentDist = 0f;

        for (int i = 0; i < navs.Length; i++)
        {
            float dist = Vector2.Distance(navs[i].transform.position, currentDestination.transform.position);

            string loc = (destinationChild == 0) ? "top" : "bottom" ;

            if (navs[i].transform.parent.parent.name.Equals(loc))
            {

                if (ballFetcher == -1)
                {
                    ballFetcher = i;
                    currentDist = dist;
                }
                else if (currentDist > dist)
                {
                    ballFetcher = i;
                    currentDist = dist;
                }
                // navs[i].SetDestination(GetRandPos(dest));
                movers.Add(i);
            }

        }

        if (ballFetcher != -1 &&
            Vector2.Distance(GameObject.FindGameObjectWithTag("Player").transform.position,
            currentDestination.transform.position) > currentDist)
        {
            navs[ballFetcher].SetDestination(currentDestination.transform.position);
            foreach (int i in movers)
            {
                if(i!=ballFetcher)
                navs[i].SetDestination(GetRandPos(dest));
            }
        }
    }
}

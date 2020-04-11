using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiplayerGeneralControl : MonoBehaviour
{

    private List<Transform> playersTransform;
    private List<Vector3> playersTransformPosition;

    private Transform BottomTosser;
    private Transform TopTosser;

    [HideInInspector]
    public Transform BottomServer;
    [HideInInspector]
    public Transform TopServer;

    public RectTransform Player1SpecialThreshold;
    public GameObject Player1SpecialButton;

    public static RectTransform P1SpecialThreshold;
    public static GameObject P1SpecialButton;


    private Transform Ball;
    private SepakBall BallController;

    [HideInInspector]
    public PlayerHost server;

    // Start is called before the first frame update
    void Start()
    {
        P1SpecialThreshold = Player1SpecialThreshold;
        P1SpecialButton = Player1SpecialButton;
        
        server = GameObject.Find("Server").GetComponent<PlayerHost>();

        playersTransform = new List<Transform>();
        playersTransformPosition = new List<Vector3>();
        int childCount = transform.childCount;
        for(int i = 0; i < childCount; i++)
        {
            Transform t = transform.GetChild(i);
            if (t.name.Contains("player"))
            {
                playersTransform.Add(t);
                playersTransformPosition.Add(t.position);

                if (t.name.Equals("player1ai2"))
                {
                    BottomTosser = t;
                }
                else if (t.name.Equals("player2ai2"))
                {
                    TopTosser = t;
                }
                else if (t.name.Equals("player2"))
                {
                    TopServer = t;
                }
                else if (t.name.Equals("player1"))
                {
                    BottomServer = t;
                }
            }
            else if (t.name.Equals("ball"))
            {
                Ball = t;
                BallController = t.GetComponent<SepakBall>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            ResetPositions();
        }

        if (Input.GetKeyUp(KeyCode.Y))
        {
            TossTop();
        }

        if (Input.GetKeyUp(KeyCode.N))
        {
            TossBottom();
        }
    }

    public void ResetPositions()
    {
        SepakBall.Ball.SetSpecial(false);
        int count = playersTransform.Count;
        for(int i = 0; i < count; i++)
        {
            Transform t = playersTransform[i];
            t.position = playersTransformPosition[i];

            //HostPlayerController h = t.GetComponent<HostPlayerController>();
            //MultiplayerAIController ai = t.GetComponent<MultiplayerAIController>();
            //ClientPlayerControllerr c = t.GetComponent<ClientPlayerControllerr>();

            //if (h != null)
            //{

            //}
            //if(ai != null)
            //{

            //}
            //if(c != null)
            //{

            //}
        }
    }

    public void StartRound()
    {
        ResetPositions();

        if (server.GameSession.IsPlayer1Serve)
        {
            TossBottom();
        }
        else
        {
            TossTop();
        }
    }

    public void TossTop()
    {
        Toss();
        Animator anim = TopTosser.GetComponent<Animator>();
        anim.SetTrigger("Toss");
        SendTossTrigger(TopTosser);
        Ball.position = TopTosser.position;

        BallController.CreateDestination(TopServer.position);

        //
        SetCanPlayersMove(false);
        TopServer.GetComponent<NetworkClientController>()._canserve = true;
    }

    private void Toss()
    {
        BallController.speed = GameConstants.TossSpeed;
        SepakBall.Ball.SetBallDead(false);
        PlayerHost.Server.GameSession.Player1SpecialThreshold = 0f;
        PlayerHost.Server.GameSession.Player2SpecialThreshold = 0f;


    }

    private void SendTossTrigger(Transform tosser)
    {
        server.Send("ANIMATIONTRIGGER|"+tosser.name+"%Toss",true);
    }

    public void TossBottom()
    {
        Toss();
        Animator anim = BottomTosser.GetComponent<Animator>();
        anim.SetTrigger("Toss");
        SendTossTrigger(BottomTosser);
        Ball.position = BottomTosser.position;
        BallController.CreateDestination(BottomServer.position);

        //
        SetCanPlayersMove(false);
        BottomServer.GetComponent<HostPlayerController>()._canserve = true;
    }

    public void EnablePlayersMovement()
    {
        Invoke("EnableMove", 0.471f);
    }

    private void EnableMove()
    {
        SetCanPlayersMove(true);
    }

    public void SetCanPlayersMove(bool canmove)
    {
        HostPlayerController p1 = BottomServer.GetComponent<HostPlayerController>();
        p1._canmove = canmove; p1.CancelInvoke();

        NetworkClientController p2 = TopServer.GetComponent<NetworkClientController>();
        p2._canmove = canmove; p2.CancelInvoke();

        foreach(Transform ai in playersTransform)
        {
            MultiplayerAIController aiPlayer = ai.GetComponent<MultiplayerAIController>();
            if (aiPlayer != null)
            {
                aiPlayer._canmove = canmove;

                if (!canmove)
                {
                    aiPlayer.IsKicker = false;
                }
            }

        }
    }


    public void FindKicker()
    {
        string kickerTerm = (SepakBall.Ball.CurrentDestination.transform.position.y < 0) ?
                            "player1" : "player2";

        Transform kicker = null;
        float tempDistance = 0f;

        foreach(Transform i in playersTransform)
        {
            if (i.name.Contains(kickerTerm))
            {
                float dist = Vector3.Distance(i.position, SepakBall.Ball.CurrentDestination.transform.position);
             
                
                if(kicker == null)
                {
                    kicker = i;
                    tempDistance = dist;
                }
                else
                {

                    if(dist < tempDistance)
                    {
                        tempDistance = dist;
                        kicker = i;
                    }

                }
            }
        }

        MultiplayerAIController ai = kicker.GetComponent<MultiplayerAIController>();
        if (ai != null)
        {
            ai.IsKicker = true;
        }
    }

    public void RoundOver(string message = "")
    {
        PlayerHost.NetUI.ShowModalSend(message);
        Invoke("NextRound",1f);
    }

    private void NextRound()
    {
        PlayerHost.NetUI.CloseModalSend();
        StartRound();
    }

    public void AddPlayer1Threshold(float amount)
    {
        float toAdd = 
            Mathf.Clamp((Random.Range(amount / 5, amount)) + PlayerHost.Server.GameSession.Player1SpecialThreshold,
            0f,
            100f);


        PlayerHost.Server.GameSession.Player1SpecialThreshold = toAdd;
        //float currentAMount = Player1SpecialThreshold.rect.height + (Random.Range(amount / 5, amount));
        //currentAMount = Mathf.Clamp(currentAMount, 0f, 100f);
        //Player1SpecialThreshold.sizeDelta = new Vector2(Player1SpecialThreshold.rect.width, currentAMount);
        //if (currentAMount == 100f)
        //{
        //    Player1SpecialButton.SetActive(true);
        //}
    }

    public void AddPlayer2Threshold(float amount)
    {

        float toAdd =
            Mathf.Clamp((Random.Range(amount / 5, amount)) + PlayerHost.Server.GameSession.Player2SpecialThreshold,
            0f,
            100f);


        PlayerHost.Server.GameSession.Player2SpecialThreshold = toAdd;
    }


}

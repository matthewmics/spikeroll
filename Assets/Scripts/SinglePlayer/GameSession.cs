using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameSession : MonoBehaviour
{

    private const int SCORE_TO_WIN = 21;
    private const int SCORE_TO_WIN_LAST_SET = 15;
    private int currentScoreToWin;
    private const int SETS_TO_WIN = 2;
    private const int FINAL_SET = 2;
    private int currentSet = 0;

    private bool hasWinner = false;

    private int NumberOfPlayers;
 

    [System.Serializable]
    public class SepakCharacters
    {
        public GameObject AIEnemy;
        public GameObject AIAlly;
        public GameObject Player;
    }

    private Vector3[] topPlayersLocation = 
    { 
        GameConstants.TopServiceLocation, 
        new Vector3(-2.94f, 0.8f),
        new Vector3(2.86f, 0.88f)
    };


    private Vector3[] bottomPlayersLocation =
    {
        GameConstants.BottomServiceLocation,
        new Vector3(2.85f, -0.71f),
        new Vector3(-2.99f, -0.71f)
    };

    public SepakCharacters CharacterComponents;

    private SepakCharacters SelectedEnemyTeam;
    private SepakCharacters SelectedPlayerTeam;


    [Header("Players")]
    public GameObject TopPlayers;
    public GameObject BottomPlayers;

    [HideInInspector]
    public bool isServing = true;

    bool isTopServe = false;

    AIController ballTosser;

    GameObject player;

    [Header("Other")]
    public Text TextEndGame;
    public GameObject ScoreBoard;
    public RectTransform SuperThreshold;
    public GameObject SuperButton;
    public GameObject SuperKickDestination;
    public GameObject HasScoredMessage;
    public GameObject MatchFinishedPanel;
    public GameObject PausePanel;
    public GameObject EarnedPointsUI;
    public Text PlayerNameUI;
    public Text EnemyNameUI;
    public Text ScoredTeamText;
    public Text TopScoreUI;
    public Text TopSetScoreUI;
    public Text BottomScoreUI;
    public Text BottomSetScoreUI;

    [Header("Audio Clips")]
    public AudioClip kickAudio;
    public AudioClip igniteAudio;


    private TeamProps[] teams = new TeamProps[2];


    public bool dontPlay = false;
    public bool DemoPlay = false;

   

    private AudioSource audioSource;
    private float sfxVol;

    class TeamProps
    {
        public string TeamName;
        public int Score;
        public int SetCore;
        public Text UIScore;
        public Text UISetScore;
    }

    public void AddScore(int teamIndex)
    {

        var sd = GameObject.FindObjectsOfType<SuperKickDestination>();

        foreach(var i in sd)
        {
            Destroy(i.gameObject);
        }

        if (SelectionSingleton.instance.IsPractice)
        {
            return;
        }

        TeamProps t = teams[teamIndex];
        TeamProps to = teams[(teamIndex == 0) ? 1 : 0];
        string sr = "ROUND";
        t.Score += 1;
        t.UIScore.text = t.Score.ToString();
        string c = (teamIndex == 0) ? "red" : "blue";

        if((t.Score + to.Score) % 3 == 0)
        {
            isTopServe = !isTopServe;
        }

        if (t.Score >= currentScoreToWin && t.Score - to.Score > 1)
        {
            currentSet++;

            sr = "SET";
            t.SetCore += 1;
            t.UISetScore.text = t.SetCore.ToString();
            t.Score = 0;
            t.UIScore.text = t.Score.ToString();
            to.Score = 0;
            to.UIScore.text = t.Score.ToString();


            if (currentSet == FINAL_SET)
            {
                Debug.Log("FINAL SET");
                currentScoreToWin = SCORE_TO_WIN_LAST_SET;
            }

        }

        if(t.SetCore == SETS_TO_WIN)
        {
            MatchFinishedPanel.SetActive(true);
            hasWinner = true;
            sr = "MATCH";

            if(teamIndex == 1)
            {
                if (!SelectionSingleton.instance.IsMinigame)
                {
                    TextEndGame.text = "You've earned <color=\"yellow\">100</color> points";
                    EarnedPointsUI.SetActive(true);
                    PointsUtil.AddPoints(100);
                }
                else
                {
                    TextEndGame.text = "You've earned <color=\"yellow\">5</color> allocation points";
                    EarnedPointsUI.SetActive(true);
                    var vsac = VSAllCountriesModel.GetCurrentGame();
                    vsac.RemainingPoints += 5;
                    vsac.CurrentLevel = vsac.CurrentLevel < 4 ? vsac.CurrentLevel + 1 : 0 ;
                    VSAllCountriesModel.SaveInstance(vsac);
                    //PointsUtil.AddPoints(100);
                }
            }
        }

        ScoredTeamText.text = $"<color={c}>{t.TeamName}</color> WIN {sr}";
        
    }

    void Start()
    {

        if (SelectionSingleton.instance.IsPractice)
        {
            ScoreBoard.SetActive(false);
        }

        audioSource = GetComponent<AudioSource>();
        sfxVol = AudioUtil.GetSfx();

        if(AudioUtil.GetBgm() == 0f)
        {
            audioSource.clip = null;
        }

        currentScoreToWin = SCORE_TO_WIN;
        Time.timeScale = 1;
        teams[0] = new TeamProps { Score = 0, SetCore = 0,  
            TeamName =  SelectionSingleton.instance.OpponentCountry.ShortName, 
            UIScore = TopScoreUI,
        UISetScore = TopSetScoreUI};
        teams[1] = new TeamProps { Score = 0, SetCore = 0,  
            TeamName = SelectionSingleton.instance.PlayerCountry.ShortName, 
            UIScore = BottomScoreUI,
        UISetScore = BottomSetScoreUI };

        NumberOfPlayers = (DemoPlay) ? 3 : SelectionSingleton.instance.NumberOfPlayers;
        Screen.sleepTimeout = SleepTimeout.NeverSleep;

        if (SelectionSingleton.instance.PlayerCountry.Name.Equals(SelectionSingleton.instance.OpponentCountry.Name))
        {

            PlayerNameUI.text = SelectionSingleton.instance.PlayerCountry.Name+"(YOU)";
            EnemyNameUI.text = SelectionSingleton.instance.OpponentCountry.Name;
            teams[1].TeamName = PlayerNameUI.text;
            teams[0].TeamName = EnemyNameUI.text;

        }
        else
        {
            PlayerNameUI.text = SelectionSingleton.instance.PlayerCountry.Name;
            EnemyNameUI.text = SelectionSingleton.instance.OpponentCountry.Name;
        }

        SelectedPlayerTeam = CharacterComponents;
        SelectedEnemyTeam = CharacterComponents;

        if (dontPlay)
        {
            isServing = false;
        }
        else
        {
            SpawnPlayers();
        }
    }

    public void DestroyAll()
    {
        for(int i = 0; i < NumberOfPlayers; i++)
        {
            Destroy(TopPlayers.transform.GetChild(i).gameObject);
            Destroy(BottomPlayers.transform.GetChild(i).gameObject);
            GameObject ball = FindBall();
            if (ball != null)
            {
                Destroy(ball);
            }
        }
    }

    public GameObject FindBall()
    {
        return GameObject.FindGameObjectWithTag("mainball");
    }




    public void SpawnPlayers()
    {
        if (hasWinner)
        {
            return;
        }


        isServing = true;
        HideHasScored();
        ResetSuperThreshold();

        for(int i = 0; i < NumberOfPlayers; i++)
        {
            GameObject g = GameObject.Instantiate(SelectedEnemyTeam.AIEnemy,
                topPlayersLocation[i], Quaternion.identity,
                TopPlayers.transform);

            g.GetComponentInChildren<Animator>().runtimeAnimatorController = SelectionSingleton.instance.OpponentCountry.ControllerTop1;
            AIController aic = g.GetComponentInChildren<AIController>();
            aic.StatSpd = SelectionSingleton.instance.OpponentCountry.RawSpeed;
            aic.StatPow = SelectionSingleton.instance.OpponentCountry.RawPower;
            aic.StatInt = SelectionSingleton.instance.OpponentCountry.RawInt;

            if(i == 1)
            {
                if (isTopServe)
                {
                    ballTosser = g.GetComponentInChildren<AIController>();
                }
            }
        }

        for(int i = 0; i < NumberOfPlayers; i++)
        {
            GameObject toSpawn = (i == 0) ? SelectedPlayerTeam.Player : SelectedPlayerTeam.AIAlly ;
            GameObject g = GameObject.Instantiate(toSpawn,
                bottomPlayersLocation[i], Quaternion.identity,
                BottomPlayers.transform);

            //g.GetComponent<Animator>().runtimeAnimatorController = SelectionSingleton.instance.PlayerCountry.ControllerBottom1;

            AIController aic = g.GetComponentInChildren<AIController>();
            PlayerController pic = g.GetComponent<PlayerController>();

            if (aic != null)
            {
                g.GetComponentInChildren<Animator>().runtimeAnimatorController = SelectionSingleton.instance.PlayerCountry.ControllerBottom1;
                aic.StatSpd = SelectionSingleton.instance.PlayerCountry.RawSpeed;
                aic.StatPow = SelectionSingleton.instance.PlayerCountry.RawPower;
                aic.StatInt = SelectionSingleton.instance.PlayerCountry.RawInt;
            }
            else
            {
                g.GetComponent<Animator>().runtimeAnimatorController = SelectionSingleton.instance.PlayerCountry.ControllerBottom1;
                pic.StatSpd = SelectionSingleton.instance.PlayerCountry.RawSpeed;
                Debug.Log("PLAYER SPEED IS " + SelectionSingleton.instance.PlayerCountry.RawSpeed);
                pic.StatPow = SelectionSingleton.instance.PlayerCountry.RawPower;
                Debug.Log("PLAYER POWER IS " + SelectionSingleton.instance.PlayerCountry.RawPower);
                pic.StatInt = SelectionSingleton.instance.PlayerCountry.RawInt;
                Debug.Log("PLAYER INT IS " + SelectionSingleton.instance.PlayerCountry.RawInt);
            }

            if (i == 0)
            {
                player = g;
            }
            if (i == 1)
            {
                if (!isTopServe)
                {
                    ballTosser = g.GetComponentInChildren<AIController>();
                }
            }

        }
        Invoke("StartServing", 1f);
    }
    // Start is called before the first frame update

    public void AddThreshold(float amount)
    {
        float currentAMount = SuperThreshold.rect.height + (Random.Range(amount / 5, amount));
        currentAMount = Mathf.Clamp(currentAMount, 0f, 100f);
        SuperThreshold.sizeDelta = new Vector2(SuperThreshold.rect.width, currentAMount);
        if(currentAMount == 100f)
        {
            SuperButton.SetActive(true);
        }
    }

    public void ModifyCharacterMovements(bool enabled)
    {
        int count = TopPlayers.transform.childCount;
        for(int i = 0; i < count; i++)
        {
            GameObject g = TopPlayers.transform.GetChild(i).gameObject;
            AIController ai = g.GetComponentInChildren<AIController>();
            PlayerController p = g.GetComponent<PlayerController>();
            if(ai)
            {
                ai.enabled = enabled;
            }
            else if(p != null)
            {
                p.enabled = enabled;
            }
        }
        count = BottomPlayers.transform.childCount;
        for (int i = 0; i < count; i++)
        {
            GameObject g = BottomPlayers.transform.GetChild(i).gameObject;
            AIController ai = g.GetComponentInChildren<AIController>();
            PlayerController p = g.GetComponent<PlayerController>();
            if (ai)
            {
                ai.enabled = enabled;
            }
            else if(p != null)
            {
                p.enabled = enabled;
            }
        }
    }


    void StartServing()
    {
        ballTosser.TossBall();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }

    public void SummonSuperKickDestination()
    {
        float posy = TopPlayers.transform.GetChild(0).GetChild(0).position.y;
        GameObject.Instantiate(SuperKickDestination,new Vector3(0,posy),Quaternion.identity);

    }

    public bool SuperKickReady()
    {
        return SuperThreshold.rect.height == 100;
    }

    public void ResetSuperThreshold()
    {
        SuperThreshold.sizeDelta = new Vector2(SuperThreshold.rect.width, 0f);
        SuperButton.SetActive(false);
    }

    public int GetPlayerCount()
    {
        return TopPlayers.transform.childCount;
    }

    public Vector3 GetPlayerPosition()
    {
        return player.transform.position;
    }

    public void ShowHasScored()
    {
        HasScoredMessage.SetActive(true);
    }

    public void HideHasScored()
    {

        HasScoredMessage.SetActive(false);
    }

    public void PauseGame()
    {
        Time.timeScale = 0;
        PausePanel.SetActive(true);
    }
    public void ResumeGame()
    {
        Time.timeScale = 1;
        PausePanel.SetActive(false);
    }

    public void ReturnToMenu()
    {
        if (SelectionSingleton.instance.IsMinigame)
        {
            SceneManager.LoadScene("VSAllCountries");
        }
        else
        {
            SceneManager.LoadScene("MainMenu");
        }
    }

    public void PlayKickAudio()
    {
        audioSource.PlayOneShot(kickAudio, sfxVol);
    }
    public void PlayIgniteAudio()
    {
        audioSource.PlayOneShot(igniteAudio, sfxVol);
    }
}

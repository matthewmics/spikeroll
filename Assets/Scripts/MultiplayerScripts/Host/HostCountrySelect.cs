using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

public class HostCountrySelect : MonoBehaviour
{

    public Button ButtonSelectCountry; 

    PlayerHost server;

    [HideInInspector]
    public ButtonHostCountrySelect SelectedPlayer1 = null;
    [HideInInspector]
    public ButtonHostCountrySelect SelectedPlayer2 = null;

    public static HostCountrySelect Instance = null;
    // Start is called before the first frame update
    void Start()
    {
        Instance = this;
        server = GameObject.Find("Server").GetComponent<PlayerHost>();

        int count = transform.childCount;
        for (int i = 0; i < count; i++)
        {
            var gObj = transform.GetChild(i).GetComponent<ButtonHostCountrySelect>();
            var gBtn = transform.GetChild(i).GetComponent<Button>();
            gObj.DeselectPlayer1();
            gObj.DeselectPlayer2();
            gBtn.onClick.AddListener(() =>
            {
                if (!ButtonSelectCountry.interactable)
                    return;

                if(SelectedPlayer1 != null)
                {
                    SelectedPlayer1.DeselectPlayer1();                    
                }
                SelectedPlayer1 = gObj;
                gObj.SelectPlayer1();

                server.SendPlayer1Selection(gObj.name);
               // Debug.Log(gObj.name);
            });
        }

        ButtonSelectCountry.onClick.AddListener(() =>
        {
            if(PlayerHost.MultiplayerSession.P1Country != null
            && SelectedPlayer1 != null && SelectedPlayer1.gameObject.activeInHierarchy)
            {
                PlayerHost.MultiplayerSession.P1Country = SelectedPlayer1.name;
                server.Send("P1LOCKIN|" + SelectedPlayer1.name, true);
                SelectedPlayer1.gameObject.SetActive(false);
                ButtonSelectCountry.interactable = false;
                ButtonSelectCountry.transform.GetChild(0).GetComponent<Text>().text = "WAITING FOR OTHER PLAYER";
            }
        });
    }

    public void LockInPlayer2()
    {
        if (PlayerHost.MultiplayerSession.P2Country != null
            && SelectedPlayer2 != null && SelectedPlayer2.gameObject.activeInHierarchy)
        {
            PlayerHost.MultiplayerSession.P2Country = SelectedPlayer2.name;
            server.Send("P2LOCKIN|" + SelectedPlayer2.name, true);
            SelectedPlayer2.gameObject.SetActive(false);
            //ButtonSelectCountry.interactable = false;
            //ButtonSelectCountry.transform.GetChild(0).GetComponent<Text>().text = "WAITING FOR OTHER PLAYER";
        }
    }


    public void SelectPlayer2(string sel)
    {
        var obj = this.transform.Find(sel).GetComponent<ButtonHostCountrySelect>();

        if (SelectedPlayer2 != null)
        {
            SelectedPlayer2.DeselectPlayer2();
        }
        SelectedPlayer2 = obj;
        obj.SelectPlayer2();

    }


    // Update is called once per frame
    void Update()
    {
        if(!string.IsNullOrWhiteSpace(PlayerHost.MultiplayerSession.P1Country) &&
           !string.IsNullOrWhiteSpace(PlayerHost.MultiplayerSession.P2Country))
        {
            // gameObject.SetActive(false);
            transform.parent.gameObject.SetActive(false);
            Debug.Log("both players ready");

            PlayerHost.MultiplayerSession.CountryPl = CountryModel.GetCountryByShortName(PlayerHost.MultiplayerSession.P1Country);
            PlayerHost.MultiplayerSession.CountryP2 = CountryModel.GetCountryByShortName(PlayerHost.MultiplayerSession.P2Country);

            RuntimeAnimatorController player1Controller = PlayerHost.MultiplayerSession.CountryPl.ControllerBottom1;
            RuntimeAnimatorController player2Controller = PlayerHost.MultiplayerSession.CountryP2.ControllerTop1;

            CountryModel p1country = PlayerHost.MultiplayerSession.CountryPl;
            CountryModel p2country = PlayerHost.MultiplayerSession.CountryP2;

            // server.NetworkObjects.transform.Find("player1").GetComponent<Animator>().runtimeAnimatorController = player1Controller;
            //server.NetworkObjects.transform.Find("player2").GetComponent<Animator>().runtimeAnimatorController = player2Controller;
            int count = server.NetworkObjects.transform.childCount;
            for(int i = 0; i < count; i++)
            {
                Transform t = server.NetworkObjects.transform.GetChild(i);

                Animator animator = t.GetComponent<Animator>(); 

                MultiplayerAIController ai = t.GetComponent<MultiplayerAIController>();

                HostPlayerController hplayer = t.GetComponent<HostPlayerController>();
                NetworkClientController cplayer = t.GetComponent<NetworkClientController>();


                if (t.name.Contains("player1"))
                {
                    animator.runtimeAnimatorController = player1Controller;
                    if (ai != null)
                    {
                        ai.SetSpeed(p1country.RawSpeed);
                        ai.SetPower(p1country.RawPower);
                        ai.SetInt(p1country.RawInt);
                    }else if(hplayer != null)
                    {
                        hplayer.SetSpeed(p1country.RawSpeed);
                        hplayer.SetPower(p1country.RawPower);
                        hplayer.SetInt(p1country.RawInt);
                    }

                }
                else if (t.name.Contains("player2"))
                {
                    animator.runtimeAnimatorController = player2Controller;
                    if (ai != null)
                    {
                        ai.SetSpeed(p2country.RawSpeed);
                        ai.SetPower(p2country.RawPower);
                        ai.SetInt(p2country.RawInt);
                    }
                    else if (cplayer != null)
                    {
                        cplayer.SetSpeed(p2country.RawSpeed);
                        cplayer.SetPower(p2country.RawPower);
                        cplayer.SetInt(p2country.RawInt);
                    }

                }
            }

            //server.NetworkObjects.SetActive(true);

            server.MultiplayerUIScript.ShowModal("Loading...");
            server.Send("HOSTACTION|PICKINGSDONE%" + PlayerHost.MultiplayerSession.P1Country + "%" + PlayerHost.MultiplayerSession.P2Country
                , true);

            ScoreboardUI.Scoreboard.SetPlayer1Name("Player 1" + "(" + PlayerHost.MultiplayerSession.P1Country + ")");
            ScoreboardUI.Scoreboard.SetPlayer2Name("Player 2" + "(" + PlayerHost.MultiplayerSession.P2Country + ")");
        }
    }

}
